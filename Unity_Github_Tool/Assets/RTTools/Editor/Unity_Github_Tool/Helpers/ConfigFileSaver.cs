using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity_Github_Tool.Helpers;
using Unity_Github_Tool.Models;
using UnityEngine;

public static class ConfigFileSaver
{
	public static void SaveUpdatedTime(GithubItem item)
	{
		ConfigFile file = null; //todo: get this from json etc.



		if (file.entries != null)
		{
			for (int i = 0; i < file.entries.Length; i++)
			{
				if (file.entries[i].repositoryName == item.repositoryName && file.entries[i].repositoryOwnerName == item.repositoryOwnerName)
				{
					file.entries[i] = item;
				}
			}

			SaveConfigFile(file);
		}
	}

	private static void SaveConfigFile(ConfigFile file)
	{
		string json = JsonUtility.ToJson(file);
		File.WriteAllText(Path.Combine(Application.streamingAssetsPath, ""), json);
	}
}
