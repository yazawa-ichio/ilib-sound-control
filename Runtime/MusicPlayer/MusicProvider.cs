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
	public class MusicProvider : MusicProviderBase<string>, IMusicProvider
	{
		public MusicProvider() { }

		public MusicProvider(AudioMixerGroup group, string format) : base(group, format) { }

		protected override string GetString(string prm)
		{
			return prm;
		}
	}

	/// <summary>
	/// パスでロードが解決できる場合のプロバイダーです。
	/// Resourcesをパスで探索します
	/// </summary>
	public class MusicProvider<T> : MusicProviderBase<T> where T : ICacheKey
	{
		public MusicProvider() { }

		public MusicProvider(AudioMixerGroup group, string format) : base(group, format) { }

		protected override string GetString(T prm)
		{
			return prm.GetCacheKey();
		}
	}

	/// <summary>
	/// パスでロードが解決できる場合のプロバイダーです。
	/// Resourcesをパスで探索します
	/// </summary>
	public class GeneralMusicProvider<T> : MusicProviderBase<T>
	{
		public GeneralMusicProvider() { }

		public GeneralMusicProvider(AudioMixerGroup group, string format) : base(group, format) { }

		protected override string GetString(T prm)
		{
			return prm.ToString();
		}
	}

	/// <summary>
	/// パスでロードが解決できる場合のプロバイダーです。
	/// Resourcesをパスで探索します
	/// </summary>
	public class IntKeyMusicProvider<T> : MusicProviderBase<T> where T : struct, IConvertible
	{
		public IntKeyMusicProvider() { }

		public IntKeyMusicProvider(AudioMixerGroup group, string format) : base(group, format) { }

		protected override string GetString(T prm)
		{
			return prm.ToInt32(null).ToString();
		}
	}

	/// <summary>
	/// パスでロードが解決できる場合のプロバイダーです。
	/// Resourcesをパスで探索します
	/// </summary>
	public abstract class MusicProviderBase<T> : IMusicProvider<T>
	{
		Func<string, string> m_PathConversion;
		public Func<string, string> PathConversion { set => m_PathConversion = value; }

		Func<string, AudioMixerGroup> m_GroupSelector;
		public Func<string, AudioMixerGroup> GroupSelector { set => m_GroupSelector = value; }

		Func<T, Action<MusicInfo, Exception>, bool> m_CustomLoad;
		public Func<T, Action<MusicInfo, Exception>, bool> CustomLoad { set => m_CustomLoad = value; }

		public MusicProviderBase() { }

		public MusicProviderBase(AudioMixerGroup group, string format)
		{
			SetGroup(group);
			SetPathFormat(format);
		}

		public MusicProviderBase<T> SetGroup(AudioMixerGroup group)
		{
			m_GroupSelector = (x) => group;
			return this;
		}

		public MusicProviderBase<T> SetPathFormat(string format)
		{
			m_PathConversion = x => string.Format(format, x);
			return this;
		}

		protected abstract string GetString(T prm);

		protected string GetPath(T prm)
		{
			var key = GetString(prm);
			if (m_PathConversion != null)
			{
				key = m_PathConversion(key);
			}
			return key;
		}

		bool IMusicProvider<T>.Load(T prm, Action<MusicInfo, Exception> onComplete)
		{
			return Load(prm, onComplete);
		}

		protected virtual bool Load(T prm, Action<MusicInfo, Exception> onComplete)
		{
			if (m_CustomLoad != null)
			{
				return m_CustomLoad(prm, onComplete);
			}
			string path = GetPath(prm);
			var op = Resources.LoadAsync(path);
			op.completed += _ =>
			{
				var data = op.asset as MusicData;
				if (data != null)
				{
					onComplete?.Invoke(data.CreateMusic(m_GroupSelector), null);
					return;
				}

				var clip = op.asset as AudioClip;
				if (clip == null)
				{
					onComplete?.Invoke(null, new System.IO.FileNotFoundException("Not Found AudioClip.", path));
				}
				else
				{
					MusicInfo info = new MusicInfo();
					info.Clip = clip;
					info.Volume = 1f;
					info.Pitch = 1f;
					if (m_GroupSelector != null)
					{
						info.Group = m_GroupSelector("");
					}
					onComplete?.Invoke(info, null);
				}
			};
			return true;
		}

	}

}
