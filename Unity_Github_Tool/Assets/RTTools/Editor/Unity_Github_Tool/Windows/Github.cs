using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;
using System;
using RTTools.UI.Elements;
using RTTools.Models;
using RTTools.Helpers;

namespace RTTools.Windows
{
	public class Github : EditorWindow
	{
		private const string WINDOW_TITLE = "RT-Tools";
		private const int MINI_BUTTON_SIZE = 22;

		private DisplayType displayType;
		private IconIndex iconIndex;

		private ConfigFileReader configReader;
		private GithubItem[] items;
		private Texture[] icons;

		private string pathToPackage;
		private string repoOwner;
		private string repoName;
		private bool favorite;

		private AnimBool showInputField;
		private Vector2 scrollPos;

		[MenuItem("RTTools/GitHub")]
		private static void ShowWindow()
		{
			Github window = (Github)EditorWindow.GetWindow(typeof(Github), false);
			window.minSize = new Vector2(400, 400);
			window.titleContent = new GUIContent(WINDOW_TITLE);
			window.Show();
			window.CenterOnMainWindow();
		}

		public void OnEnable()
		{
			pathToPackage = EditorPrefs.GetString("pathToSavePackages");
			icons = IconLoader.GetTextures();

			try
			{
				configReader = new ConfigFileReader();
				UpdateItems();
			}
			catch
			{
				Debug.LogError("Could not find config file, please check te path in the preference window.");
			}

			showInputField = new AnimBool(false);
			showInputField.target = false;
		}

		private void OnGUI()
		{
			ShowEntries();
			Repaint();
		}

		/// <summary>
		/// Shows the entries.
		/// </summary>
		private void ShowEntries()
		{
			DrawToolBar();

			scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(position.width), GUILayout.Height(position.height - 40));

			foreach (GithubItem item in items)
			{
				if (item.isFavorite && displayType == DisplayType.Favorites || displayType == DisplayType.All)
				{
					DrawEntries(item);
				}
				else if (!item.isFavorite && displayType == DisplayType.Others || displayType == DisplayType.All)
				{
					DrawEntries(item);
				}
			}

			EditorGUILayout.EndScrollView();
		}

		/// <summary>
		/// Draws the entries.
		/// </summary>
		/// <param name="item">GithubItem.</param>
		private void DrawEntries(GithubItem item)
		{
			EditorGUILayout.BeginVertical("Box");
			EditorGUILayout.BeginHorizontal();

			GUILayout.Label(icons[(int)IconIndex.GitHub]);

			EditorGUILayout.BeginVertical();
			EditorGUILayout.LabelField(item.repositoryOwnerName + " - " + item.repositoryName);
			EditorGUILayout.LabelField("Package updated: " + DateTime.Parse(item.lastUpdatedAt, null, System.Globalization.DateTimeStyles.RoundtripKind));
			EditorGUILayout.EndVertical();

			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button(icons[(int)IconIndex.GitHub], GUILayout.Width(MINI_BUTTON_SIZE), GUILayout.Height(MINI_BUTTON_SIZE)))
			{
				Help.BrowseURL("https://github.com/" + item.repositoryOwnerName + "/" + item.repositoryName);
			}
			if (GUILayout.Button((string.IsNullOrEmpty(item.downloadUrl)) ? icons[(int)IconIndex.Refresh] : icons[(int)IconIndex.Download], GUILayout.Width(MINI_BUTTON_SIZE), GUILayout.Height(MINI_BUTTON_SIZE)))
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

		/// <summary>
		/// Draws the favorite button.
		/// </summary>
		/// <param name="item">GithubItem.</param>
		private void DrawFavoriteButton(GithubItem item)
		{
			if (ColoredButton.Draw(icons[(int)IconIndex.Star], item.isFavorite ? Color.yellow : Color.white, GUILayout.Width(MINI_BUTTON_SIZE), GUILayout.Height(MINI_BUTTON_SIZE)))
			{
				configReader.MarkItemAsFavorite(item, true);
			}
		}

		/// <summary>
		/// Draws the delete button and make a dialogwindow with the option to delete the entry or the entry and the package
		/// </summary>
		/// <param name="item">GithubItem</param>
		private void DrawDeleteButton(GithubItem item)
		{
			if (ColoredButton.Draw(icons[(int)IconIndex.Delete], Color.red, GUILayout.Width(MINI_BUTTON_SIZE), GUILayout.Height(MINI_BUTTON_SIZE)))
			{
				if (item.isFavorite)
				{
					int option = EditorUtility.DisplayDialogComplex(
						"Deleting package: " + item.repositoryName,
						"Are you sure you want to delete this package?",
						"Delete Entry",
						"Delete Entry and Package",
						"Cancel"
					);

					switch (option)
					{
						case 0:
							configReader.DeleteItem(item, true);
							UpdateItems();
							break;
						case 1:
							AssetDatabase.DeleteAsset(pathToPackage + item.repositoryName + ".unitypackage");
							configReader.DeleteItem(item, true);
							UpdateItems();
							break;
					}
				}
				else if (!item.isFavorite)
				{
					configReader.DeleteItem(item, true);
					UpdateItems();
				}
			}
		}

		/// <summary>
		/// Draws the tool bar.
		/// </summary>
		private void DrawToolBar()
		{
			GUILayout.BeginHorizontal(GUI.skin.FindStyle("Toolbar"));
			GUILayout.FlexibleSpace();
			if (GUILayout.Button(icons[(int)IconIndex.Plus], EditorStyles.toolbarButton))
			{
				showInputField.target = true;
			}
			if (GUILayout.Button(icons[(int)IconIndex.Refresh], EditorStyles.toolbarButton))
			{
				foreach (DownloadableItem item in items)
				{
					item.GetDownloadUrl();
				}
			}
			EditorGUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Favorites"))
			{
				displayType = DisplayType.Favorites;
			}

			if (GUILayout.Button("Other"))
			{
				displayType = DisplayType.Others;
			}

			if (GUILayout.Button("All"))
			{
				displayType = DisplayType.All;
			}
			GUILayout.EndHorizontal();


			if (EditorGUILayout.BeginFadeGroup(showInputField.faded))
			{
				EditorGUILayout.LabelField(string.Empty, GUI.skin.horizontalSlider);
				EditorGUILayout.BeginVertical();

				NewItemInput();

				EditorGUILayout.EndVertical();
				EditorGUILayout.LabelField(string.Empty, GUI.skin.horizontalSlider);
			}
			EditorGUILayout.EndFadeGroup();
		}

		// TODO: fix the time settings..
		private void NewItemInput()
		{
			repoOwner = EditorGUILayout.TextField("Repository Owner: ", repoOwner);
			repoName = EditorGUILayout.TextField("Repository Name: ", repoName);
			favorite = GUILayout.Toggle(favorite, "Favorite");

			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Save new Item") && repoOwner != string.Empty && repoName != string.Empty)
			{
				configReader.AddItem(new GithubItem(repoOwner, repoName, "0001-01-01T00:00:00Z", favorite), true);
				ResetValues();
				UpdateItems();
			}
			if (GUILayout.Button("Cancel"))
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

	public enum DisplayType
	{
		Favorites,
		Others,
		All,
		None
	}
}

