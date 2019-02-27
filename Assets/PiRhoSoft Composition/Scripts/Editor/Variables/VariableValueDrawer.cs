using PiRhoSoft.CompositionEngine;
using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PiRhoSoft.CompositionEditor
{
	public class VariableValueDrawer
	{
		public static VariableValue Draw(Rect position, GUIContent label, VariableValue value, VariableDefinition definition)
		{
			position = EditorGUI.PrefixLabel(position, label);

			var type = definition.Type != VariableType.Empty ? definition.Type : value.Type;

			switch (type)
			{
				case VariableType.Empty: return value;
				case VariableType.Boolean: return DrawBoolean(position, value, definition);
				case VariableType.Integer: return DrawInteger(position, value, definition);
				case VariableType.Number: return DrawNumber(position, value, definition);
				case VariableType.String: return DrawString(position, value, definition);
				case VariableType.Object: return DrawObject(position, value, definition);
				case VariableType.Null: return value;
				default: return value;
			}
		}

		private static VariableValue DrawBoolean(Rect rect, VariableValue value, VariableDefinition definition)
		{
			var boolean = EditorGUI.ToggleLeft(rect, GUIContent.none, value.Boolean);
			return VariableValue.Create(boolean);
		}

		private static VariableValue DrawInteger(Rect rect, VariableValue value, VariableDefinition definition)
		{
			var integer = value.Integer;

			if (definition.UseRangeConstraint)
				integer = EditorGUI.IntSlider(rect, GUIContent.none, integer, (int)definition.MinimumConstraint, (int)definition.MaximumConstraint);
			else
				integer = EditorGUI.IntField(rect, GUIContent.none, integer);

			return VariableValue.Create(integer);
		}

		private static VariableValue DrawNumber(Rect rect, VariableValue value, VariableDefinition definition)
		{
			var number = value.Number;

			if (definition.UseRangeConstraint)
				number = EditorGUI.Slider(rect, GUIContent.none, number, definition.MinimumConstraint, definition.MaximumConstraint);
			else
				number = EditorGUI.FloatField(rect, GUIContent.none, number);

			return VariableValue.Create(number);
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
	}
}
