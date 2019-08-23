using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace ILib.Audio
{
	public class Cache
	{
		class Entry
		{
			public SoundInfo Info;
			public bool Global;
			public int Count;
		}

		int m_Revision = 0;
		int m_Ref = 0;
		Dictionary<string, Entry> m_Cache = new Dictionary<string, Entry>();

		public void AddRef()
		{
			System.Threading.Interlocked.Increment(ref m_Ref);
		}

		public void RemoveRef()
		{
			var ret = System.Threading.Interlocked.Decrement(ref m_Ref);
			if (ret == 0)
			{
				Clear(true);
			}
		}

		public SoundInfo GetInfo(string key)
		{
			lock (m_Cache)
			{
				Entry entry;
				if (m_Cache.TryGetValue(key, out entry))
				{
					return entry.Info;
				}
				return null;
			}
		}

		public void Add(string key, bool referenceCounting, ref bool cacheEmpty)
		{
			lock (m_Cache)
			{
				Entry entry;
				if (!m_Cache.TryGetValue(key, out entry))
				{
					entry = m_Cache[key] = new Entry();
				}
				if (referenceCounting)
				{
					entry.Count++;
				}
				else
				{
					entry.Global = true;
				}
				cacheEmpty = entry.Info == null;
			}
		}

		public void Add(string key, bool referenceCounting, SoundInfo info)
		{
			lock (m_Cache)
			{
				Entry entry;
				if (!m_Cache.TryGetValue(key, out entry))
				{
					entry = m_Cache[key] = new Entry();
				}
				if (referenceCounting)
				{
					entry.Count++;
				}
				else
				{
					entry.Global = true;
				}
				entry.Info = info;
			}
		}

		public void Remove(string key, bool referenceCounting)
		{
			lock (m_Cache)
			{
				Entry entry;
				if (m_Cache.TryGetValue(key, out entry))
				{
					if (referenceCounting)
					{
						entry.Count--;
					}
					else
					{
						entry.Global = false;
					}
					if (!entry.Global && entry.Count == 0)
					{
						m_Cache.Remove(key);
					}
				}
			}
		}

		public void Clear(bool ignoreReferenceCounting = true)
		{
			lock (m_Cache)
			{
				if (!ignoreReferenceCounting)
				{
					m_Revision++;
					m_Cache.Clear();
					return;
				}
				foreach (var key in m_Cache.Keys.ToArray())
				{
					var entry = m_Cache[key];
					entry.Global = false;
					if (entry.Count == 0)
					{
						m_Cache.Remove(key);
					}
				}
			}
		}

		public CacheScope CreateScope(string[] keys)
		{
			CacheScope scope = new CacheScope();
			scope.Revision = m_Revision;
			scope.OnDispose = () =>
			{
				lock (m_Cache)
				{
					if (scope.Revision != m_Revision)
					{
						//全キャッシュ削除をした場合は古いスコープは無視する
						return;
					}
					foreach (var key in keys)
					{
						Remove(key, true);
					}
				}
			};
			return scope;
		}

		public void OnLoad(string key, Action<bool, Exception> onLoad, SoundInfo info, Exception ex)
		{
			if (ex == null)
			{
				onLoad?.Invoke(false, ex);
				return;
			}
			Entry entry;
			lock (m_Cache)
			{
				if (m_Cache.TryGetValue(key, out entry))
				{
					if (entry.Global || entry.Count > 0)
					{
						entry.Info = info;
						onLoad?.Invoke(true, null);
						return;
					}
				}
			}
			onLoad?.Invoke(false, null);
		}


	}
}
