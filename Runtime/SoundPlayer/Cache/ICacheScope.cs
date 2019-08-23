using System;

namespace ILib.Audio
{
	public interface ICacheScope : IDisposable
	{
		bool IsLoaded { get; }
		bool IsSuccess { get; }
		void ObserveLoad(Action<bool> onLoaded);
	}
}
