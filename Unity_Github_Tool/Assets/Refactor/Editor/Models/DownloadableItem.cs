using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;

[System.Serializable]
public abstract class DownloadableItem
{
	public bool isDownloading;

	protected WWW getInfoCall;
	protected WWW getDownloadItem;

	protected string infoUrl;
	public string downloadUrl;

	protected float progress;

	public AnimBool showProgressBar;

	public float GetDownloadProgress()
	{
		return progress;
	}

	public virtual void GetDownloadUrl()
	{
		getInfoCall = GetWWW(infoUrl);
		AddCallback(DownloadUrlCallback);
	}

	public virtual void GetDownloadItem()
	{
		showProgressBar.target = true;
		getDownloadItem = GetWWW(downloadUrl);
		AddCallback(DownloadItemCallback);
	}

	protected WWW GetWWW(string url)
	{
		return new WWW(url);
	}

	protected virtual void DownloadItemCallback()
	{
		if (getDownloadItem.isDone)
		{
			showProgressBar.target = false;
			getDownloadItem.Dispose();
			RemoveCallback(DownloadItemCallback);
		}
		else
		{
			progress = getDownloadItem.progress;
		}
	}

	protected virtual void DownloadUrlCallback()
	{
		if (getInfoCall.isDone)
		{
			getInfoCall.Dispose();
			RemoveCallback(DownloadUrlCallback);
		}
	}

	protected void RemoveCallback(EditorApplication.CallbackFunction callback)
	{
		EditorApplication.update -= callback;
	}

	protected void AddCallback(EditorApplication.CallbackFunction callback)
	{
		EditorApplication.update += callback;
	}
}
