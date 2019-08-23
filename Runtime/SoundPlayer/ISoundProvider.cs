using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace ILib.Audio
{
	public interface ISoundProvider<T>
	{
		AudioMixerGroup MixerGroup { get; }

		string GetCacheKey(T prm);
		
		bool Load(T prm, System.Action<SoundInfo, System.Exception> onComplete);
	}

	public static class ISoundProviderEx
	{
		/// <summary>
		/// サウンドプレイヤーを作成します。
		/// </summary>
		public static ISoundPlayer Create(this ISoundProvider<string> self, SoundPlayerConfig config = null)
		{
			return SoundControl.CreatePlayer(self, config);
		}

		/// <summary>
		/// サウンドプレイヤーを作成します。
		/// </summary>
		public static ISoundPlayer<T> Create<T>(this ISoundProvider<T> self, SoundPlayerConfig config = null)
		{
			return SoundControl.CreatePlayer(self, config);
		}

	}

}
