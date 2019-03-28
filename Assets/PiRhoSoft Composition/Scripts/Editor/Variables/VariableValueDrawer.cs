using PiRhoSoft.CompositionEngine;
using PiRhoSoft.UtilityEditor;
using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PiRhoSoft.CompositionEditor
{
	public class VariableValueDrawer
	{
		private static readonly IconButton _addButton = new IconButton(IconButton.Add, "Add an item to the list");
		private static readonly IconButton _removeButton = new IconButton(IconButton.Remove, "Remove this item from the list");

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
				case VariableType.Enum: return EditorGUI.GetPropertyHeight(SerializedPropertyType.Enum, GUIContent.none);
				case VariableType.Object: return EditorGUI.GetPropertyHeight(SerializedPropertyType.ObjectReference, GUIContent.none);
				case VariableType.Store: return EditorGUIUtility.singleLineHeight;
				case VariableType.List:
				{
					var height = EditorGUIUtility.singleLineHeight;

					var itemDefinition = definition.Constraint is ListVariableConstraint listConstraint
						? VariableDefinition.Create(string.Empty, listConstraint.ItemType, listConstraint.ItemConstraint)
						: VariableDefinition.Create(string.Empty, VariableType.Empty);

					for (var i = 0; i < value.List.Count; i++)
					{
						if (i != 0)
							height += EditorGUIUtility.standardVerticalSpacing;

						var item = value.List.GetVariable(i);
						height += GetHeight(item, itemDefinition);
					}

					return height;
				}
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
				case VariableType.Enum: return DrawEnum(position, value, definition);
				case VariableType.Object: return DrawObject(position, value, definition);
				case VariableType.Store: return DrawStore(position, value, definition);
				case VariableType.List: return DrawList(position, value, definition);
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

			if (definition.Constraint is IntVariableConstraint intConstraint)
				integer = EditorGUI.IntSlider(rect, GUIContent.none, integer, intConstraint.Minimum, intConstraint.Maximum);
			else
				integer = EditorGUI.IntField(rect, GUIContent.none, integer);

			return VariableValue.Create(integer);
		}

		private static VariableValue DrawFloat(Rect rect, VariableValue value, VariableDefinition definition)
		{
			var number = value.Float;

			if (definition.Constraint is FloatVariableConstraint floatConstraint)
				number = EditorGUI.Slider(rect, GUIContent.none, number, floatConstraint.Minimum, floatConstraint.Maximum);
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

			if (definition.Constraint is StringVariableConstraint stringConstraint && stringConstraint.Values.Length > 0)
			{
				var index = Array.IndexOf(stringConstraint.Values, s);
				index = EditorGUI.Popup(rect, index, stringConstraint.Values);
				s = index >= 0 ? stringConstraint.Values[index] : string.Empty;
			}
			else
			{
				s = EditorGUI.TextField(rect, GUIContent.none, s);
			}

			return s == null ? VariableValue.Create(string.Empty) : VariableValue.Create(s);
		}

		private static VariableValue DrawEnum(Rect rect, VariableValue value, VariableDefinition definition)
		{
			// value can have type Empty if the definition doesn't define the enum type

			if (value.Type == VariableType.Empty)
			{
				EditorGUI.LabelField(rect, "Enum type not specified");
				return value;
			}

			var e = EditorGUI.EnumPopup(rect, GUIContent.none, value.Enum);
			return VariableValue.Create(e);
		}

		private static VariableValue DrawObject(Rect rect, VariableValue value, VariableDefinition definition)
		{
			var objectType = (definition.Constraint is ObjectVariableConstraint objectConstraint ? objectConstraint.Type : null) ?? typeof(Object);
			var unityObject = EditorGUI.ObjectField(rect, GUIContent.none, value.Object, objectType, true);

			return VariableValue.Create(unityObject);
		}

		private static VariableValue DrawStore(Rect rect, VariableValue value, VariableDefinition definition)
		{
			return value;
		}

		private static VariableValue DrawList(Rect rect, VariableValue value, VariableDefinition definition)
		{
			var itemDefinition = definition.Constraint is ListVariableConstraint listConstraint
				? VariableDefinition.Create(string.Empty, listConstraint.ItemType, listConstraint.ItemConstraint)
				: VariableDefinition.Create("", VariableType.Empty);

			var remove = -1;

			for (var i = 0; i < value.List.Count; i++)
			{
				if (i != 0)
					RectHelper.TakeVerticalSpace(ref rect);

				var item = value.List.GetVariable(i);
				var height = GetHeight(item, itemDefinition);
				var itemRect = RectHelper.TakeHeight(ref rect, height);
				var removeRect = RectHelper.TakeTrailingIcon(ref itemRect);

				item = Draw(itemRect, GUIContent.none, item, itemDefinition);
				value.List.SetVariable(i, item);

				if (GUI.Button(removeRect, _removeButton.Content, GUIStyle.none))
					remove = i;
			}

			var addRect = RectHelper.TakeTrailingIcon(ref rect);

			if (GUI.Button(addRect, _addButton.Content, GUIStyle.none))
				value.List.AddVariable(itemDefinition.Generate(null));

			if (remove >= 0)
				value.List.RemoveVariable(remove);

			return value;
		}
	}
}
