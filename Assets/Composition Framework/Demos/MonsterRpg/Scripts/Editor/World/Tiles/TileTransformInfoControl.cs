using OoT2D;
using PiRhoSoft.UtilityEditor;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace OoT2DEditor
{
	public class TileTransformInfoControl : PropertyControl
	{
		public static readonly Label _spriteContent = new Label(typeof(TileTransformInfo), nameof(TileTransformInfo.Sprite));
		public static readonly Label _flipHorizontalContent = new Label(typeof(TileTransformInfo), nameof(TileTransformInfo.FlipHorizontal));
		public static readonly Label _flipVerticalContent = new Label(typeof(TileTransformInfo), nameof(TileTransformInfo.FlipVertical));
		public static readonly Label _rotationContent = new Label(typeof(TileTransformInfo), nameof(TileTransformInfo.Rotation));
		public static readonly GUIContent[] _rotationsContent = new GUIContent[4] { new GUIContent("0 degrees"), new GUIContent("90 degrees"), new GUIContent("180 degrees"), new GUIContent("270 degrees") };

		public static float Height => EditorGUIUtility.singleLineHeight * 3 + RectHelper.VerticalSpace * 2;

		private SerializedProperty _spriteProperty;
		private SerializedProperty _rotationProperty;
		private SerializedProperty _horizontalProperty;
		private SerializedProperty _verticalProperty;

		public override void Setup(SerializedProperty property, FieldInfo fieldInfo, PropertyAttribute attribute)
		{
			_spriteProperty = property.FindPropertyRelative(nameof(TileTransformInfo.Sprite));
			_rotationProperty = property.FindPropertyRelative(nameof(TileTransformInfo.Rotation));
			_horizontalProperty = property.FindPropertyRelative(nameof(TileTransformInfo.FlipHorizontal));
			_verticalProperty = property.FindPropertyRelative(nameof(TileTransformInfo.FlipVertical));
		}

		public override float GetHeight(SerializedProperty property, GUIContent label)
		{
			return Height;
		}

		public override void Draw(Rect position, SerializedProperty property, GUIContent label)
		{
			var info = Draw(position, label, _spriteProperty.objectReferenceValue as Sprite, _rotationProperty.intValue, _horizontalProperty.boolValue, _verticalProperty.boolValue);

			_spriteProperty.objectReferenceValue = info.Sprite;
			_rotationProperty.intValue = info.Rotation;
			_horizontalProperty.boolValue = info.FlipHorizontal;
			_verticalProperty.boolValue = info.FlipVertical;
		}

		public static TileTransformInfo Draw(GUIContent label, TileTransformInfo info)
		{
			var position = EditorGUILayout.GetControlRect(label != null, Height);
			return Draw(position, label, info);
		}

		public static TileTransformInfo Draw(Rect position, GUIContent label, TileTransformInfo info)
		{
			return Draw(position, label, info.Sprite, info.Rotation, info.FlipHorizontal, info.FlipVertical);
		}

		private static TileTransformInfo Draw(Rect position, GUIContent label, Sprite sprite, int rotation, bool horizontal, bool vertical)
		{
			var rect =				EditorGUI.PrefixLabel(position, label);
			var spriteRect =		RectHelper.TakeWidth(ref rect, Height);
									RectHelper.TakeHorizontalSpace(ref rect);
			var rotationRect =		RectHelper.TakeLine(ref rect);
			var horizontalRect =	RectHelper.TakeLine(ref rect);
			var verticalRect =		RectHelper.TakeLine(ref rect);

			var info = new TileTransformInfo
			{
				Sprite = EditorGUI.ObjectField(spriteRect, sprite, typeof(Sprite), false) as Sprite,
				Rotation = EditorGUI.IntPopup(rotationRect, _rotationContent.Content, rotation, _rotationsContent, TileTransformInfo.Rotations),
				FlipHorizontal = EditorGUI.Toggle(horizontalRect, _flipHorizontalContent.Content, horizontal),
				FlipVertical = EditorGUI.Toggle(verticalRect, _flipVerticalContent.Content, vertical)
			};

			return info;
		}
	}

	[CustomPropertyDrawer(typeof(TileTransformInfo))]
	public class TileTransformInfoDrawer : PropertyDrawer<TileTransformInfoControl>
	{
	}
}
