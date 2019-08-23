using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ILib.Audio
{
	public class MusicPlayerConfig
	{
		/// <summary>
		/// Musicのスタック中は音声情報をキャッシュしておくか？
		/// </summary>
		public bool IsCacheInfoInStack;
		/// <summary>
		/// 音源の最大プール数
		/// </summary>
		public int MaxPoolCount = 2;
	}
}
