namespace ILib.Audio
{
	public interface IMusicPlayer : IMusicPlayer<string>
	{
	}

	public interface IMusicPlayer<T> : System.IDisposable
	{
		/// <summary>
		/// Musicのスタック中は音声情報をキャッシュしておくか？
		/// </summary>
		bool IsCacheInfoInStack { get; set; }
		/// <summary>
		/// 音源の最大プール数
		/// </summary>
		int MaxPoolCount { get; set; }
		/// <summary>
		/// 現在のパラメーター
		/// </summary>
		T Current { get; }
		/// <summary>
		/// 音声を切り替えます。
		/// </summary>
		void Change(T prm, float time = 2f, bool clearStack = false);
		/// <summary>
		/// 音声を切り替えます。
		/// </summary>
		void Change(T prm, MusicPlayConfig config, bool clearStack = false);
		/// <summary>
		/// 現在の音声をスタックに積み
		/// 音声を切り替えます。
		/// </summary>
		void Push(T prm, float time = 2f);
		/// <summary>
		/// 現在の音声をスタックに積み
		/// 音声を切り替えます。
		/// </summary>
		void Push(T prm, MusicPlayConfig config);
		/// <summary>
		/// 現在の音声を停止し
		/// スタックの音声に切り替えます。
		/// startLastPositionを有効にするとスタックに積んだ時の再生位置で開始します
		/// </summary>
		void Pop(float time = 2f, bool startLastPosition = false);
		/// <summary>
		/// 現在の音声を停止し
		/// スタックの音声に切り替えます。
		/// startLastPositionを有効にするとスタックに積んだ時の再生位置で開始します
		/// </summary>
		void Pop(MusicPlayConfig config, bool startLastPosition = false);
		/// <summary>
		/// 音声を停止します
		/// </summary>
		/// <param name="time"></param>
		void Stop(float time = 2f);
		/// <summary>
		/// 音声を一時停止します。
		/// </summary>
		void Pause(float time = 0.3f);
		/// <summary>
		/// 音声を一時停止を解除します。
		/// </summary>
		void Resume(float time = 0.3f);
		/// <summary>
		/// スタックをすべてクリアします
		/// </summary>
		void ClearStack();
	}
}
