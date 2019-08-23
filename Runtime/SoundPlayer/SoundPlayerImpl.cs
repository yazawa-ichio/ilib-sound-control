using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace ILib.Audio
{

	/// <summary>
	/// サウンドプレイヤーの実体です
	/// </summary>
	public class SoundPlayerImpl : SoundPlayerImpl<string> , ISoundPlayer
	{
		public SoundPlayerImpl(ISoundProvider<string> provider, SoundPlayerConfig config = null) : base(provider, config) { }
	}

	/// <summary>
	/// サウンドプレイヤーの実体です
	/// </summary>
	public class SoundPlayerImpl<T> : ISoundPlayer<T> 
	{
		public float LoadTimeout { get; set; } = 2f;

		public bool IsCreateIfNotEnough { get; set; }

		public int MaxPoolCount
		{
			get => m_PlayingList.MaxPoolCount;
			set => m_PlayingList.MaxPoolCount = value;
		}

		public bool IsAddCacheIfLoad { get; set; }

		Cache m_Cache;
		ISoundProvider<T> m_Provider;
		PlayingList m_PlayingList;
		bool m_Disposed;
		bool m_Removed;

		public SoundPlayerImpl(ISoundProvider<T> provider, SoundPlayerConfig config = null)
		{
			m_Provider = provider;
			m_PlayingList = config?.PlayingList ?? SoundControl.SharedPlayingList;
			m_PlayingList.AddRef();
			m_Cache = config?.Cache ?? new Cache();
			m_Cache.AddRef();
			if (config != null)
			{
				LoadTimeout = config.LoadTimeout;
				IsCreateIfNotEnough = config.IsCreateIfNotEnough;
				IsAddCacheIfLoad = config.IsAddCacheIfLoad;
				if (m_PlayingList.MaxPoolCount < config.InitMaxPoolCount)
				{
					m_PlayingList.MaxPoolCount = config.InitMaxPoolCount;
				}
			}
		}

		~SoundPlayerImpl()
		{
			Dispose();
		}

		public void ReservePool(int count = -1)
		{
			m_PlayingList.ReservePool(count);
		}

		protected string GetCacheKey(T prm)
		{
			return m_Provider.GetCacheKey(prm);
		}

		public IPlayingSoundContext Play(T prm)
		{
			if (m_Disposed) return PlayingSoundContext.Empty;
			string key = GetCacheKey(prm);
			var ctx = new PlayingSoundContext();
			ctx.CreateTime = Time.unscaledTime;
			var info = m_Cache.GetInfo(key);
			if (info != null)
			{
				m_PlayingList.Play(info, m_Provider.MixerGroup, ctx, IsCreateIfNotEnough);
				return ctx;
			}
			else
			{
				ctx.IsLoading = true;
				ctx.LoadingTimeout = LoadTimeout;
				var ret = m_Provider.Load(prm, (x, ex) =>
				{
					if (m_Disposed)
					{
						ctx.PlayFail(ex);
						return;
					}
					OnLoad(x, ex, ctx);
					if (x != null && IsAddCacheIfLoad)
					{
						m_Cache.Add(key, false, x);
					}
				});
				return ret ? ctx : PlayingSoundContext.Empty;
			}
		}

		public IPlayingSoundContext Play(SoundInfo info)
		{
			if (m_Disposed) return PlayingSoundContext.Empty;
			var ctx = new PlayingSoundContext();
			ctx.CreateTime = Time.unscaledTime;
			m_PlayingList.Play(info, m_Provider.MixerGroup, ctx, IsCreateIfNotEnough);
			return ctx;
		}

		public void PlayOneShot(T prm)
		{
			if (m_Disposed) return;
			var key = GetCacheKey(prm);
			var info = m_Cache.GetInfo(key);
			if (info != null)
			{
				m_PlayingList.Play(info, m_Provider.MixerGroup, null, IsCreateIfNotEnough);
			}
			else
			{
				var startTime = Time.unscaledTime;
				m_Provider.Load(prm, (x, ex) =>
				{
					if (m_Disposed) return;
					if (Time.unscaledDeltaTime - startTime < LoadTimeout)
					{
						OnLoad(x, ex, null);
					}
					if (x != null && IsAddCacheIfLoad)
					{
						m_Cache.Add(key, false, x);
					}
				});
			}
		}

		public void PlayOneShot(SoundInfo info)
		{
			if (m_Disposed) return;
			m_PlayingList.Play(info, m_Provider.MixerGroup, null, IsCreateIfNotEnough);
		}

		public void AddCache(T prm, Action<bool, Exception> onLoad)
		{
			if (m_Disposed)
			{
				onLoad?.Invoke(false, new Exception("disposed sound player."));
				return;
			}
			var key = GetCacheKey(prm);
			var cacheEmpty = false;
			m_Cache.Add(key, false, ref cacheEmpty);
			if (cacheEmpty)
			{
				m_Provider.Load(prm, (x, ex) => m_Cache.OnLoad(key, onLoad, x, ex));
			}
			else
			{
				onLoad?.Invoke(true, null);
			}
		}

		public void RemoveCache(T prm)
		{
			if (m_Disposed) return;
			m_Cache.Remove(GetCacheKey(prm), false);
		}

		public ICacheScope CreateCacheScope(T[] prms)
		{
			if (m_Disposed) return null;
			var keys = prms.Select(x => GetCacheKey(x)).ToArray();
			var scope = m_Cache.CreateScope(keys);
			int count = 0;
			bool success = true;
			for (int i = 0; i < keys.Length; i++)
			{
				var cacheEmpty = false;
				m_Cache.Add(keys[i], false, ref cacheEmpty);
				if (!cacheEmpty)
				{
					count++;
					//完了チェック
					if (count == keys.Length) scope.OnLoaded(success);
				}
				else
				{
					m_Provider.Load(prms[i], (ret, ex) =>
					{
						count++;
						if (ex != null) success = false;
						//完了チェック
						if (count == keys.Length) scope.OnLoaded(success);
					});
				}
			}
			return scope;
		}

		public void ClearCache(bool force = false)
		{
			if (m_Disposed) return;
			m_Cache.Clear(!force);
		}

		internal void OnLoad(SoundInfo info, Exception error, PlayingSoundContext context)
		{
			if (error != null)
			{
				context?.PlayFail(error);
				return;
			}
			if (m_Disposed || info == null)
			{
				context?.PlayFail(new AbortException("再生を中断しました"));
				return;
			}
			//タイムアウト判定
			if (context != null && (Time.unscaledDeltaTime - context.CreateTime > context.LoadingTimeout))
			{
				context?.PlayFail(new TimeoutException("ロードの遅延でタイムアウトが発生しました"));
				return;
			}
			if (context != null) context.IsLoading = false;
			m_PlayingList.Play(info, m_Provider.MixerGroup, context, IsCreateIfNotEnough);
		}

		public void Dispose()
		{
			if (m_Disposed) return;
			m_Disposed = true;
			m_Cache.RemoveRef();
			m_PlayingList.RemoveRef();
		}

	}

}
