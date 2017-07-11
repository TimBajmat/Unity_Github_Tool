using UnityEngine;
using UnityEditor;

namespace RTTools.Windows
{
	public static class Preferences
	{
        private static string pathToJson;
        private static string pathToSavePackages;
		private static bool prefsLoaded;

		[PreferenceItem("Github Tool")]
		public static void PreferenceGUI()
		{
			if (!prefsLoaded)
			{
				GetPreferences();
				prefsLoaded = true;
			}

			ShowPreferences();

			if (GUI.changed)
			{
				SetPreferences();
			}
		}

		private static void ShowPreferences()
		{
			pathToJson = EditorGUILayout.TextField("Path to JSON: ", pathToJson);
			pathToSavePackages = EditorGUILayout.TextField("Path to save packages: ", pathToSavePackages);
		}

		private static void GetPreferences()
		{
			pathToJson = EditorPrefs.GetString("pathToJson");
			pathToSavePackages = EditorPrefs.GetString("pathToSavePackages");
		}

		private static void SetPreferences()
		{
			EditorPrefs.SetString("pathToJson", pathToJson);
			EditorPrefs.SetString("pathToSavePackages", pathToSavePackages);
		}
	}

}

