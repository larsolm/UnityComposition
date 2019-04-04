using System;
using System.Collections.Generic;
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

			if (GUI.Button(position, type == null ? "None" : type.Name))
				SearchTreeControl.Show(position, new List<GUIContent[]> { list.Names }, new List<GUIContent> { new GUIContent(typeof(BaseType).Name) }, index);

			if (SearchTreeControl.Selection >= 0)
				return list.GetType(SearchTreeControl.Selection);

			return list.GetType(index);
		}
	}
}
