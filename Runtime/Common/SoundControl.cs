using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ILib.Audio
{
	/// <summary>
	/// オーディオソース等のを必要なゲームオブジェクトを確保し管理する用のクラスです。
	/// 使用しているプレイヤーが破棄されたときに自動的に削除されます。
	/// また、Release関数を呼ぶと生成したすべてのプールを強制的に破棄します。
	/// </summary>
	public static class SoundControl
	{
		static bool s_Initialized = false;
		static GameObject s_Instance = null;
		static PlayingList s_SharedPlayingList;
		public static PlayingList SharedPlayingList
		{
			get
			{
				if (s_SharedPlayingList == null)
				{
					s_SharedPlayingList = CreatePlayingList("SoundPlayer:Shared");
				}
				return s_SharedPlayingList;
			}
		}

		/// <summary>
		/// プールをのルートオブジェクトを初期化します。
		/// この処理は手動で実行しなくても自動で実行されます。
		/// </summary>
		public static void Initialize()
		{
			if (s_Initialized) return;
			s_Initialized = true;
			var obj = new GameObject("SoundControl");
			GameObject.DontDestroyOnLoad(obj);
			s_Instance = obj;
		}

		/// <summary>
		/// 管理しているオブジェクトを破棄します。
		/// </summary>
		public static void Release()
		{
			if (!s_Initialized) return;
			s_SharedPlayingList = null;
			GameObject.Destroy(s_Instance, 0.1f);
			s_Instance = null;
			s_Initialized = false;
		}

		public static GameObject CreateRoot(string name)
		{
			Initialize();
			GameObject obj = new GameObject(name);
			obj.transform.SetParent(s_Instance.transform);
			return obj;
		}

		/// <summary>
		/// 再生リストを作成します。
		/// デフォルトの共有リスト以外で、プレイヤーの再生リストを共有する際に利用します。
		/// </summary>
		public static PlayingList CreatePlayingList(string name)
		{
			return CreateRoot(name).AddComponent<PlayingList>();
		}

		/// <summary>
		/// サウンドプレイヤーを作成します。
		/// 設定情報がnullの場合は共有リストを使用します。
		/// </summary>
		public static ISoundPlayer<T> CreatePlayer<T>(ISoundProvider<T> provider, SoundPlayerConfig config = null)
		{
			return new SoundPlayerImpl<T>(provider, config);
		}

		/// <summary>
		/// サウンドプレイヤーを作成します。
		/// 設定情報がnullの場合は共有リストを使用します。
		/// </summary>
		public static ISoundPlayer CreatePlayer(ISoundProvider<string> provider, SoundPlayerConfig config = null)
		{
			return new SoundPlayerImpl(provider, config);
		}

		/// <summary>
		/// 音声リストを作成します。
		/// </summary>
		public static PlayingMusic CreatePlayingMusic(string name)
		{
			return CreateRoot(name).AddComponent<PlayingMusic>();
		}

		/// <summary>
		/// 音声プレイヤーを作成します。
		/// </summary>
		public static IMusicPlayer<T> CreatePlayer<T>(IMusicProvider<T> provider, MusicPlayerConfig config = null)
		{
			var list = CreatePlayingMusic(nameof(IMusicPlayer<T>) + ":" + provider);
			return new MusicPlayerImpl<T>(list, provider, config);
		}

		/// <summary>
		/// 音声プレイヤーを作成します。
		/// </summary>
		public static IMusicPlayer CreatePlayer(IMusicProvider provider, MusicPlayerConfig config = null)
		{
			var list = CreatePlayingMusic(nameof(IMusicPlayer) + ":" + provider);
			return new MusicPlayerImpl(list, provider, config);
		}

	}
}
