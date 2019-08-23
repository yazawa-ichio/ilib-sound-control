using System;

namespace ILib.Audio
{
	/// <summary>
	/// 再生中のSEのコントローラーです。
	/// キャッシュがなく遅延再生時でも再生開始時に設定が適応されます。
	/// 再生停止後は一切の操作が反映されなくなります。
	/// </summary>
	public interface IPlayingSoundContext : IDisposable
	{
		/// <summary>
		/// ローディング中もしくは再生待ち中です。
		/// </summary>
		bool IsLoading { get; }
		/// <summary>
		/// 有効かチェックします。
		/// ローディングはtrueを返します。
		/// </summary>
		bool IsValid { get; }
		/// <summary>
		/// ループフラグ
		/// デフォルト無効です
		/// </summary>
		bool Loop { get; set; }
		/// <summary>
		/// ポーズ状態
		/// </summary>
		bool Pause { get; set; }
		/// <summary>
		/// ピッチ
		/// SoundInfoのPitchに乗算した値が実際には使われます
		/// </summary>
		float Pitch { get; set; }
		/// <summary>
		/// 音量を設定します。
		/// フェード中の場合、フェードは停止します。
		/// </summary>
		float Volume { get; set; }
		/// <summary>
		/// 再生に失敗した場合のエラー情報です
		/// </summary>
		Exception Error { get; }
		/// <summary>
		/// 0音量からフェードインします。
		/// </summary>
		void FadeIn(float time);
		/// <summary>
		/// 現在の音量から指定の音量にフェードさせます。
		/// </summary>
		void Fade(float end, float time);
		/// <summary>
		/// 指定の音量から指定の音量にフェードさせます。
		/// </summary>
		void Fade(float start, float end, float time);
		/// <summary>
		/// フェードアウトを行います。
		/// 操作後、他のフェード関数は無効になります。
		/// </summary>
		/// <param name="time"></param>
		void FadeOut(float time);
		/// <summary>
		/// 再生を即時停止します。
		/// 短いループSEはLoopをfalseにする。
		/// 長いSEはFadeOutを推奨します。
		/// </summary>
		void Stop();
	}
}
