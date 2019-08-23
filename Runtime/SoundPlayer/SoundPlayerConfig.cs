using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ILib.Audio
{
	public class SoundPlayerConfig
	{
		/// <summary>
		/// キャッシュにない場合に遅延ロードをした際に
		/// 再生リクエストを破棄するタイムアウト時間です。
		/// </summary>
		public float LoadTimeout = 2f;
		/// <summary>
		/// 音源が不足していた際に、一時的に音源を追加作成するか？
		/// </summary>
		public bool IsCreateIfNotEnough;
		/// <summary>
		/// 音源プールの最大数です。
		/// この上限を超えた音源がプールに返却される場合破棄されます。
		/// </summary>
		public int InitMaxPoolCount = 12;
		/// <summary>
		/// キャッシュになく遅延ロードを行った際に
		/// ロードしたデータをキャッシュするか？
		/// </summary>
		public bool IsAddCacheIfLoad;
		/// <summary>
		/// 再生リストを指定します
		/// 指定しない場合、共有再生リストを使用します。
		/// </summary>
		public PlayingList PlayingList;
		/// <summary>
		/// 利用するキャッシュの機能を選択します。
		/// </summary>
		public Cache Cache;
	}
}
