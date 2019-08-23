namespace ILib.Audio
{
	public interface IPlayingList
	{
		int GetCount(string controlId);
		void StopAll(string controlId);
		System.DateTime GetLastPlayStartTime(string controlId);
	}
}
