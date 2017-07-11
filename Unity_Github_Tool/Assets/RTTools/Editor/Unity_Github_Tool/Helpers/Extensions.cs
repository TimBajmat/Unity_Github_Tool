using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace RTTools.Helpers
{
	public static class Extensions
	{
		private static Type[] GetAllDerivedTypes(this AppDomain appDomain, Type aType)
		{
			List<Type> result = new List<Type>();
			Assembly[] assemblies = appDomain.GetAssemblies();

			foreach (Assembly assembly in assemblies)
			{
				Type[] types = assembly.GetTypes();
				foreach (Type type in types)
				{
					if (type.IsSubclassOf(aType))
					{
						result.Add(type);
					}
				}
			}
			return result.ToArray();
		}

		private static Rect GetEditorMainWindowPos()
		{
			Type containerWinType = AppDomain.CurrentDomain.GetAllDerivedTypes(typeof(ScriptableObject)).FirstOrDefault(t => t.Name == "ContainerWindow");
			FieldInfo showModeField = containerWinType.GetField("m_ShowMode", BindingFlags.NonPublic | BindingFlags.Instance);
			PropertyInfo positionProperty = containerWinType.GetProperty("position", BindingFlags.Public | BindingFlags.Instance);

			if (containerWinType == null)
			{
				throw new MissingMemberException("Can't find internal type ContainerWindow. Maybe something has changed inside Unity");
			}

			if (showModeField == null || positionProperty == null)
			{
				throw new MissingFieldException("Can't find internal fields 'm_ShowMode' or 'position'. Maybe something has changed inside Unity");
			}

			UnityEngine.Object[] windows = Resources.FindObjectsOfTypeAll(containerWinType);
			foreach (UnityEngine.Object win in windows)
			{
				int showmode = (int)showModeField.GetValue(win);
				if (showmode == 4) // main window = 4
				{
					Rect pos = (Rect)positionProperty.GetValue(win, null);
					return pos;
				}
			}
			throw new NotSupportedException("Can't find internal main window. Maybe something has changed inside Unity");
		}

		/// <summary>
		/// Extension method to center a custom window. If you make a window make sure you call this last, otherwise this won't work.
		/// </summary>
		/// <param name="win">Custom EditorWindow.</param>
		public static void CenterOnMainWindow(this EditorWindow win)
		{
			Rect main = GetEditorMainWindowPos();
			Rect pos = win.position;
			float w = (main.width - pos.width) * 0.5f;
			float h = (main.height - pos.height) * 0.5f;
			pos.x = main.x + w;
			pos.y = main.y + h;
			win.position = pos;
		}

	}
}