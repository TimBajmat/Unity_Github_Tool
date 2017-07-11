using UnityEngine;

namespace RTTools.UI.Elements
{
	public static class ToggleButton
	{
		private static GUIStyle ToggleButtonStyleNormal;
		private static GUIStyle ToggleButtonStyleToggled;

		/// <summary>
		/// Draw the specified button with name and uses b to determine the state.
		/// </summary>
		/// <returns>The draw.</returns>
		/// <param name="name">Name.</param>
		/// <param name="b">If set to <c>true</c> b.</param>
		public static bool Draw(string name, bool b)
		{
			GetToggleStyle();
			return GUILayout.Button(name, b ? ToggleButtonStyleToggled : ToggleButtonStyleNormal);
		}

		/// <summary>
		/// Draw the specified button with name, options and uses b to determine the state.
		/// </summary>
		/// <returns>The draw.</returns>
		/// <param name="name">Name.</param>
		/// <param name="b">If set to <c>true</c> b.</param>
		/// <param name="options">Options.</param>
		public static bool Draw(string name, bool b, params GUILayoutOption[] options)
		{
			GetToggleStyle();
			return GUILayout.Button(name, b ? ToggleButtonStyleToggled : ToggleButtonStyleNormal, options);
		}

		/// <summary>
		/// Gets the toggle style.
		/// </summary>
		private static void GetToggleStyle()
		{
			if (ToggleButtonStyleNormal == null)
			{
				ToggleButtonStyleNormal = "Button";
				ToggleButtonStyleToggled = new GUIStyle(ToggleButtonStyleNormal);
				ToggleButtonStyleToggled.normal.background = ToggleButtonStyleToggled.active.background;
			}
		}
	}
}

