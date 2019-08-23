using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ILib.Audio
{
	public class ValueTweener
	{
		public bool IsRunning;
		public float CurrentValue;
		float m_TweenTime;
		float m_CurrentTime;
		float m_StartValue;
		float m_EndValue;
		System.Action m_OnComplete;
		System.Func<float, float> m_Easing;

		public ValueTweener(float initValue, System.Func<float, float> easing = null)
		{
			CurrentValue = initValue;
			m_Easing = easing;
		}

		public void Start(float start, float end, float time, System.Action onComplete = null)
		{
			IsRunning = true;
			m_OnComplete = onComplete;
			m_CurrentTime = 0;
			m_TweenTime = time;
			m_StartValue = start;
			m_EndValue = end;
			CurrentValue = start;
		}

		public void Start(float end, float time, System.Action onComplete = null)
		{
			IsRunning = true;
			m_OnComplete = onComplete;
			m_CurrentTime = 0;
			m_TweenTime = time;
			m_StartValue = CurrentValue;
			m_EndValue = end;
		}

		public void Stop(bool complete = false)
		{
			if (!IsRunning) return;
			IsRunning = false;
			if (complete)
			{
				CurrentValue = m_EndValue;
				m_OnComplete?.Invoke();
				m_OnComplete = null;
			}
		}

		public float Get(float delta)
		{
			if (!IsRunning) return CurrentValue;
			m_CurrentTime += delta;
			if (m_CurrentTime > m_TweenTime)
			{
				IsRunning = false;
				m_OnComplete?.Invoke();
				m_OnComplete = null;
				return CurrentValue = m_EndValue;
			}
			else
			{
				float rate = (m_CurrentTime / m_TweenTime);
				if (m_Easing != null)
				{
					rate = m_Easing(rate);
				}
				return CurrentValue = m_StartValue + (m_EndValue - m_StartValue) * rate;
			}
		}
	}
}
