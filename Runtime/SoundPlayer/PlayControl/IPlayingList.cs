namespace ILib.Audio
{
	public interface IPlayingList
	{
		int GetCount(string controlId);
		void StopAll(string controlId);
		float GetLastPlayStartTime(string controlId);
	}
}
