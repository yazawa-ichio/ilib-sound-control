using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace ILib.Audio
{
	/// <summary>
	/// パスでロードが解決できる場合のプロバイダーです。
	/// Resourcesをパスで探索します
	/// </summary>
	public class SoundProvider : SoundProviderBase<string>
	{
		public SoundProvider() { }

		public SoundProvider(AudioMixerGroup group, string format) : base(group, format) { }

		public override string GetCacheKey(string prm)
		{
			return prm;
		}
	}

	/// <summary>
	/// パスでロードが解決できる場合のプロバイダーです。
	/// Resourcesをパスで探索します
	/// </summary>
	public class SoundProvider<T> : SoundProviderBase<T> where T : ICacheKey
	{
		public SoundProvider() { }

		public SoundProvider(AudioMixerGroup group, string format) : base(group, format) { }

		public override string GetCacheKey(T prm)
		{
			return prm.GetCacheKey();
		}
	}

	/// <summary>
	/// パスでロードが解決できる場合のプロバイダーです。
	/// Resourcesをパスで探索します
	/// </summary>
	public class GeneralSoundProvider<T> : SoundProviderBase<T>
	{
		public GeneralSoundProvider() { }

		public GeneralSoundProvider(AudioMixerGroup group, string format) : base(group, format) { }

		public override string GetCacheKey(T prm)
		{
			return prm.ToString();
		}
	}

	/// <summary>
	/// パスでロードが解決できる場合のプロバイダーです。
	/// Resourcesをパスで探索します
	/// </summary>
	public class IntKeySoundProvider<T> : SoundProviderBase<T> where T : struct, IConvertible
	{
		public IntKeySoundProvider() { }

		public IntKeySoundProvider(AudioMixerGroup group, string format) : base(group, format) { }

		public override string GetCacheKey(T prm)
		{
			return prm.ToInt32(null).ToString();
		}
	}

	/// <summary>
	/// パスでロードが解決できる場合のプロバイダーです。
	/// Resourcesをパスで探索します
	/// </summary>
	public abstract class SoundProviderBase<T> : ISoundProvider<T>
	{

		public AudioMixerGroup MixerGroup { get; set; }

		Func<string, string> m_PathConversion;
		public Func<string, string> PathConversion { set => m_PathConversion = value; }

		Func<T, Action<SoundInfo, Exception>, bool> m_CustomLoad;
		public Func<T, Action<SoundInfo, Exception>, bool> CustomLoad { set => m_CustomLoad = value; }

		public SoundProviderBase() { }

		public SoundProviderBase(AudioMixerGroup group, string format)
		{
			MixerGroup = group;
			if (!string.IsNullOrEmpty(format))
			{
				SetPathFormat(format);
			}
		}

		public SoundProviderBase<T> SetPathFormat(string format)
		{
			m_PathConversion = x => string.Format(format, x);
			return this;
		}

		public abstract string GetCacheKey(T prm);

		protected string GetPath(T prm)
		{
			var key = GetCacheKey(prm);
			if (m_PathConversion != null)
			{
				key = m_PathConversion(key);
			}
			return key;
		}

		bool ISoundProvider<T>.Load(T prm, Action<SoundInfo, Exception> onComplete)
		{
			return Load(prm, onComplete);
		}

		protected virtual bool Load(T prm, Action<SoundInfo, Exception> onComplete)
		{
			if (m_CustomLoad != null)
			{
				return m_CustomLoad(prm, onComplete);
			}
			string path = GetPath(prm);
			var op = Resources.LoadAsync(path);
			op.completed += _ =>
			{
				var data = op.asset as SoundData;
				if (data != null)
				{
					onComplete?.Invoke(data.CreateInfo(), null);
					return;
				}

				var clip = op.asset as AudioClip;
				if (clip == null)
				{
					onComplete?.Invoke(null, new System.IO.FileNotFoundException("Not Found AudioClip.", path));
				}
				else
				{
					SoundInfo info = new SoundInfo();
					info.Clip = clip;
					info.ControlId = path;
					onComplete?.Invoke(info, null);
				}
			};
			return true;
		}
	}

}
