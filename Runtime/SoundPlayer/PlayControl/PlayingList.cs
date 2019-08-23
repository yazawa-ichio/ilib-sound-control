using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace ILib.Audio
{
	[AddComponentMenu("")]
	public class PlayingList : MonoBehaviour, IPlayingList , IDisposable
	{
		struct Request
		{
			public SoundInfo Info;
			public AudioMixerGroup Group;
			public PlayingSoundContext Context;
			public bool Force;
		}

		Transform m_Root;

		int m_TotalCount;
		Stack<SoundObject> m_Stack = new Stack<SoundObject>();

		List<SoundObject> m_Playing = new List<SoundObject>();
		List<Request> m_Request = new List<Request>();

		public int MaxPoolCount { get; set; } = 12;

		int m_RefCount = 0;

		void Awake()
		{
			m_Root = transform;
		}

		internal void AddRef()
		{
			System.Threading.Interlocked.Increment(ref m_RefCount);
		}

		internal void RemoveRef()
		{
			System.Threading.Interlocked.Decrement(ref m_RefCount);
		}

		public void ReservePool(int count = -1)
		{
			if (count < 0) count = MaxPoolCount;
			for (int i = m_TotalCount; i < count; i++)
			{
				m_Stack.Push(SoundObject.Create(this, m_Root));
				m_TotalCount++;
			}
		}

		internal void Play(SoundInfo info, AudioMixerGroup group, PlayingSoundContext context, bool force)
		{
			if (info.PlayControl == null)
			{
				PlayImpl(info, group, context, force);
			}
			else
			{
				switch (info.PlayControl.Update(this, info))
				{
					case StartControl.Result.Start:
						PlayImpl(info, group, context, force);
						break;
					case StartControl.Result.Wait:
						m_Request.Add(new Request
						{
							Info = info,
							Group = group,
							Context = context,
							Force = force
						});
						break;
				}
			}
		}

		void PlayImpl(SoundInfo info, AudioMixerGroup group, PlayingSoundContext context, bool force)
		{
			var sound = Borrow(force);
			if (sound == null)
			{
				context.PlayFail(new NotPoolException("プールから音源を確保できませんでした"));
				return;
			}
			sound.PlayRequest(info, group, context);
			m_Playing.Add(sound);
		}

		void Update()
		{
			if (m_RefCount == 0)
			{
				Dispose();
				return;
			}
			UpdateRequest();
			UpdateSoundObject();
		}

		void UpdateRequest()
		{
			bool remove = false;
			for (int i = 0; i < m_Request.Count; i++)
			{
				var info = m_Request[i].Info;
				switch (info.PlayControl.Update(this, info))
				{
					case StartControl.Result.Start:
						PlayImpl(info, m_Request[i].Group, m_Request[i].Context, m_Request[i].Force);
						m_Request[i] = default;
						remove = true;
						break;
					case StartControl.Result.Reject:
						m_Request[i] = default;
						remove = true;
						break;
				}
			}
			if (remove)
			{
				m_Request.RemoveAll((x) => x.Info == null);
			}
		}

		void UpdateSoundObject()
		{
			bool stop = false;
			for (int i = 0; i < m_Playing.Count; i++)
			{
				var obj = m_Playing[i];
				if (obj == null)
				{
					stop = true;
				}
				else if (!obj.Update())
				{
					Return(m_Playing[i]);
					m_Playing[i] = null;
					stop = true;
				}
			}
			if (stop)
			{
				m_Playing.RemoveAll((x) => x == null);
			}
		}

		internal void Stop(SoundObject soundObject)
		{
			int index = m_Playing.IndexOf(soundObject);
			m_Playing[index] = null;
			Return(soundObject);
		}

		SoundObject Borrow(bool force)
		{
			if (m_Stack.Count > 0)
			{
				return m_Stack.Pop();
			}
			else if (m_TotalCount < MaxPoolCount)
			{
				m_TotalCount++;
				return SoundObject.Create(this, m_Root);
			}
			else if (force)
			{
				return SoundObject.Create(this, m_Root);
			}
			return null;
		}

		void Return(SoundObject obj)
		{
			if (m_Stack.Count < MaxPoolCount)
			{
				m_Stack.Push(obj);
			}
			else
			{
				obj.Destroy();
			}
		}

		int IPlayingList.GetCount(string controlId)
		{
			int count = 0;
			for (int i = 0; i < m_Playing.Count; i++)
			{
				var obj = m_Playing[i];
				if (obj != null && obj.ControlId == controlId)
				{
					count++;
				}
			}
			return count;
		}

		void IPlayingList.StopAll(string controlId)
		{
			for (int i = 0; i < m_Playing.Count; i++)
			{
				var obj = m_Playing[i];
				if (obj != null && obj.ControlId == controlId)
				{
					obj.Stop();
				}
			}
		}

		DateTime IPlayingList.GetLastPlayStartTime(string controlId)
		{
			DateTime last = default;
			for (int i = 0; i < m_Playing.Count; i++)
			{
				var obj = m_Playing[i];
				if (obj != null && obj.ControlId == controlId)
				{
					if (last < obj.PlayStartTime)
					{
						last = obj.PlayStartTime;
					}
				}
			}
			return last;
		}

		public void Dispose()
		{
			m_Playing.Clear();
			m_Stack.Clear();
			m_Request.Clear();
			GameObject.Destroy(gameObject);
		}
	}
}
