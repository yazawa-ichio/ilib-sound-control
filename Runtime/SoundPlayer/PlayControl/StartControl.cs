using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ILib.Audio
{
	public abstract class StartControl : IStartControl
	{
		public enum Result
		{
			Start,
			Wait,
			Reject,
		}

		public enum Type
		{
			None,
			LimitCount,
			LimitInterval,
			FirstComeFirstServed,
			LateArrivalPriority,
		}

		public static IStartControl Create(Type type, float prm1, float prm2)
		{
			switch (type)
			{
				case Type.LimitCount: return new LimitCountControl((int)prm1);
				case Type.LimitInterval: return new LimitIntervalControl(prm1, prm2);
				case Type.FirstComeFirstServed: return new FirstComeFirstServedControl();
				case Type.LateArrivalPriority: return new LateArrivalPriorityControl(prm1);
			}
			return null;
		}

		public abstract Result Update(IPlayingList list, SoundInfo info);

	}

	class LimitCountControl : StartControl
	{
		int m_Count;
		public LimitCountControl(int count) => m_Count = count;

		public override Result Update(IPlayingList list, SoundInfo info)
		{
			if (list.GetCount(info.ControlId) < m_Count)
			{
				return Result.Start;
			}
			return Result.Reject;
		}
	}

	class LimitIntervalControl : StartControl
	{
		float m_Interval;
		float m_WaitTime;

		public LimitIntervalControl(float interval, float wait = 0.1f)
		{
			m_Interval = interval;
			m_WaitTime = wait;
		}

		public override Result Update(IPlayingList list, SoundInfo info)
		{
			if ((System.DateTime.UtcNow - list.GetLastPlayStartTime(info.ControlId)).TotalSeconds >= m_Interval)
			{
				return Result.Start;
			}
			m_WaitTime -= Time.unscaledDeltaTime;
			return (m_WaitTime > 0) ? Result.Wait : Result.Reject;
		}
	}

	class FirstComeFirstServedControl : StartControl
	{
		public override Result Update(IPlayingList list, SoundInfo info)
		{
			return list.GetCount(info.ControlId) == 0 ? Result.Start : Result.Reject;
		}
	}

	class LateArrivalPriorityControl : StartControl
	{
		float m_FadeTime;

		public LateArrivalPriorityControl(float fadeTime)
		{
			m_FadeTime = fadeTime;
		}
		public override Result Update(IPlayingList list, SoundInfo info)
		{
			list.StopAll(info.ControlId);
			return Result.Start;
		}
	}

}
