using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ILib.Audio
{
#if !ILIB_AUDIO_DISABLE_TOOL_MENU
	[CreateAssetMenu(menuName = "ILib/Audio/SoundData")]
#endif
	public class SoundData : ScriptableObject
	{
		[Header("音源")]
		public AudioClip Clip;
		[Header("音量")]
		public float Volume = 1f;
		[Header("ピッチ")]
		public float Pitch = 1f;
		[Header("再生管理用のID")]
		public string ControlId;
		[Header("再生管理の方式")]
		public StartControl.Type ControlType;
		public float ControlParam1;
		public float ControlParam2;

		public SoundInfo CreateInfo()
		{
			var info = new SoundInfo();
			info.ControlId = string.IsNullOrEmpty(ControlId) ? name : ControlId;
			info.Clip = Clip;
			info.Pitch = Pitch;
			info.Volume = Mathf.Clamp01(Volume);
			if (!string.IsNullOrEmpty(info.ControlId))
			{
				info.PlayControl = StartControl.Create(ControlType, ControlParam1, ControlParam2);
			}
			return info;
		}

	}
#if UNITY_EDITOR
	[CustomEditor(typeof(SoundData), true)]
	public class SoundDataInspector : Editor
	{
		SoundData m_Taret;
		private void OnEnable()
		{
			m_Taret = target as SoundData;
		}

		public override void OnInspectorGUI()
		{
			serializedObject.UpdateIfRequiredOrScript();
			using (new EditorGUI.DisabledScope(true))
			{
				EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Script"), true);
			}
			EditorGUILayout.PropertyField(serializedObject.FindProperty("Clip"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("Volume"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("Pitch"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("ControlId"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("ControlType"));

			EditorGUILayout.LabelField($"【{m_Taret.ControlType}】", GetTypeName(m_Taret.ControlType));

			switch (m_Taret.ControlType)
			{
				case StartControl.Type.LimitCount:
					EditorGUILayout.PropertyField(serializedObject.FindProperty("ControlParam1"), new GUIContent("最大個数(整数になります)"));
					break;
				case StartControl.Type.LimitInterval:
					EditorGUILayout.PropertyField(serializedObject.FindProperty("ControlParam1"), new GUIContent("再生間隔"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("ControlParam2"), new GUIContent("再生待ち状態で待機する時間"));
					break;
				case StartControl.Type.LateArrivalPriority:
					EditorGUILayout.PropertyField(serializedObject.FindProperty("ControlParam1"), new GUIContent("停止する音源のフェード時間"));
					break;
			}

			serializedObject.ApplyModifiedProperties();
		}

		string GetTypeName(StartControl.Type type)
		{
			switch (type)
			{
				case StartControl.Type.None:
					return "なし";
				case StartControl.Type.LimitCount:
					return "再生個数を制限";
				case StartControl.Type.LimitInterval:
					return "再生間隔を制限";
				case StartControl.Type.FirstComeFirstServed:
					return "先勝ち";
				case StartControl.Type.LateArrivalPriority:
					return "後勝ち";
			}
			return "不明";
		}

	}

#endif

}
