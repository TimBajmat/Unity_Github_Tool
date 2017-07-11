using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using Unity_Github_Tool.Models;

namespace Unity_Github_Tool.Helpers
{
	public class ConfigFileReader
	{
        private const string CONFIG_FILE_NAME = "config.json";

		private static string pathToJson;
		private readonly ConfigFile config;

		public ConfigFileReader()
		{
			pathToJson = EditorPrefs.GetString("pathToJson");
			config = LoadConfigFile();
		}

        /// <summary>
        /// Loads the config file at the given path.
        /// </summary>
        /// <returns>The config file.</returns>
		private static ConfigFile LoadConfigFile()
		{
			return JsonUtility.FromJson<ConfigFile>(File.ReadAllText(Path.Combine(pathToJson, CONFIG_FILE_NAME)));
		}

        /// <summary>
        /// Gets the items.
        /// </summary>
        /// <returns>The items.</returns>
		public GithubItem[] GetItems()
		{
			return (config != null) ? config.entries : null;
		}

		/// <summary>
		/// Adds the given item.
		/// </summary>
		/// <param name="item">GithubItem.</param>
		/// <param name="shouldAlsoSave">If set to <c>true</c> should also save.</param>
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

        /// <summary>
        /// Deletes the given item.
        /// </summary>
        /// <param name="item">GithubItem.</param>
        /// <param name="shouldAlsoSave">If set to <c>true</c> should also save.</param>
		public void DeleteItem(GithubItem item, bool shouldAlsoSave = false)
		{
			List<GithubItem> items = new List<GithubItem>(config.entries);
			items.Remove(item);
			config.entries = items.ToArray();

			if (shouldAlsoSave)
			{
				SaveConfigFile();
			}
		}

        /// <summary>
        /// Marks the given item as favorite.
        /// </summary>
        /// <param name="item">GithubItem.</param>
        /// <param name="shouldAlsoSave">If set to <c>true</c> should also save.</param>
		public void MarkItemAsFavorite(GithubItem item, bool shouldAlsoSave = false)
		{
			item.isFavorite = !item.isFavorite;

			if (shouldAlsoSave)
			{
				SaveConfigFile();
			}
		}

        /// <summary>
        /// Saves the config file.
        /// </summary>
		private void SaveConfigFile()
		{
			string json = JsonUtility.ToJson(config);
			File.WriteAllText(Path.Combine(Application.streamingAssetsPath, CONFIG_FILE_NAME), json);

			LoadConfigFile();
		}
	}
}

