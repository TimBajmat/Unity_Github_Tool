using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace RTTools.Models
{
	[System.Serializable]
	public class GithubItem : DownloadableItem
	{
		private const string GITHUB_API_URL = "https://api.github.com/repos/";
		private const string GITHUB_API_URL_PREFIX = "/releases/latest";

		public string repositoryOwnerName;
		public string repositoryName;
		public string lastUpdatedAt;

		public string lastKnownReleaseName;
		public string lastKnownReleaseDate;

		public GithubItem(string repositoryOwnerName, string repositoryName, string lastUpdatedAt, bool markedAsFavorite)
		{
			this.repositoryOwnerName = repositoryOwnerName;
			this.repositoryName = repositoryName;
			this.lastUpdatedAt = lastUpdatedAt;
			isFavorite = markedAsFavorite;
		}

        /// <summary>
        /// Gets the download URL.
        /// </summary>
		public override void GetDownloadUrl()
		{
			infoUrl = GITHUB_API_URL + repositoryOwnerName + "/" + repositoryName + GITHUB_API_URL_PREFIX;
			base.GetDownloadUrl();
		}

        /// <summary>
        /// Gets the download item.
        /// </summary>
		public override void GetDownloadItem()
		{
			if (string.IsNullOrEmpty(downloadUrl))
			{
				throw new UnityException("Download URL was null!");
			}

			base.GetDownloadItem();
		}

		/// <summary>
		/// Downloads the item and opens it when it is in the project.
		/// </summary>
		protected override void DownloadItemCallback()
		{
			if (getDownloadItem.isDone)
			{
				string filePath = Path.Combine(EditorPrefs.GetString("pathToSavePackages"), repositoryName + ".unitypackage");
				File.WriteAllBytes(filePath, getDownloadItem.bytes);

				AssetDatabase.Refresh();

				System.Diagnostics.Process.Start(filePath);
			}

			base.DownloadItemCallback();
		}

		/// <summary>
		/// Downloads information from the URL.
		/// </summary>
		protected override void DownloadUrlCallback()
		{
			if (getInfoCall.isDone)
			{
				try
				{
					GithubJsonReleaseAsset release = JsonUtility.FromJson<GithubJsonRelease>(getInfoCall.text).assets.First(x => x.name.Contains(".unitypackage"));
					lastKnownReleaseName = release.name;
					lastKnownReleaseDate = release.created_at;
					downloadUrl = release.browser_download_url;
				}
				catch
				{
					Debug.LogError("Package: " + this.repositoryName + ", is not responding." +
						" Please check if there are no typos in the repository names");
				}
			}

			base.DownloadUrlCallback();
		}
	}

	[System.Serializable]
	public class ConfigFile
	{
		public GithubItem[] entries;
	}
}

