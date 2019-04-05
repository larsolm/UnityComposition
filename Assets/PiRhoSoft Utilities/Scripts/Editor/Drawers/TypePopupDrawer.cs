using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PiRhoSoft.UtilityEditor
{
	public static class TypePopupDrawer
	{
		private static readonly IconButton DefaultTypeIcon = new IconButton("cs Script Icon");

		private static List<Type> _types; // This is here purely so that you don't have to pass a reference list

		public static float GetHeight()
		{
			return EditorGUIUtility.singleLineHeight;
		}

		public static void Draw<BaseType>(GUIContent label, Type type, bool creatable, Action<int, int> onSelected)
		{
			 Draw<BaseType>(label, type, creatable, ref _types, onSelected);
		}

		public static void Draw<BaseType>(Rect position, GUIContent label, Type type, bool creatable, Action<int, int> onSelected)
		{
			Draw<BaseType>(position, label, type, creatable, ref _types, onSelected);
		}

		public static void Draw<BaseType>(GUIContent label, Type type, bool creatable, ref List<Type> types, Action<int, int> onSelected)
		{
			var rect = EditorGUILayout.GetControlRect(false, GetHeight());
			Draw<BaseType>(rect, label, type, creatable, ref types, onSelected);
		}

		public static void Draw<BaseType>(Rect position, GUIContent label, Type type, bool creatable, ref List<Type> types, Action<int, int> onSelected)
		{
			var list = TypeHelper.GetTypeList<BaseType>(true, creatable);
			var index = list.GetIndex(type);

			types = list.Types;

			if (GUI.Button(position, type == null ? new GUIContent("None") : new GUIContent(type.Name, AssetPreview.GetMiniTypeThumbnail(type) ?? DefaultTypeIcon.Content.image)))
				SearchTreeControl.Show(position, new List<GUIContent[]> { list.Names }, new List<GUIContent> { new GUIContent(typeof(BaseType).Name) }, onSelected, index);
		}
	}
}
