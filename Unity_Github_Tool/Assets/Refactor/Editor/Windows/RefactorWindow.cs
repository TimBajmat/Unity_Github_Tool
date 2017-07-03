using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;
using System;

public enum DisplayType
{
	Favorites,
	Others,
	All
}
	
public class RefactorWindow : EditorWindow
{
	private const string WINDOW_TITLE = "RT-Tools";

	private DisplayType displayType;
	private ConfigFileReader configReader;
	private GithubItem[] items;

	private Texture refreshIcon;
	private Texture download;
	private Texture addIcon;
	private Texture github;
	private Texture star;
	private Texture delete;

	private string repoOwner;
	private string repoName;
	private bool favorite;

	private AnimBool showInputField;
	private Vector2 scrollPos;

	[MenuItem("Triple/GitHub %l")]
	private static void ShowWindow()
	{
		RefactorWindow window = (RefactorWindow)EditorWindow.GetWindow(typeof(RefactorWindow), false, WINDOW_TITLE);
		window.minSize = new Vector2(400, 400);
		window.titleContent = new GUIContent(WINDOW_TITLE);
		window.Show();
		window.CenterOnMainWindow();
	}

	public void OnEnable()
	{
		refreshIcon = Resources.Load<Texture>("refresh");
		addIcon = Resources.Load<Texture>("plus");

		github = Resources.Load<Texture>("Github");
		download = Resources.Load<Texture>("download");
		star = Resources.Load<Texture>("star");
		delete = Resources.Load<Texture>("delete");

		configReader = new ConfigFileReader();
		UpdateItems();

		showInputField = new AnimBool(false);
		showInputField.target = false;
	}

	private void OnGUI()
	{
		ShowEntries();
		Repaint();
	}
		
	private void ShowEntries()
	{
		DrawToolBar();

		scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(position.width), GUILayout.Height(position.height - 40));

		foreach (GithubItem item in items)
		{
			if(item.isFavorite && displayType == DisplayType.Favorites || displayType == DisplayType.All)
			{
				DrawEntries(item);
			}
			else if(!item.isFavorite && displayType == DisplayType.Others || displayType == DisplayType.All) 
			{
				DrawEntries(item);
			}
		}
			
		EditorGUILayout.EndScrollView();
	}
    	
	// TODO: Replace the label with a picture we got from the internet
	private void DrawEntries(GithubItem item)
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
        	
		DrawFavoriteButton(item);
		DrawDeleteButton(item);

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

	private void DrawFavoriteButton(GithubItem item)
	{
		GUI.color = item.isFavorite ? Color.yellow : Color.white;
		if(GUILayout.Button(star, GUILayout.Width(22), GUILayout.Height(22)))
		{
			configReader.MarkItemAsFavorite(item, true);
		}
		GUI.color = Color.white;
	}

	private void DrawDeleteButton(GithubItem item)
	{
		GUI.color = Color.red;
		if(GUILayout.Button(delete, GUILayout.Width(22), GUILayout.Height(22)))
		{
			if (item.isFavorite &&
				EditorUtility.DisplayDialog(
					"Deleting package: " + item.repositoryName,
					"This package is marked as favorite, are you sure you want to delete this package?" 
					, "Delete", "Cancel"
				))
			{
				configReader.DeleteItem(item, true);
				UpdateItems();
			}
			else if(!item.isFavorite)
			{
				configReader.DeleteItem(item, true);
				UpdateItems();
			}
		}
		GUI.color = Color.white;
	}
	
	private void DrawToolBar()
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
			showInputField.target = true;
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

		DrawCatogoryButtons();

		if (EditorGUILayout.BeginFadeGroup(showInputField.faded))
		{
			EditorGUILayout.LabelField(string.Empty, GUI.skin.horizontalSlider);
			EditorGUILayout.BeginVertical();

			NewItemField();

			EditorGUILayout.EndVertical();
			EditorGUILayout.LabelField(string.Empty, GUI.skin.horizontalSlider);
		}
		EditorGUILayout.EndFadeGroup();
	}

	// TODO: Replace buttons with toggles to give better visual feedback
	public void DrawCatogoryButtons()
	{
		GUILayout.BeginHorizontal();
		if(GUILayout.Button("Favorites"))
		{
			displayType = DisplayType.Favorites;
		}

		if(GUILayout.Button("Other"))
		{
			displayType = DisplayType.Others;
		}

		if(GUILayout.Button("All"))
		{
			displayType = DisplayType.All;
		}
		GUILayout.EndHorizontal();
	}
    	
	// TODO: fix the time settings..
	private void NewItemField()
	{
		repoOwner = EditorGUILayout.TextField("Repository Owner: ", repoOwner);
		repoName = EditorGUILayout.TextField("Repository Name: ", repoName);
		favorite = GUILayout.Toggle(favorite, "Favorite");

		GUILayout.BeginHorizontal();
		if(GUILayout.Button("Save new Item") && repoOwner != string.Empty && repoName != string.Empty)
		{
			configReader.AddItem(new GithubItem(repoOwner, repoName, "0001-01-01T00:00:00Z", favorite), true);
			ResetValues();
			UpdateItems();
		}
		if(GUILayout.Button("Cancel"))
		{
			ResetValues();
		}
		GUILayout.EndHorizontal();
	}

	private void ResetValues()
	{	
		showInputField.target = false;
		repoOwner = string.Empty;
		repoName = string.Empty;
		favorite = false;
	}

	private void UpdateItems()
	{
		items = configReader.GetItems();
	}

	private void OnLostFocus()
	{
		ResetValues();
	}		
}
