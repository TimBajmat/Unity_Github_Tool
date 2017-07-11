using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;

namespace Unity_Github_Tool.Models
{
	[System.Serializable]
	public abstract class DownloadableItem
	{
		protected WWW getInfoCall;
		protected WWW getDownloadItem;
		protected string infoUrl;
		protected float progress;

		public bool isFavorite;
		public string downloadUrl;
		public AnimBool showProgressBar;

        /// <summary>
        /// Gets the download progress.
        /// </summary>
        /// <returns>The download progress.</returns>
		public float GetDownloadProgress()
		{
			return progress;
		}

        /// <summary>
        /// Gets the download URL.
        /// </summary>
		public virtual void GetDownloadUrl()
		{
			getInfoCall = GetWWW(infoUrl);
			AddCallback(DownloadUrlCallback);
		}

        /// <summary>
        /// Gets the item from the url.
        /// </summary>
		public virtual void GetDownloadItem()
		{
			showProgressBar.target = true;
			getDownloadItem = GetWWW(downloadUrl);
			AddCallback(DownloadItemCallback);
		}

        /// <summary>
        /// Gets the www.
        /// </summary>
        /// <returns>The www.</returns>
        /// <param name="url">URL.</param>
		protected WWW GetWWW(string url)
		{
			return new WWW(url);
		}

        /// <summary>
        /// Downloads the item.
        /// </summary>
		protected virtual void DownloadItemCallback()
		{
			if (getDownloadItem.isDone)
			{
				showProgressBar.target = false;
				RemoveCallback(DownloadItemCallback);
			}
			else
			{
				progress = getDownloadItem.progress;
			}
		}

        /// <summary>
        /// Downloads information from the URL.
        /// </summary>
		protected virtual void DownloadUrlCallback()
		{
			if (getInfoCall.isDone)
			{
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
}

