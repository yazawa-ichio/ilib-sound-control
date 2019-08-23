using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ILib.Audio
{
	/// <summary>
	/// ミュージックプレイヤーの実体です
	/// </summary>
	public class MusicPlayerImpl : MusicPlayerImpl<string>, IMusicPlayer
	{
		public MusicPlayerImpl(PlayingMusic playingMusic, IMusicProvider provider, MusicPlayerConfig config = null) : base(playingMusic, provider, config) { }
	}

	/// <summary>
	/// ミュージックプレイヤーの実体です
	/// </summary>
	public class MusicPlayerImpl<T> : IMusicPlayer<T>
	{
		IMusicProvider<T> Provider;
		MusicStack m_MusicStack = new MusicStack();
		PlayingMusic m_PlayingMusic;
		bool m_Disposed;
		bool m_Removed;

		public bool IsCacheInfoInStack
		{
			get => m_MusicStack.IsCacheInfoInStack;
			set => m_MusicStack.IsCacheInfoInStack = value;
		}

		public int MaxPoolCount
		{
			get => m_PlayingMusic.MaxPoolCount;
			set => m_PlayingMusic.MaxPoolCount = value;
		}

		public T Current
		{
			get
			{
				var cur = m_MusicStack.Current;
				if (cur != null)
				{
					return (T)cur;
				}
				return default;
			}
		}

		public MusicPlayerImpl(PlayingMusic playingMusic, IMusicProvider<T> provider, MusicPlayerConfig config = null)
		{
			Provider = provider;
			m_PlayingMusic = playingMusic;
			if (config != null)
			{
				IsCacheInfoInStack = config.IsCacheInfoInStack;
				MaxPoolCount = config.MaxPoolCount;
			}
		}

		~MusicPlayerImpl()
		{
			Dispose();
		}

		public void Change(T prm, float time = 2f, bool clearStack = false)
		{
			Change(prm, MusicPlayConfig.Get(time), clearStack);
		}

		public void Change(T prm, MusicPlayConfig config, bool clearStack = false)
		{
			if (m_Disposed) return;
			if (!clearStack && !config.IsOverrideEqualParam && (object)prm == m_MusicStack.Current)
			{
				return;
			}
			if (clearStack)
			{
				m_MusicStack.Clear();
			}
			else
			{
				m_MusicStack.Pop();
			}
			var push = m_MusicStack.Push(prm);
			Play(push, push.Main, config);
		}

		public void Push(T prm, float time = 2f)
		{
			Push(prm, MusicPlayConfig.Get(time));
		}

		public void Push(T prm, MusicPlayConfig config)
		{
			if (m_Disposed) return;
			var cur = m_MusicStack.Current;
			var push = m_MusicStack.Push(prm);
			if (!config.IsOverrideEqualParam && cur == (object)prm)
			{
				return;
			}
			Play(push, push.Main, config);
		}

		public void Pop(float time = 2f, bool startLastPosition = false)
		{
			Pop(MusicPlayConfig.Get(time), startLastPosition);
		}

		public void Pop(MusicPlayConfig config, bool startLastPosition = false)
		{
			if (m_Disposed) return;
			var cur = m_MusicStack.Current;
			var pop = m_MusicStack.Pop();
			if (pop == null)
			{
				Debug.Assert(false, "music stack is empty.");
				Stop(config.FadeOutTime);
				return;
			}
			if (!config.IsOverrideEqualParam && cur == pop.Main.Param)
			{
				return;
			}
			if (!startLastPosition)
			{
				pop.Main.Position = 0;
			}
			Play(pop, pop.Main, config);
		}

		public void Stop(float time = 2f)
		{
			if (m_Disposed) return;
			m_PlayingMusic.SetCurrent(null);
			m_PlayingMusic.Stop(time);
		}

		public void Pause(float time = 0.3f)
		{
			if (m_Disposed) return;
			m_PlayingMusic.Pause(time);
		}

		public void Resume(float time = 0.3f)
		{
			if (m_Disposed) return;
			m_PlayingMusic.Resume(time);
		}

		public void ClearStack()
		{
			if (m_Disposed) return;
			m_MusicStack.Clear();
		}

		void Play(MusicStack.Entry entry, MusicRequest req, MusicPlayConfig config)
		{
			if (m_Disposed) return;

			m_PlayingMusic.SetCurrent(entry);

			if (req.Music != null)
			{
				m_PlayingMusic.Stop(config.FadeOutTime);
				m_PlayingMusic.Play(entry, req, config);
				return;
			}

			var cache = GetCacheInfo(req.Param);
			if (cache != null)
			{
				cache.AddRef();
				req.Music = cache;
				m_PlayingMusic.Stop(config.FadeOutTime);
				m_PlayingMusic.Play(entry, req, config);
				return;
			}

			if (config.SkipLoadWait)
			{
				m_PlayingMusic.Stop(config.FadeOutTime);
			}

			Provider.Load((T)req.Param, (info, ex) =>
			{
				if (!config.SkipLoadWait)
				{
					m_PlayingMusic.Stop(config.FadeOutTime);
				}
				if (ex != null)
				{

				}
				else
				{
					info.AddRef();
					req.Music = info;
					m_PlayingMusic.Play(entry, req, config);
				}
			});
		}

		MusicInfo GetCacheInfo(object prm)
		{
			return m_MusicStack.GetInfo(prm) ?? m_PlayingMusic.GetCacheInfo(prm) ?? null;
		}

		public void Dispose()
		{
			if (m_Disposed) return;
			m_Disposed = true;
			m_MusicStack.Clear();
			m_PlayingMusic.Dispose();
		}
	}
}
