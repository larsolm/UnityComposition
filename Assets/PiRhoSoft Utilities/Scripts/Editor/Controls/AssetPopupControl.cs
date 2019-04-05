using System;
using System.Collections.Generic;
using System.Reflection;
using PiRhoSoft.UtilityEngine;
using UnityEditor;
using UnityEngine;

namespace PiRhoSoft.UtilityEditor
{
	public class AssetPopupControl : PropertyControl
	{
		private const string _invalidTypeWarning = "(UCAPCIT) Invalid type for AssetPopup on field {0}: AssetPopup can only be applied to ScriptableObject fields";

		private static readonly IconButton _editButton = new IconButton(IconButton.Edit, "Edit this object");

		#region Static Interface

		public static float GetHeight()
		{
			return EditorGUIUtility.singleLineHeight;
		}

		public static void Draw<AssetType>(GUIContent label, AssetType asset, Action<int, int> onSelected, bool showNoneOption, bool showEditButton, bool showCreateOptions) where AssetType : ScriptableObject
		{
			var rect = EditorGUILayout.GetControlRect(false, GetHeight());
			Draw(rect, label, asset, onSelected, showNoneOption, showEditButton, showCreateOptions);
		}

		public static void Draw<AssetType>(Rect position, GUIContent label, AssetType asset, Action<int, int> onSelected, bool showNoneOption, bool showEditButton, bool showCreateOptions) where AssetType : ScriptableObject
		{
			Draw(position, label, asset, typeof(AssetType), onSelected, showNoneOption, showEditButton, showCreateOptions);
		}

		public static void Draw(GUIContent label, SerializedProperty property, Type type, Action<int, int> onSelected, bool showNoneOption, bool showEditButton, bool showCreateOptions)
		{
			var height = GetHeight();
			var rect = EditorGUILayout.GetControlRect(false, height);

			Draw(rect, label, property, type, onSelected, showNoneOption, showEditButton, showCreateOptions);
		}

		public static void Draw(Rect position, GUIContent label, SerializedProperty property, Type type, Action<int, int> onSelected, bool showNoneOption, bool showEditButton, bool showCreateOptions)
		{
			if (property.propertyType == SerializedPropertyType.ObjectReference && typeof(ScriptableObject).IsAssignableFrom(type))
			{
				Draw(position, label, property.objectReferenceValue as ScriptableObject, type, onSelected, showNoneOption, showEditButton, showCreateOptions);
			}
			else
			{
				Debug.LogWarningFormat(_invalidTypeWarning, property.propertyPath);
				EditorGUI.PropertyField(position, property, label);
			}
		}

		#endregion

		#region Drawer Interface

		private bool _showNone;
		private bool _showEdit;
		private bool _showCreate;
		private Type _assetType;
		private AssetList _assets;
		private ScriptableObject _asset;

		public override void Setup(SerializedProperty property, FieldInfo fieldInfo, PropertyAttribute attribute)
		{
			var assetPopup = attribute as AssetPopupAttribute;
			_showNone = assetPopup.ShowNone;
			_showEdit = assetPopup.ShowEdit;
			_showCreate = assetPopup.ShowCreate;
			_assetType = fieldInfo.FieldType;
			_assets = AssetHelper.GetAssetList(_assetType, _showNone, _showCreate);
			_asset = property.objectReferenceValue as ScriptableObject;
		}

		public override float GetHeight(SerializedProperty property, GUIContent label)
		{
			return GetHeight();
		}

		public override void Draw(Rect position, SerializedProperty property, GUIContent label)
		{
			Draw(position, label, property, _assetType, SetAsset, _showNone, _showEdit, _showCreate);

			if (_asset != property.objectReferenceValue)
			{
				property.objectReferenceValue = _asset;
				GUI.changed = true;
			}
		}

		private void SetAsset(int tab, int selection)
		{
			if (selection >= 0)
			{
				var list = AssetHelper.GetAssetList(_assetType, _showNone, _showCreate);

				if (tab == 0)
				{
					_asset = list.GetAsset(selection);
				}
				else if (tab == 1)
				{
					var type = list.GetType(selection);
					_asset = AssetHelper.CreateAssetWindow(type);
				}
			}
		}

		#endregion

		#region Drawing

		private static void Draw(Rect position, GUIContent label, ScriptableObject asset, Type type, Action<int, int> onSelected, bool showNoneOption, bool showEditButton, bool showCreateOptions)
		{
			if (showEditButton)
			{
				var editRect = RectHelper.TakeTrailingIcon(ref position);

				if (asset)
				{
					if (GUI.Button(editRect, _editButton.Content, GUIStyle.none))
						Selection.activeObject = asset;
				}
			}

			var rect = EditorGUI.PrefixLabel(position, label);
			var list = AssetHelper.GetAssetList(type, showNoneOption, showCreateOptions);
			var index = list.GetIndex(asset);

			if (GUI.Button(rect, asset ? new GUIContent(asset.name, AssetPreview.GetMiniThumbnail(asset) ?? AssetPreview.GetMiniTypeThumbnail(asset.GetType())) : new GUIContent("None")))
			{
				var trees = new List<GUIContent[]> { list.Names };
				var tabs = new List<GUIContent> { new GUIContent(type.Name) };

				if (showCreateOptions)
				{
					trees.Add(list.Types.Names);
					tabs.Add(new GUIContent("Create"));
				}

				SearchTreeControl.Show(rect, trees, tabs, onSelected, index, 0);
			}
		}

		#endregion
	}

	[CustomPropertyDrawer(typeof(AssetPopupAttribute))]
	public class AssetPopupDrawer : ControlDrawer<AssetPopupControl>
	{
	}
}
