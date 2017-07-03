using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ConfigFileReader
{
	private const string CONFIG_FILE_NAME = "config.json";

	private ConfigFile config;

	public ConfigFileReader()
	{
		config = LoadConfigFile();
	}

	private static ConfigFile LoadConfigFile()
	{
		return JsonUtility.FromJson<ConfigFile>(File.ReadAllText(Path.Combine(Application.streamingAssetsPath, CONFIG_FILE_NAME)));
	}

	public GithubItem[] GetItems()
	{
		return (config != null) ? config.entries : null;
	}

	public void AddItem(GithubItem item, bool shouldAlsoSave = false)
	{
		List<GithubItem> items = new List<GithubItem>(config.entries);
		items.Add(item);
		config.entries = items.ToArray();

		if (shouldAlsoSave)
		{
			SaveConfigFile();
		}
	}

	public void DeleteItem(GithubItem item, bool shouldAlsoSave = false)
	{
		List<GithubItem> items = new List<GithubItem>(config.entries);
		items.Remove(item);
		config.entries = items.ToArray();

		if(shouldAlsoSave)
		{
			SaveConfigFile();
		}
	}

	public void MarkItemAsFavorite(GithubItem item, bool shouldAlsoSave = false)
	{
		item.isFavorite = !item.isFavorite;

		if(shouldAlsoSave)
		{
			SaveConfigFile();
		}
	}
		
	private void SaveConfigFile()
	{
		string json = JsonUtility.ToJson(config);
		File.WriteAllText(Path.Combine(Application.streamingAssetsPath, CONFIG_FILE_NAME), json);

		LoadConfigFile();
	}
}
