using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ILib.Audio
{
	public interface IStartControl
	{
		StartControl.Result Update(IPlayingList list, SoundInfo info);
	}
}
