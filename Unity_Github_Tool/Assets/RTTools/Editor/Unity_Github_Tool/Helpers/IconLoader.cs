using UnityEngine;

namespace Unity_Github_Tool.Helpers
{
	public static class IconLoader
	{
		private static Texture[] textures;
		private static string[] paths = 
        {
			"Icons/Refresh",
			"Icons/Plus",
			"Icons/Github",
			"Icons/Download",
			"Icons/Star",
			"Icons/Delete"
	    };

        /// <summary>
        /// Gets the textures.
        /// </summary>
        /// <returns>The textures.</returns>
		public static Texture[] GetTextures()
		{
			textures = new Texture[paths.Length];
			for (int i = 0; i < paths.Length; i++)
			{
				textures[i] = GetIcon(paths[i]);
			}

			return textures;
		}

        /// <summary>
        /// Gets the icon.
        /// </summary>
        /// <returns>The icon.</returns>
        /// <param name="path">Path.</param>
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

