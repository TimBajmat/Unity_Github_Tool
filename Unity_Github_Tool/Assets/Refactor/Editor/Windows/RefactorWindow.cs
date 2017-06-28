using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;
using System;

public class RefactorWindow : EditorWindow
{
	private const string WINDOW_TITLE = "RT-Tools";

	private ConfigFileReader configReader;
	private GithubItem[] items;

	private Texture refreshIcon;
	private Texture download;
	private Texture addIcon;
	private Texture github;
	private Texture star;
	private Texture cancel;

	private string repoOwner = "";
	private string repoName = "";
	private bool favorite = false;

	bool showPosition = true;
	bool showPosition2 = true;

	private AnimBool ShowInputField;
	private GUISkin skin;

	[MenuItem("Triple/GitHub %l")]
	private static void ShowWindow()
	{
		RefactorWindow window = (RefactorWindow)EditorWindow.GetWindow(typeof(RefactorWindow), false, WINDOW_TITLE);
		window.minSize = new Vector2(400, 400);
		window.titleContent = new GUIContent(WINDOW_TITLE);
		window.ShowAuxWindow();
		window.CenterOnMainWindow();
	}

	public void OnEnable()
	{
		cancel = Resources.Load<Texture>("cancel");
		star = Resources.Load<Texture>("star");
		download = Resources.Load<Texture>("download");
		github = Resources.Load<Texture>("Github");
		refreshIcon = Resources.Load<Texture>("refresh");
		addIcon = Resources.Load<Texture>("plus");
		skin = Resources.Load<GUISkin>("GUISkin");

		configReader = new ConfigFileReader();
		items = configReader.GetItems();

		ShowInputField = new AnimBool(false);
		ShowInputField.target = false;
	}

	private void OnGUI()
	{
		ShowEntries();
		Repaint();
	}
		
	private void ShowEntries()
	{
		ToolBar();

		foreach (GithubItem item in items)
		{
			if(item.markedAsFavorite)
			{
				if(showPosition){
					DrawEntrie(item);
				}
			}
			else 
			{
				if(showPosition2){
					DrawEntrie(item);
				}
			}
		}
	}
		
	// Replace the label with a picture we got from the internet
	private void DrawEntrie(GithubItem item)
	{
		EditorGUILayout.BeginVertical("Box");
		EditorGUILayout.BeginHorizontal();

		GUILayout.Label(github);

		EditorGUILayout.BeginVertical();
		EditorGUILayout.LabelField(item.repositoryOwnerName + " - " + item.repositoryName);
		EditorGUILayout.LabelField("Package updated: " + DateTime.Parse(item.lastUpdatedAt, null, System.Globalization.DateTimeStyles.RoundtripKind));
		EditorGUILayout.EndVertical();

		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button(github, GUILayout.Width(22), GUILayout.Height(22)))
		{
			Help.BrowseURL("https://github.com/" + item.repositoryOwnerName + "/" + item.repositoryName);
		}
		if (GUILayout.Button((string.IsNullOrEmpty(item.downloadUrl)) ? refreshIcon : download , GUILayout.Width(22), GUILayout.Height(22)))
		{
			if (string.IsNullOrEmpty(item.downloadUrl))
			{
				item.GetDownloadUrl();
			}
			else
			{
				item.GetDownloadItem();
			}
		}

		GUI.color = item.markedAsFavorite ? Color.yellow : Color.white;
		if(GUILayout.Button(star, GUILayout.Width(22), GUILayout.Height(22)))
		{
			item.markedAsFavorite = !item.markedAsFavorite;
			configReader.SaveConfigFile();
		}
		GUI.color = Color.white;

		GUI.color = Color.red;
		if(GUILayout.Button(cancel, GUILayout.Width(22), GUILayout.Height(22)))
		{
			if (item.markedAsFavorite &&
				EditorUtility.DisplayDialog("Deleting package: " + item.repositoryName,
					"This package is marked as favorite, are you sure you want to delete this package?" 
					 , "Delete", "Cancel"))
			{
				configReader.DeleteItem(item);
				configReader.SaveConfigFile();
			}
			else
			{
				configReader.DeleteItem(item);
				configReader.SaveConfigFile();
			}
		}
		GUI.color = Color.white;


		GUILayout.EndHorizontal();

		EditorGUILayout.EndHorizontal();

		if (EditorGUILayout.BeginFadeGroup(item.showProgressBar.faded))
		{
			EditorGUI.indentLevel++;
			Rect r = EditorGUILayout.BeginVertical();
			EditorGUI.ProgressBar(r, item.GetDownloadProgress(), "Downloading..");
			GUILayout.Space(16);
			EditorGUILayout.EndVertical();
			EditorGUI.indentLevel--;
		}
		EditorGUILayout.EndFadeGroup();
		EditorGUILayout.EndVertical();
	
	}
		
	private void ToolBar()
	{
		Rect backgroundRect = EditorGUILayout.BeginHorizontal();

		if(Event.current.type == EventType.Repaint)
		{
			EditorStyles.toolbar.Draw(backgroundRect, false, true, true, false);
		}

		EditorGUILayout.BeginHorizontal();
		GUILayout.Label("Github");
		GUILayout.FlexibleSpace();

		if (GUILayout.Button(addIcon, EditorStyles.toolbarButton))
		{
			//ShowInputField.target = true;
		}
		if (GUILayout.Button(refreshIcon, EditorStyles.toolbarButton))
		{
			foreach (DownloadableItem item in items)
			{
				item.GetDownloadUrl();
			}
		}
			
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		if(GUILayout.Button("Favorites"))
		{
			showPosition = true;
			showPosition2 = false;
		}

		if(GUILayout.Button("Other"))
		{
			showPosition = false;
			showPosition2 = true;
		}

		if(GUILayout.Button("All"))
		{
			showPosition = true;
			showPosition2 = true;
		}
		GUILayout.EndHorizontal();

		if (EditorGUILayout.BeginFadeGroup(ShowInputField.faded))
		{
			EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
			EditorGUILayout.BeginVertical();

			NewItemInput();

			EditorGUILayout.EndVertical();
			EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
		}
		EditorGUILayout.EndFadeGroup();
	}
		
	public void NewItemInput()
	{
		repoOwner = EditorGUILayout.TextField("Repo Owner: ", repoOwner);
		repoName = EditorGUILayout.TextField("Repo Name: ", repoName);
		favorite = GUILayout.Toggle(favorite, "Favorite");

		GUILayout.BeginHorizontal();
		if(GUILayout.Button("Save new Item") && repoOwner != "" && repoName != "")
		{
			configReader.AddItem(new GithubItem(repoOwner, repoName, "0001-01-01T00:00:00Z", favorite), true);
			ShowInputField.target = false;

			repoOwner = "";
			repoName = "";
			favorite = false;

			Repaint();
		}
		if(GUILayout.Button("Cancel"))
		{
			ShowInputField.target = false;

			repoOwner = "";
			repoName = "";
			favorite = false;
		}
		GUILayout.EndHorizontal();
	}
}
