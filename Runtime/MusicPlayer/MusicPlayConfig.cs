using UnityEngine.Audio;

namespace ILib.Audio
{
	public struct MusicPlayConfig
	{
		/// <summary>
		/// フェードイン時間です
		/// </summary>
		public float FadeInTime;
		/// <summary>
		/// フェードアウト時間です
		/// </summary>
		public float FadeOutTime;
		/// <summary>
		/// ロード待ちを無視して遷移処理を走らせます
		/// </summary>
		public bool SkipLoadWait;
		/// <summary>
		/// ループを無効にします
		/// </summary>
		public bool NoLoop;
		/// <summary>
		/// 同一パラメーターの場合でも強制的に最初から再生を行います
		/// </summary>
		public bool IsForceRestartIfEqualParam;
		/// <summary>
		/// 基底のミキサーグループを上書きます
		/// </summary>
		public AudioMixerGroup Group;

		public static MusicPlayConfig Get(float time)
		{
			MusicPlayConfig config = new MusicPlayConfig();
			config.FadeInTime = time;
			config.FadeOutTime = time;
			config.SkipLoadWait = false;
			return config;
		}

	}
}