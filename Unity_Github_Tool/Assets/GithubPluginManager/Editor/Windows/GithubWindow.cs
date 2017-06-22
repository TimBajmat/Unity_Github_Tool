using UnityEngine;
using UnityEditor;
using System;
using UnityEditor.AnimatedValues;

public class GithubWindow : EditorWindow
{
	private const string GITHUB_REPOSITORY_LIST_PATH = "GithubRepositoryList";
	private const string NO_AVAILABLE_DATE_MSG = "Plugin has not been updated";
	private const string JSON_PATH = "Assets/GitHubEntry_Config.json";

	private const string GUI_SKIN = "GUISkin";
	private const string WINDOW_TITLE = "GitHub";
	private const float DEFAULT_SIZE = 160f;
	private GUISkin skin;

	private GithubJsonReader reader;
	private GithubNetworkManager manager;
	private GithubJsonReleaseAsset[] releaseAssets;

	private AnimBool ShowExtraFields;

	[MenuItem("Triple/GitHub %l")]
	public static void ShowWindow()
	{
		GithubWindow window = (GithubWindow)EditorWindow.GetWindow(typeof(GithubWindow), false, WINDOW_TITLE);
		window.minSize = new Vector2(400, 400);
		window.ShowAuxWindow();
		window.CenterOnMainWindow();
	}

	private void OnEnable()
	{
		reader = new GithubJsonReader();
		manager = new GithubNetworkManager();
		releaseAssets = new GithubJsonReleaseAsset[reader.GetList().entries.Length];

		ShowExtraFields = new AnimBool(true);
		ShowExtraFields.valueChanged.AddListener(Repaint);
		ShowExtraFields.target = false;
			
		skin = Resources.Load<GUISkin>(GUI_SKIN);
	}

	private void OnGUI()
	{
		if(GUILayout.Button("Check for packages", GUILayout.Height(50)))
		{
			for (int i = 0; i < reader.GetList().entries.Length; i++)
			{
				new GithubNetworkManager().GetEntryInfo(HandleGetInfoCompleted, reader.GetList().entries[i], i);
			}
		}
			
		ShowEntries();
		Repaint();
	}

	//TODO: Fix styling
	//TODO: Fix DateTime
	private void ShowEntries()
	{
		for (int i = 0; i < reader.GetList().entries.Length; i++)
		{
			GithubEntry entry = reader.GetList().entries[i];

			string buttonText = "";

			if (releaseAssets[i] != null)
			{
				buttonText = (entry.lastUpdatedAt == releaseAssets[i].created_at) ? "UpToDate" : "Download";
			}
			else
			{
				buttonText = "Loading";
			}

			EditorGUILayout.BeginVertical("Box");
			EditorGUILayout.BeginHorizontal();
		

			EditorGUILayout.BeginVertical();
			EditorGUILayout.LabelField(entry.repositoryOwnerName + " - " + entry.repositoryName);
			EditorGUILayout.LabelField("Package updated: " + DateTime.Parse(entry.lastUpdatedAt, null, System.Globalization.DateTimeStyles.RoundtripKind));
			EditorGUILayout.EndVertical();

			EditorGUILayout.BeginVertical();
			if(GUILayout.Button("GitHub")) 
			{
				Help.BrowseURL("https://github.com/" + entry.repositoryOwnerName + "/" + entry.repositoryName);
			}
			if (GUILayout.Button(buttonText))
			{
				ShowExtraFields.target = true;
				new GithubNetworkManager().StartNetworkCall(HandleDownloadCompleted, entry, i);
			}
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.EndVertical();

			if (EditorGUILayout.BeginFadeGroup(ShowExtraFields.faded))
			{
				EditorGUI.indentLevel++;
				Rect r = EditorGUILayout.BeginVertical();
				EditorGUI.ProgressBar(r, manager.GetDownloadProgress(), "Downloading..");
				GUILayout.Space(16);
				EditorGUILayout.EndVertical();
				EditorGUI.indentLevel--;
			}
			EditorGUILayout.EndFadeGroup();

			if(manager.GetDownloadProgress() >= 0.9)
			{
				ShowExtraFields.target = false;
			}

			EditorGUILayout.EndVertical();
		}
	}
		
	public void HandleDownloadCompleted(GithubEntry newEntry, int entryIndex)
	{
		reader.ChangeEntryAt(newEntry, entryIndex);
		reader.SaveList();
	}

	public void HandleGetInfoCompleted(GithubJsonReleaseAsset asset, int entryIndex)
	{
		releaseAssets[entryIndex] = asset;
	}
}
