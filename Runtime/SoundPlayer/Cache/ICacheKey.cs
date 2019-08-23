namespace ILib.Audio
{
	/// <summary>
	/// キャッシュ用のキーへの変換機能を提供します
	/// </summary>
	public interface ICacheKey
	{
		/// <summary>
		/// キャッシュ用のキーを取得
		/// </summary>
		string GetCacheKey();
	}

}
