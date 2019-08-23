using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace ILib.Audio
{
	/// <summary>
	/// サウンドプレイヤーです。
	/// 不要になった際はDisposeを実行してください。
	/// </summary>
	public interface ISoundPlayer : ISoundPlayer<string> { }

	/// <summary>
	/// サウンドプレイヤーです。
	/// 不要になった際はDisposeを実行してください。
	/// </summary>
	public interface ISoundPlayer<T> : IDisposable
	{
		/// <summary>
		/// キャッシュにない場合に遅延ロードをした際に
		/// 再生リクエストを破棄するタイムアウト時間です。
		/// </summary>
		float LoadTimeout { get; set; }

		/// <summary>
		/// 音源プールの最大数です。
		/// この上限を超えた音源がプールに返却される場合破棄されます。
		/// </summary>
		int MaxPoolCount { get; set; }

		/// <summary>
		/// 音源が不足していた際に、一時的に音源を追加作成するか？
		/// </summary>
		bool IsCreateIfNotEnough { get; set; }

		/// <summary>
		/// キャッシュになく遅延ロードを行った際に
		/// ロードしたデータをキャッシュするか？
		/// </summary>
		bool IsAddCacheIfLoad { get; set; }

		/// <summary>
		/// 音源のプールを指定分キャッシュします。
		/// 負の値の場合、MaxPoolCount分までキャッシュします。
		/// </summary>
		void ReservePool(int count = -1);

		/// <summary>
		/// サウンドを再生します。
		/// キャッシュにある場合は即時、そうでない場合はロードしたのち再生されます。
		/// フェードや音量は返り値を利用してください。
		/// </summary>
		IPlayingSoundContext Play(T prm);

		/// <summary>
		/// サウンド情報を直接渡して再生します。
		/// フェードや音量は返り値を利用してください。
		/// </summary>
		IPlayingSoundContext Play(SoundInfo info);

		/// <summary>
		/// サウンドを再生します。
		/// キャッシュにある場合は即時、そうでない場合はロードしたのち再生されます。
		/// 何も操作せず、サウンド情報のまま再生する場合に効率的です。
		/// </summary>
		void PlayOneShot(T prm);

		/// <summary>
		/// サウンド情報を直接渡して再生します。
		/// 何も操作せず、サウンド情報のまま再生する場合に効率的です。
		/// </summary>
		void PlayOneShot(SoundInfo info);

		/// <summary>
		/// キャッシュにサウンドを追加します。
		/// この関数でロードした場合、参照カウントを持たないためRemoveCache実行時に破棄されます。
		/// CreateCacheScopeで参照カウントがある場合は、参照カウントが0になるまで破棄されません。
		/// </summary>
		void AddCache(T prm, Action<bool, Exception> onLoad);

		/// <summary>
		/// キャッシュにサウンドを削除します。
		/// CreateCacheScopeで参照カウントがある場合は、参照カウントが0になるまで破棄されません。
		/// </summary>
		void RemoveCache(T prm);

		/// <summary>
		/// 参照カウント方式でサウンド情報をキャッシュします。
		/// 解放する際は戻り値のDisposeを実行して下さい。
		/// </summary>
		ICacheScope CreateCacheScope(T[] prms);

		/// <summary>
		/// forceフラグが無効の際は、CreateCacheScopeで参照カウントがある場合は破棄されません。
		/// 有効の際は参照カウントがあっても破棄されます。
		/// </summary>
		void ClearCache(bool force = false);

	}
}
