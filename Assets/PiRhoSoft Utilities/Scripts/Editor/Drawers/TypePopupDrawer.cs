using System;
using UnityEditor;
using UnityEngine;

namespace PiRhoSoft.UtilityEditor
{
	public static class TypePopupDrawer
	{
		public static float GetHeight()
		{
			return EditorGUIUtility.singleLineHeight;
		}
		
		public static Type Draw<BaseType>(GUIContent label, Type type, bool creatable)
		{
			var height = GetHeight();
			var rect = EditorGUILayout.GetControlRect(false, height);

			return Draw<BaseType>(rect, label, type, creatable);
		}

		public static Type Draw<BaseType>(Rect position, GUIContent label, Type type, bool creatable)
		{
			var list = TypeHelper.GetTypeList<BaseType>(true, creatable);
			var index = list.GetIndex(type);

			index = EditorGUI.Popup(position, label, index, list.Names);

			return list.GetType(index);
		}
	}
}
