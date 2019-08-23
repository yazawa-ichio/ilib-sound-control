namespace ILib.Audio
{

	internal partial class MusicStack
	{
		public class Entry
		{
			public MusicRequest Main;
			// TODO: サブ音源対応
			//public List<MusicInfo> Sub;

			public void RemoveCache()
			{
				if (Main != null)
				{
					Main.Music?.RemoveRef();
					Main.Music = null;
				}
				/*
				if (Sub != null)
				{
					foreach (var e in Sub)
					{
						e.Music?.RemoveRef();
						e.Music = null;
					}
				}
				*/
			}
		}

	}
}
