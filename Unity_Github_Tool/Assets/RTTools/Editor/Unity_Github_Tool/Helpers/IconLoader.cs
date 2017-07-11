using UnityEngine;

namespace RTTools.Helpers
{
	public static class IconLoader
	{
		private static Texture[] textures;
		private static string[] paths = {
		"Icons/Refresh",
		"Icons/Plus",
		"Icons/Github",
		"Icons/Download",
		"Icons/Star",
		"Icons/Delete"
	};

		public static Texture[] GetTextures()
		{
			textures = new Texture[paths.Length];
			for (int i = 0; i < paths.Length; i++)
			{
				textures[i] = GetIcon(paths[i]);
			}

			return textures;
		}

		private static Texture GetIcon(string path)
		{
			return Resources.Load<Texture>(path);
		}
	}

	public enum IconIndex
	{
		Refresh,
		Plus,
		GitHub,
		Download,
		Star,
		Delete
	}
}

