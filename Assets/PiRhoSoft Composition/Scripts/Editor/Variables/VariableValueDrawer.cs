using PiRhoSoft.CompositionEngine;
using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PiRhoSoft.CompositionEditor
{
	public class VariableValueDrawer
	{
		public static float GetHeight(VariableValue value, VariableDefinition definition)
		{
			var type = definition.Type != VariableType.Empty ? definition.Type : value.Type;

			switch (type)
			{
				case VariableType.Empty: return EditorGUIUtility.singleLineHeight;
				case VariableType.Bool: return EditorGUI.GetPropertyHeight(SerializedPropertyType.Boolean, GUIContent.none);
				case VariableType.Int: return EditorGUI.GetPropertyHeight(SerializedPropertyType.Integer, GUIContent.none);
				case VariableType.Float: return EditorGUI.GetPropertyHeight(SerializedPropertyType.Float, GUIContent.none);
				case VariableType.Int2: return EditorGUI.GetPropertyHeight(SerializedPropertyType.Vector2Int, GUIContent.none);
				case VariableType.Int3: return EditorGUI.GetPropertyHeight(SerializedPropertyType.Vector3Int, GUIContent.none);
				case VariableType.IntRect: return EditorGUI.GetPropertyHeight(SerializedPropertyType.RectInt, GUIContent.none);
				case VariableType.IntBounds: return EditorGUI.GetPropertyHeight(SerializedPropertyType.BoundsInt, GUIContent.none);
				case VariableType.Vector2: return EditorGUI.GetPropertyHeight(SerializedPropertyType.Vector2, GUIContent.none);
				case VariableType.Vector3: return EditorGUI.GetPropertyHeight(SerializedPropertyType.Vector3, GUIContent.none);
				case VariableType.Vector4: return EditorGUI.GetPropertyHeight(SerializedPropertyType.Vector4, GUIContent.none);
				case VariableType.Quaternion: return EditorGUI.GetPropertyHeight(SerializedPropertyType.Quaternion, GUIContent.none);
				case VariableType.Rect: return EditorGUI.GetPropertyHeight(SerializedPropertyType.Rect, GUIContent.none);
				case VariableType.Bounds: return EditorGUI.GetPropertyHeight(SerializedPropertyType.Bounds, GUIContent.none);
				case VariableType.Color: return EditorGUI.GetPropertyHeight(SerializedPropertyType.Color, GUIContent.none);
				case VariableType.String: return EditorGUI.GetPropertyHeight(SerializedPropertyType.String, GUIContent.none);
				case VariableType.Object: return EditorGUI.GetPropertyHeight(SerializedPropertyType.ObjectReference, GUIContent.none);
				case VariableType.Store: return EditorGUIUtility.singleLineHeight;
				case VariableType.Other: return EditorGUIUtility.singleLineHeight;
				default: return EditorGUIUtility.singleLineHeight;
			}
		}

		public static VariableValue Draw(GUIContent label, VariableValue value, VariableDefinition definition)
		{
			var height = GetHeight(value, definition);
			var rect = EditorGUILayout.GetControlRect(true, height);
			return Draw(rect, label, value, definition);
		}

		public static VariableValue Draw(Rect position, GUIContent label, VariableValue value, VariableDefinition definition)
		{
			position = EditorGUI.PrefixLabel(position, label);

			var type = definition.Type != VariableType.Empty ? definition.Type : value.Type;

			switch (type)
			{
				case VariableType.Empty: return value;
				case VariableType.Bool: return DrawBool(position, value, definition);
				case VariableType.Int: return DrawInt(position, value, definition);
				case VariableType.Float: return DrawFloat(position, value, definition);
				case VariableType.Int2: return DrawInt2(position, value, definition);
				case VariableType.Int3: return DrawInt3(position, value, definition);
				case VariableType.IntRect: return DrawIntRect(position, value, definition);
				case VariableType.IntBounds: return DrawIntBounds(position, value, definition);
				case VariableType.Vector2: return DrawVector2(position, value, definition);
				case VariableType.Vector3: return DrawVector3(position, value, definition);
				case VariableType.Vector4: return DrawVector4(position, value, definition);
				case VariableType.Quaternion: return DrawQuaternion(position, value, definition);
				case VariableType.Rect: return DrawRect(position, value, definition);
				case VariableType.Bounds: return DrawBounds(position, value, definition);
				case VariableType.Color: return DrawColor(position, value, definition);
				case VariableType.String: return DrawString(position, value, definition);
				case VariableType.Object: return DrawObject(position, value, definition);
				case VariableType.Store: return DrawStore(position, value, definition);
				case VariableType.Other: return DrawOther(position, value, definition);
				default: return value;
			}
		}

		private static VariableValue DrawBool(Rect rect, VariableValue value, VariableDefinition definition)
		{
			var boolean = EditorGUI.ToggleLeft(rect, GUIContent.none, value.Bool);
			return VariableValue.Create(boolean);
		}

		private static VariableValue DrawInt(Rect rect, VariableValue value, VariableDefinition definition)
		{
			var integer = value.Int;

			if (definition.UseRangeConstraint)
				integer = EditorGUI.IntSlider(rect, GUIContent.none, integer, (int)definition.MinimumConstraint, (int)definition.MaximumConstraint);
			else
				integer = EditorGUI.IntField(rect, GUIContent.none, integer);

			return VariableValue.Create(integer);
		}

		private static VariableValue DrawFloat(Rect rect, VariableValue value, VariableDefinition definition)
		{
			var number = value.Float;

			if (definition.UseRangeConstraint)
				number = EditorGUI.Slider(rect, GUIContent.none, number, definition.MinimumConstraint, definition.MaximumConstraint);
			else
				number = EditorGUI.FloatField(rect, GUIContent.none, number);

			return VariableValue.Create(number);
		}

		private static VariableValue DrawInt2(Rect rect, VariableValue value, VariableDefinition definition)
		{
			var result = EditorGUI.Vector2IntField(rect, GUIContent.none, value.Int2);
			return VariableValue.Create(result);
		}

		private static VariableValue DrawInt3(Rect rect, VariableValue value, VariableDefinition definition)
		{
			var result = EditorGUI.Vector3IntField(rect, GUIContent.none, value.Int3);
			return VariableValue.Create(result);
		}

		private static VariableValue DrawIntRect(Rect rect, VariableValue value, VariableDefinition definition)
		{
			var result = EditorGUI.RectIntField(rect, GUIContent.none, value.IntRect);
			return VariableValue.Create(result);
		}

		private static VariableValue DrawIntBounds(Rect rect, VariableValue value, VariableDefinition definition)
		{
			var result = EditorGUI.BoundsIntField(rect, GUIContent.none, value.IntBounds);
			return VariableValue.Create(result);
		}

		private static VariableValue DrawVector2(Rect rect, VariableValue value, VariableDefinition definition)
		{
			var result = EditorGUI.Vector2Field(rect, GUIContent.none, value.Vector2);
			return VariableValue.Create(result);
		}

		private static VariableValue DrawVector3(Rect rect, VariableValue value, VariableDefinition definition)
		{
			var result = EditorGUI.Vector3Field(rect, GUIContent.none, value.Vector3);
			return VariableValue.Create(result);
		}

		private static VariableValue DrawVector4(Rect rect, VariableValue value, VariableDefinition definition)
		{
			var result = EditorGUI.Vector4Field(rect, GUIContent.none, value.Vector4);
			return VariableValue.Create(result);
		}

		private static VariableValue DrawQuaternion(Rect rect, VariableValue value, VariableDefinition definition)
		{
			var result = EditorGUI.Vector3Field(rect, GUIContent.none, value.Quaternion.eulerAngles);
			return VariableValue.Create(Quaternion.Euler(result));
		}

		private static VariableValue DrawRect(Rect rect, VariableValue value, VariableDefinition definition)
		{
			var result = EditorGUI.RectField(rect, GUIContent.none, value.Rect);
			return VariableValue.Create(result);
		}

		private static VariableValue DrawBounds(Rect rect, VariableValue value, VariableDefinition definition)
		{
			var result = EditorGUI.BoundsField(rect, GUIContent.none, value.Bounds);
			return VariableValue.Create(result);
		}

		private static VariableValue DrawColor(Rect rect, VariableValue value, VariableDefinition definition)
		{
			var result = EditorGUI.ColorField(rect, GUIContent.none, value.Color);
			return VariableValue.Create(result);
		}

		private static VariableValue DrawString(Rect rect, VariableValue value, VariableDefinition definition)
		{
			var s = value.String;
			var values = !string.IsNullOrEmpty(definition.TypeConstraint) ? definition.TypeConstraint.Split(',') : null;

			if (values != null)
			{
				var index = Array.IndexOf(values, s);
				index = EditorGUI.Popup(rect, index, values);
				s = index >= 0 ? values[index] : "";
			}
			else
			{
				s = EditorGUI.TextField(rect, GUIContent.none, s);
			}

			return s == null ? VariableValue.Create(VariableType.String) : VariableValue.Create(s);
		}

		private static VariableValue DrawObject(Rect rect, VariableValue value, VariableDefinition definition)
		{
			var objectType = (!string.IsNullOrEmpty(definition.TypeConstraint) ? Type.GetType(definition.TypeConstraint) : null) ?? typeof(Object);
			var unityObject = EditorGUI.ObjectField(rect, GUIContent.none, value.Object, objectType, true);

			return unityObject == null ? VariableValue.Create(VariableType.Object) : VariableValue.Create(unityObject);
		}

		private static VariableValue DrawStore(Rect rect, VariableValue value, VariableDefinition definition)
		{
			var store = value.Store;

			if (store != null)
				EditorGUI.LabelField(rect, store.ToString());

			return value;
		}

		private static VariableValue DrawOther(Rect rect, VariableValue value, VariableDefinition definition)
		{
			if (value.Other != null)
				EditorGUI.LabelField(rect, value.Other.ToString());

			return value;
		}
	}
}
