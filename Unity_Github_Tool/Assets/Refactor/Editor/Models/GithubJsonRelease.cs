using System;

[Serializable]
public class GithubJsonRelease
{
	public string created_at;
	public string tag_name;

	public GithubJsonReleaseAsset[] assets;
}

[Serializable]
public class GithubJsonReleaseAsset
{
	public string name;
	public string browser_download_url;
	public string created_at;
}