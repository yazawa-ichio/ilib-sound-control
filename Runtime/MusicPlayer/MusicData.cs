﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace ILib.Audio
{
#if !ILIB_AUDIO_DISABLE_TOOL_MENU
	[CreateAssetMenu(menuName = "ILib/Audio/MusicData")]
#endif
	public class MusicData : ScriptableObject
	{
		public AudioClip Clip;
		public float Volume = 1f;
		public float Pitch = 1f;

		public MusicInfo CreateMusic()
		{
			var music = new MusicInfo();
			music.Clip = Clip;
			music.Pitch = Pitch;
			music.Volume = Mathf.Clamp01(Volume);
			return music;
		}

	}
}