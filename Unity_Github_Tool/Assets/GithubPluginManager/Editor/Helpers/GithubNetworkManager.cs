using System;
using System.IO;
using UnityEditor;
using UnityEngine;

public class GithubNetworkManager
{
	private const string GITHUB_API_URL = "https://api.github.com/repos/";
	private const string GITHUB_API_URL_PREFIX = "/releases/latest";
	private const string PACKAGE_EXTENSION = ".unitypackage";

	private Action<GithubEntry, int> actionCallback;
	private Action<GithubJsonReleaseAsset, int> jsonCallback;

	private GithubEntry entry;
	private int index;

	private string packageName;
	private float progress = 0;

	private GithubJsonReleaseAsset assetToDownload;

	private WWW releaseInfoCall;
	private WWW getPackageCall;

	private bool shouldDownLoadPackage;

	public void GetEntryInfo(Action<GithubJsonReleaseAsset, int> callback, GithubEntry entry, int index)
	{
		shouldDownLoadPackage = false;

		this.entry = entry;
		this.index = index;
		jsonCallback = callback;

		string url = GITHUB_API_URL + entry.repositoryOwnerName + "/" + entry.repositoryName + GITHUB_API_URL_PREFIX;
		releaseInfoCall = new WWW(url);

		EditorApplication.update += GetReleaseInfo;
	}

	public void StartNetworkCall(Action<GithubEntry, int> callback, GithubEntry entry, int index)
	{
		shouldDownLoadPackage = true;

		this.entry = entry;
		this.index = index;
		this.actionCallback = callback;
		Debug.Log("start download");

		string url = GITHUB_API_URL + entry.repositoryOwnerName + "/" + entry.repositoryName + GITHUB_API_URL_PREFIX;
		releaseInfoCall = new WWW(url);

		EditorApplication.update += GetReleaseInfo;
	}

	private void GetReleaseInfo()
	{
		if (releaseInfoCall.isDone)
		{
			Debug.Log("done with release info: " + releaseInfoCall.text);
			HandleReleaseInfoCallback(releaseInfoCall.text);
			StopNetworkCall();

			releaseInfoCall.Dispose();
		}
	}

	private void HandleReleaseInfoCallback(string json)
	{
		Debug.Log(json);
		GithubJsonRelease release = JsonUtility.FromJson<GithubJsonRelease>(json);
		Debug.Log("total assets = " + release.assets);

		foreach (GithubJsonReleaseAsset asset in release.assets)
		{
			if (asset.browser_download_url.Contains(PACKAGE_EXTENSION))
			{
				assetToDownload = asset;
				Debug.Log("gonna download: " + assetToDownload);
				break;
			}
		}

		if (shouldDownLoadPackage)
		{
			DownloadAndSaveUnityPackage(assetToDownload.browser_download_url, assetToDownload.name);
		}
		else
		{
			jsonCallback(assetToDownload, index);
		}

	}

	private void DownloadAndSaveUnityPackage(string url, string fileName)
	{
		packageName = fileName;
		Debug.Log("starting download");

		getPackageCall = new WWW(url);

		EditorApplication.update += GetUnityPackage;
	}

	private void GetUnityPackage()
	{
		if(!getPackageCall.isDone)
		{
			progress = getPackageCall.progress;
		}

		if (getPackageCall.isDone)
		{
			Debug.Log("Done");

			string filePath = Path.Combine(Application.streamingAssetsPath, packageName);
			File.WriteAllBytes(filePath, getPackageCall.bytes);

			AssetDatabase.Refresh();
			StopUnpacking();

			getPackageCall.Dispose();

			Debug.Log(assetToDownload.created_at);

			entry.lastUpdatedAt = assetToDownload.created_at;

			System.Diagnostics.Process.Start(filePath);
			actionCallback(entry, index);

			progress = 0;
		}
	}

	public float GetDownloadProgress()
	{
		return progress;
	}
		
	private void StopUnpacking()
	{
		EditorApplication.update -= GetUnityPackage;
	}

	private void StopNetworkCall()
	{
		EditorApplication.update -= GetReleaseInfo;
	}
}
