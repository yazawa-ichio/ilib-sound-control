using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ILib.Audio
{
	internal class MusicObject
	{
		public static MusicObject Create(Transform parent)
		{
			MusicObject ret = new MusicObject();
			var obj = new GameObject(nameof(MusicObject));
			obj.transform.SetParent(parent);
			var source = obj.AddComponent<AudioSource>();
			source.playOnAwake = false;
			source.priority = 0;
			source.loop = true;
			ret.m_Object = obj;
			ret.m_Source = source;
			ret.m_TransformCache = source.transform;
			obj.SetActive(false);
			return ret;
		}

		GameObject m_Object;
		AudioSource m_Source;
		Transform m_TransformCache;
		public MusicRequest Request { get; private set; }
		public MusicInfo Music { get; private set; }
		ValueTweener m_FadeVolume = new ValueTweener(0);

		bool m_Stop;
		public bool IsPause { get; set; }

		public object Entry { get; private set; }

		public void PlayRequest(MusicRequest req, object entry, float time)
		{
			m_Stop = false;
			Entry = entry;
			Request = req;
			Music = req.Music;
			Music.AddRef();

			m_Object.SetActive(true);

			m_Source.clip = Music.Clip;
			m_Source.outputAudioMixerGroup = Music.Group;
			m_Source.pitch = Music.Pitch;
			m_Source.volume = 0;
			m_Source.timeSamples = Request.Position;
			m_Source.Play();
			m_FadeVolume.Start(0, 1f, time);
		}

		public bool Update()
		{
			if (Request == null) return false;

			if (m_FadeVolume.IsRunning)
			{
				m_Source.volume = Music.Volume * m_FadeVolume.Get(Time.unscaledDeltaTime);
			}

			return true;
		}

		public void Stop(float time)
		{
			m_Stop = true;
			if (IsPause)
			{
				Finish();
			}
			else
			{
				m_FadeVolume.Start(0f, m_FadeVolume.CurrentValue * time, Finish);
			}
		}

		public void Pause(float time)
		{
			if (m_Stop || IsPause) return;
			m_FadeVolume.Start(0f, time, () =>
			{
				m_Source.Pause();
				IsPause = true;
			});
		}

		public void Resume(float time)
		{
			if (m_Stop || !IsPause) return;
			m_Source.UnPause();
			IsPause = false;
			m_FadeVolume.Stop();
			m_FadeVolume.Start(1f, time);
		}

		void Finish()
		{
			if (Request != null)
			{
				Request.Position = m_Source.timeSamples;
			}
			IsPause = false;
			m_Source.Stop();
			m_Source.clip = null;

			Request = null;
			Music?.RemoveRef();
			Music = null;

			m_Object.SetActive(false);

		}

		public void Destroy()
		{
			GameObject.Destroy(m_Object);
		}

	}
}
