using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace ILib.Audio
{
	public class SoundInfo
	{
		/// <summary>
		/// 音源データです
		/// </summary>
		public AudioClip Clip;
		/// <summary>
		/// 音量です
		/// </summary>
		public float Volume = 1f;
		/// <summary>
		/// ピッチです。
		/// </summary>
		public float Pitch = 1f;
		/// <summary>
		/// 再生管理用のIDです。
		/// </summary>
		public string ControlId;
		/// <summary>
		/// 再生管理用のIDに対する処理です。
		/// </summary>
		public IStartControl PlayControl;
	}
}
