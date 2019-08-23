using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace ILib.Audio
{
#if !ILIB_AUDIO_DISABLE_TOOL_MENU
	public static class ToolMenu 
	{
		[MenuItem("Tools/SoundControl/Import SoundManager")]
		static void ImportSoundManager()
		{
			var name = "ILib.SoundManager";
			var ext = ".unitypackage";
			var paths = AssetDatabase.FindAssets(name).Select(x => AssetDatabase.GUIDToAssetPath(x));
			foreach (var path in paths)
			{
				if (System.IO.Path.GetExtension(path) == ext)
				{
					AssetDatabase.ImportPackage(path, true);
					return;
				}
			}
		}
	}
#endif
}
