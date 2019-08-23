using System;

namespace ILib.Audio
{

	public class CacheScope : ICacheScope
	{

		Action<bool> m_OnLoaded;
		public Action OnDispose;

		public int Revision { get; set; }
		public bool IsLoaded { get; private set; }
		public bool IsSuccess { get; private set; }

		public void OnLoaded(bool success)
		{
			IsSuccess = success;
			IsLoaded = true;
			m_OnLoaded?.Invoke(success);
			m_OnLoaded = null;
		}

		~CacheScope()
		{
			Dispose();
		}

		public void Dispose()
		{
			m_OnLoaded = null;
			OnDispose();
			GC.SuppressFinalize(this);
		}

		public void ObserveLoad(Action<bool> onLoaded)
		{
			if (IsLoaded)
			{
				onLoaded?.Invoke(IsSuccess);
			}
			else
			{
				m_OnLoaded += onLoaded;
			}
		}
	}
}
