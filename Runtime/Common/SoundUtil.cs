using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ILib.Audio
{
	public static class SoundUtil
	{
		public static float VolumeToDecibel(float volume, bool clamp = true)
		{
			if (clamp)
			{
				volume = Mathf.Clamp01(volume);
			}
			var ret = 20f * Mathf.Log10(volume);
			return Mathf.Clamp(ret, -80f, 20f);
		}

		public static float DecibelToVolume(float decibel, bool clamp = true)
		{
			var ret = Mathf.Pow(10f, decibel / 20f);
			if (clamp)
			{
				ret = Mathf.Clamp01(ret);
			}
			return ret;
		}
	}
}
