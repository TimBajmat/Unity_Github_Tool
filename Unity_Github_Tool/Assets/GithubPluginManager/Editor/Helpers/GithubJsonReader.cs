using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class GithubJsonReader {

	private const string JSON_FILE_PATH = "config.json";

	private GithubList list;

	public GithubJsonReader()
	{
		ReadList();
	}

	public GithubList GetList()
	{
		return list;
	}

	public GithubEntry GetEntry(int index)
	{
		return list.entries[index];
	}

	public void ChangeEntryAt(GithubEntry entry, int index)
	{
		list.entries[index] = entry;
	}

	public void AddEntry(GithubEntry entry)
	{
		List<GithubEntry> entries = new List<GithubEntry>(list.entries);
		entries.Add(entry);
		list.entries = entries.ToArray();
	}

	public void ReadList()
	{
		string json = File.ReadAllText(Path.Combine(Application.streamingAssetsPath, JSON_FILE_PATH));
		Debug.Log(json);
		list = JsonUtility.FromJson<GithubList>(json);
		Debug.Log(list.entries.Length);
	}

	public void SaveList()
	{
		string json = JsonUtility.ToJson(list);
		File.WriteAllText(Path.Combine(Application.streamingAssetsPath, JSON_FILE_PATH), json);

		ReadList();
	}

}
