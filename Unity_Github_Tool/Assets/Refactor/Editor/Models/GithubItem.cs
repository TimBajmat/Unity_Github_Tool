using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class GithubItem : DownloadableItem
{
	private const string GITHUB_API_URL = "https://api.github.com/repos/";
	private const string GITHUB_API_URL_PREFIX = "/releases/latest";

	public string repositoryOwnerName;
	public string repositoryName;
	public string lastUpdatedAt;

	public bool markedAsFavorite;

	public string lastKnownReleaseName;
	public string lastKnownReleaseDate;

	public GithubItem(string repositoryOwnerName, string repositoryName, string lastUpdatedAt, bool markedAsFavorite)
	{
		this.repositoryOwnerName = repositoryOwnerName;
		this.repositoryName = repositoryName;
		this.lastUpdatedAt = lastUpdatedAt;
		this.markedAsFavorite = markedAsFavorite;
	}

	public override void GetDownloadUrl()
	{
		infoUrl = GITHUB_API_URL + repositoryOwnerName + "/" + repositoryName + GITHUB_API_URL_PREFIX;
		base.GetDownloadUrl();
	}

	public override void GetDownloadItem()
	{
		if (string.IsNullOrEmpty(downloadUrl))
		{
			throw new UnityException("Download URL was null!");
		}

		base.GetDownloadItem();
	}

	protected override void DownloadItemCallback()
	{
		if (getDownloadItem.isDone)
		{
			string filePath = Path.Combine(Application.streamingAssetsPath, repositoryName + ".unitypackage");
			File.WriteAllBytes(filePath, getDownloadItem.bytes);

			AssetDatabase.Refresh();

			System.Diagnostics.Process.Start(filePath);
		}

		base.DownloadItemCallback();
	}

	protected override void DownloadUrlCallback()
	{
		if (getInfoCall.isDone)
		{
			
			GithubJsonReleaseAsset release = JsonUtility.FromJson<GithubJsonRelease>(getInfoCall.text).assets.Where((x) => x.name.Contains(".unitypackage")).First();			
			lastKnownReleaseName = release.name;
			lastKnownReleaseDate = release.created_at;
			downloadUrl = release.browser_download_url;
		}

		base.DownloadUrlCallback();
	}
}

[System.Serializable]
public class ConfigFile
{
	public GithubItem[] entries;
}
