using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ILib.Audio
{

	internal partial class MusicStack
	{
		
		Stack<Entry> m_Entries = new Stack<Entry>();

		public bool IsCacheInfoInStack { get; set; }

		public object Current
		{
			get
			{
				var cur = Peek();
				if (cur != null && cur.Main != null)
				{
					return cur.Main.Param;
				}
				return default;
			}
		}

		public MusicInfo GetInfo(object prm)
		{
			foreach (var e in m_Entries)
			{
				var info = e.Main;
				if (info.Music != null && info.Param == prm)
				{
					return info.Music;
				}
			}
			return null;
		}

		public Entry Peek()
		{
			if (m_Entries.Count == 0) return null;
			return m_Entries.Peek();
		}

		public Entry Push(object prm)
		{
			if (!IsCacheInfoInStack)
			{
				var prev = Peek();
				prev?.RemoveCache();
			}
			Entry entry = new Entry();
			entry.Main = new MusicRequest();
			entry.Main.Param = prm;
			m_Entries.Push(entry);
			return entry;
		}

		public Entry Pop()
		{
			if (m_Entries.Count == 0) return null;
			var prev = m_Entries.Pop();
			prev.RemoveCache();
			return Peek();
		}

		public void Clear()
		{
			while (m_Entries.Count > 0)
			{
				var e = m_Entries.Pop();
				e.RemoveCache();
			}
		}

	}
}
