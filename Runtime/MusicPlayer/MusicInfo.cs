using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace ILib.Audio
{

	public class MusicInfo 
	{
		public AudioClip Clip;
		public AudioMixerGroup Group;
		public float Volume = 1f;
		public float Pitch = 1f;

		int m_RefCount;
		public event System.Action OnUnused;

		public void AddRef()
		{
			m_RefCount++;
		}

		public void RemoveRef()
		{
			m_RefCount--;
			if (m_RefCount == 0)
			{
				OnUnused?.Invoke();
			}
		}

	}

}
