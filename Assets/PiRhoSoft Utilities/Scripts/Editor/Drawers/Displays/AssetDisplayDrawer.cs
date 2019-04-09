using System;
using System.Collections.Generic;
using PiRhoSoft.UtilityEngine;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PiRhoSoft.UtilityEditor
{
	[CustomPropertyDrawer(typeof(AssetDisplayAttribute))]
	public class AssetDisplayDrawer : PropertyDrawer
	{
		private const string _invalidTypeWarning = "(UASDDIT) Invalid type for AssetDisplay of field {0}: AssetDisplay can only be used with Object derived fields";

		private static readonly Label _editButton = new Label(Icon.BuiltIn(Icon.Edit), string.Empty, "Show this asset in the inspector");

		#region Static Property Interface

		public static float GetHeight(GUIContent label)
		{
			return EditorGUIUtility.singleLineHeight;
		}

		public static void Draw(GUIContent label, SerializedProperty property, Type assetType, bool showNoneOption, bool showEditButton, AssetLocation saveLocation, string defaultName)
		{
			var height = GetHeight(label);
			var rect = EditorGUILayout.GetControlRect(false, height);

			Draw(rect, label, property, assetType, showNoneOption, showEditButton, saveLocation, defaultName);
		}

		public static void Draw(Rect position, GUIContent label, SerializedProperty property, Type assetType, bool showNoneOption, bool showEditButton, AssetLocation saveLocation, string defaultName)
		{
			if (property.propertyType == SerializedPropertyType.ObjectReference)
			{
				property.objectReferenceValue = Draw(position, label, property.objectReferenceValue, assetType, showNoneOption, showEditButton, saveLocation, defaultName);
			}
			else
			{
				Debug.LogWarningFormat(_invalidTypeWarning, property.propertyPath);
				EditorGUI.PropertyField(position, property, label);
			}
		}

		#endregion

		#region Static Generic Interface

		public static AssetType Draw<AssetType>(GUIContent label, AssetType asset, bool showNoneOption, bool showEditButton, AssetLocation saveLocation, string defaultName) where AssetType : Object
		{
			return Draw(label, asset, typeof(AssetType), showNoneOption, showEditButton, saveLocation, defaultName) as AssetType;
		}

		public static AssetType Draw<AssetType>(Rect position, GUIContent label, AssetType asset, bool showNoneOption, bool showEditButton, AssetLocation saveLocation, string defaultName) where AssetType : Object
		{
			return Draw(position, label, asset, typeof(AssetType), showNoneOption, showEditButton, saveLocation, defaultName) as AssetType;
		}

		#endregion

		#region Static Type Interface

		public static Object Draw(GUIContent label, Object asset, Type assetType, bool showNoneOption, bool showEditButton, AssetLocation saveLocation, string defaultName)
		{
			var height = GetHeight(label);
			var rect = EditorGUILayout.GetControlRect(false, height);

			return Draw(rect, label, asset, showNoneOption, showEditButton, saveLocation, defaultName);
		}

		public static Object Draw(Rect position, GUIContent label, Object asset, Type assetType, bool showNoneOption, bool showEditButton, AssetLocation saveLocation, string defaultName)
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
			var creatable = saveLocation != AssetLocation.None && typeof(ScriptableObject).IsAssignableFrom(assetType);
			var list = AssetHelper.GetAssetList(assetType, showNoneOption, creatable);
			var index = list.GetIndex(asset);
			var thumbnail = asset != null ? AssetPreview.GetMiniThumbnail(asset) ?? AssetPreview.GetMiniTypeThumbnail(asset.GetType()) : null;
			var popupLabel = asset ? new GUIContent(asset.name, thumbnail) : new GUIContent("None");

			var selection = SelectionPopup.Draw(rect, popupLabel, new SelectionState { Tab = 0, Index = index }, list.Tree);

			if (selection.Tab == 0)
			{
				return list.GetAsset(selection.Index);
			}
			else if (selection.Tab == 1)
			{
				var type = list.GetType(selection.Index);
				return AssetHelper.Create(type, saveLocation, defaultName);
			}

			return asset;
		}

		#endregion

		#region Drawer Interface

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return GetHeight(label);
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var assetDisplay = attribute as AssetDisplayAttribute;
			var type = fieldInfo.FieldType;

			if (type.IsArray)
			{
				type = type.GetElementType();
			}
			else if (!typeof(Object).IsAssignableFrom(type))
			{
				var interfaces = type.GetInterfaces();

				foreach (var i in interfaces)
				{
					if (i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IList<>))
					{
						type = i.GetGenericArguments()[0];
						break;
					}

					if (i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IDictionary<,>))
					{
						type = i.GetGenericArguments()[1];
						break;
					}
				}
			}

			if (!typeof(Object).IsAssignableFrom(type))
				type = typeof(Object);

			Draw(position, label, property, type, assetDisplay.ShowNoneOption, assetDisplay.ShowEditButton, assetDisplay.SaveLocation, assetDisplay.DefaultName);
		}

		#endregion
	}
}
