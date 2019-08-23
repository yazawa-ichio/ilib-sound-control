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
		/// 同一パラメーターの場合、上書せず音声を継続させます
		/// </summary>
		public bool IsOverrideEqualParam;

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
