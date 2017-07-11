using UnityEngine;

namespace Unity_Github_Tool.UI.Elements
{
	public static class ColoredButton
	{
		/// <summary>
		/// Draws the button with the specified name and color.
		/// </summary>
		/// <param name="name">Name.</param>
		/// <param name="color">Color.</param>
		public static bool Draw(string name, Color color)
		{
			Color c = GUI.color;

			GUI.color = color;
			bool b = GUILayout.Button(name);
			GUI.color = c;

			return b;
		}

		/// <summary>
		/// Draws the button with the specified name, color and options.
		/// </summary>
		/// <param name="name">Name.</param>
		/// <param name="color">Color.</param>
		/// <param name="options">Options.</param>
		public static bool Draw(string name, Color color, params GUILayoutOption[] options)
		{
			Color c = GUI.color;

			GUI.color = color;
			bool b = GUILayout.Button(name, options);
			GUI.color = c;

			return b;
		}

		/// <summary>
		/// Draws the button with the specified image and color.
		/// </summary>
		/// <param name="image">Image.</param>
		/// <param name="color">Color.</param>
		public static bool Draw(Texture image, Color color)
		{
			Color c = GUI.color;

			GUI.color = color;
			bool b = GUILayout.Button(image);
			GUI.color = c;

			return b;
		}

		/// <summary>
		/// Draws the button with the specified image, color and options.
		/// </summary>
		/// <param name="image">Image.</param>
		/// <param name="color">Color.</param>
		/// <param name="options">Options.</param>
		public static bool Draw(Texture image, Color color, params GUILayoutOption[] options)
		{
			Color c = GUI.color;

			GUI.color = color;
			bool b = GUILayout.Button(image, options);
			GUI.color = c;

			return b;
		}
	}
}

