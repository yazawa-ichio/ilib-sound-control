using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ILib.Audio
{
	public interface IMusicProvider : IMusicProvider<string>
	{
	}

	public interface IMusicProvider<T>
	{
		bool Load(T prm, System.Action<MusicInfo, System.Exception> onComplete);
	}

	public static class IMusicProviderEx
	{
		/// <summary>
		/// ミュージックプレイヤーを作成します。
		/// </summary>
		public static IMusicPlayer Create(this IMusicProvider self, MusicPlayerConfig config = null)
		{
			return SoundControl.CreatePlayer(self, config);
		}

		/// <summary>
		/// ミュージックプレイヤーを作成します。
		/// </summary>
		public static IMusicPlayer<T> Create<T>(this IMusicProvider<T> self, MusicPlayerConfig config = null)
		{
			return SoundControl.CreatePlayer(self, config);
		}

	}
}
