using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace ILib.Audio
{

	/// <summary>
	/// 再生中のサウンドのコントローラーの実体です。
	/// </summary>
	internal class PlayingSoundContext : IPlayingSoundContext
	{
		public static readonly PlayingSoundContext Empty = new PlayingSoundContext() { m_Disposed = true };

		public bool IsLoading { get; set; } = true;

		public bool IsValid
		{
			get
			{
				if (m_Disposed) return false;
				return IsLoading || m_Target.IsCurrent(this);
			}
		}

		bool CanControl => !IsLoading && m_Stop && IsValid;

		public float Volume
		{
			get => m_FadeVolume?.CurrentValue ?? 1f;
			set
			{
				if (CanControl)
				{
					if (m_FadeVolume == null)
					{
						m_FadeVolume = new ValueTweener(value);
						m_Target.Volume = value;
					}
					else
					{
						m_FadeVolume.Stop();
						m_Target.Volume = m_FadeVolume.CurrentValue = value;
					}
				}
			}
		}

		float m_Pitch = 1f;
		public float Pitch
		{
			get => m_Pitch;
			set
			{
				m_Pitch = value;
				if (CanControl)
				{
					m_Target.Pitch = m_Pitch;
				}
			}
		}

		bool m_Loop = false;
		public bool Loop
		{
			get => m_Loop;
			set
			{
				m_Loop = value;
				if (CanControl)
				{
					m_Target.Loop = value;
				}
			}
		}

		bool m_Pause;
		public bool Pause
		{
			get => m_Pause;
			set
			{
				if (m_Pause == value) return;
				m_Pause = value;
				if (CanControl)
				{
					m_Target.IsPause = value;
					if (value)
					{
						m_Target.Pause();
					}
					else
					{
						m_Target.Resume();
					}
				}
			}
		}

		public System.Exception Error { get; private set; }

		public float CreateTime;
		public float LoadingTimeout;

		ValueTweener m_FadeVolume;
		bool m_Stop = false;
		bool m_Disposed;
		SoundObject m_Target = null;

		public void Play(SoundObject obj)
		{
			IsLoading = false;
			if (m_Disposed)
			{
				obj.Stop();
				return;
			}
			m_Target = obj;
			obj.IsPause = m_Pause;
			if (!m_Pause)
			{
				m_Target.Play();
			}
			m_Target.Loop = m_Loop;
			m_Target.Pitch = m_Pitch;
		}

		public void PlayFail(System.Exception error)
		{
			Error = error;
			Dispose();
		}

		public void Update()
		{
			if (!IsValid) return;
			if (m_FadeVolume != null && m_FadeVolume.IsRunning)
			{
				m_Target.Volume = m_FadeVolume.Get(Time.unscaledDeltaTime);
			}
		}

		public void Stop()
		{
			m_Stop = true;
			Dispose();
		}

		public void Dispose()
		{
			if (m_Disposed) return;
			m_Disposed = true;
			if (m_Target != null)
			{
				if (m_Target.IsCurrent(this))
				{
					m_Target.Stop();
				}
			}
		}

		public void Detach()
		{
			m_Target = null;
		}

		public void FadeIn(float time)
		{
			if (!IsValid && m_Stop) return;
			Volume = 0f;
			if (m_FadeVolume == null) m_FadeVolume = new ValueTweener(0);
			m_FadeVolume.Start(0, 1f, time);
		}
		
		public void Fade(float end, float time)
		{
			if (!IsValid && m_Stop) return;
			if (m_FadeVolume == null) m_FadeVolume = new ValueTweener(Volume);
			m_FadeVolume.Start(Volume, end, time);
		}

		public void Fade(float start, float end, float time)
		{
			if (!IsValid && m_Stop) return;
			Volume = start;
			if (m_FadeVolume == null) m_FadeVolume = new ValueTweener(start);
			m_FadeVolume.Start(start, end, time);
		}

		public void FadeOut(float time = 0.5f)
		{
			if (!IsValid && m_Stop) return;
			if (m_FadeVolume == null) m_FadeVolume = new ValueTweener(Volume);
			m_FadeVolume.Start(Volume, 0, time, () => Dispose());
			m_Stop = true;
		}
	}
}
