using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace ILib.Audio
{
	internal class SoundObject
	{
		public static SoundObject Create(PlayingList playingList, Transform parent)
		{
			SoundObject ret = new SoundObject();
			var obj = new GameObject(nameof(SoundObject));
			obj.transform.SetParent(parent);
			var source = obj.AddComponent<AudioSource>();
			source.playOnAwake = false;
			ret.m_PlayingList = playingList;
			ret.m_Object = obj;
			ret.m_Source = source;
			ret.m_TransformCache = source.transform;
			obj.SetActive(false);
			return ret;
		}

		bool m_Init = false;
		PlayingList m_PlayingList;
		SoundInfo m_Info;
		AudioMixerGroup m_Group;
		PlayingSoundContext m_Context;
		GameObject m_Object;
		AudioSource m_Source;
		Transform m_TransformCache;

		public bool IsCurrent(PlayingSoundContext context)
		{
			return context == m_Context;
		}

		float m_Pitch = 1f;
		public float Pitch
		{
			get => m_Pitch;
			set
			{
				if (value != m_Pitch)
				{
					m_Pitch = value;
					m_Source.pitch = m_Pitch * m_Info.Pitch;
				}
			}
		}

		float m_Volume = 1f;
		public float Volume
		{
			get => m_Volume;
			set
			{
				if (value != m_Volume)
				{
					m_Volume = value;
					m_Source.volume = m_Info != null ? m_Volume * m_Info.Volume : 1f;
				}
			}
		}

		public bool IsPause { get; set; }
		public bool Loop { set => m_Source.loop = value; }

		public string ControlId;
		public DateTime PlayStartTime;
		public bool m_IsPlay;

		public void PlayRequest(SoundInfo info, AudioMixerGroup group, PlayingSoundContext context)
		{
			m_Init = false;
			m_Info = info;
			m_Group = group;
			ControlId = info.ControlId;
			m_Context = context;
		}

		void Init()
		{
			m_Init = true;
			m_Object.SetActive(true);
			if (m_TransformCache.hasChanged)
			{
				m_TransformCache.hasChanged = false;
				m_TransformCache.position = Vector3.zero;
			}
			m_IsPlay = false;
			m_Source.clip = m_Info.Clip;
			m_Source.outputAudioMixerGroup = m_Group;
			m_Volume = 1f;
			if (m_Source.volume != m_Info.Volume)
			{
				m_Source.volume = m_Info.Volume;
			}
			m_Pitch = 1f;
			if (m_Source.pitch != m_Info.Pitch)
			{
				m_Source.pitch = m_Info.Pitch;
			}
			m_Source.loop = false;
			IsPause = false;
			if (m_Context != null && !ReferenceEquals(m_Context, PlayingSoundContext.Empty))
			{
				m_Context.Play(this);
			}
			else
			{
				Play();
			}
			PlayStartTime = DateTime.UtcNow;
		}

		public void Resume()
		{
			if (m_IsPlay)
			{
				m_Source.UnPause();
			}
			else
			{
				m_Source.Play();
			}
		}

		public void Pause()
		{
			if (m_IsPlay)
			{
				m_Source.Pause();
			}
		}

		public void Play()
		{
			m_IsPlay = true;
			m_Source.Play();
		}

		public bool Update()
		{
			if (!m_Init) Init();
			m_Context?.Update();
			
			if (!IsPause && !m_Source.isPlaying)
			{
				Finish();
				return false;
			}
			return true;
		}

		public void FadeOut(float time)
		{
			if (m_Context != null)
			{
				m_Context.FadeOut(time);
			}
			else
			{
				Stop();
			}
		}

		public void Stop()
		{
			Finish();
			m_PlayingList.Stop(this);
		}

		void Finish()
		{
			m_Info = null;
			ControlId = "";
			m_Group = null;
			if (m_Source.isPlaying)
			{
				m_Source.Stop();
			}
			m_Source.clip = null;
			var ctx = m_Context;
			m_Context = null;
			ctx?.Detach();
			ctx?.Dispose();
			m_Object.SetActive(false);
		}

		public void Destroy()
		{
			GameObject.Destroy(m_Source.gameObject);
		}

	}
}
