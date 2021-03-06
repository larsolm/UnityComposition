﻿using PiRhoSoft.CompositionEngine;
using PiRhoSoft.UtilityEditor;
using System.Collections;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace PiRhoSoft.CompositionEditor
{
	public class VariableSetControl : ObjectControl<VariableSet>
	{
		private readonly static Label _refreshButton = new Label(Icon.BuiltIn(Icon.Refresh), "", "Re-compute this variable based on the schema initializer");
		private readonly static GUIContent _emptyLabel = new GUIContent("Add definitions to the corresponding Schema to populate this list");

		private SerializedProperty _property;
		private ObjectListControl _list = new ObjectListControl();
		private VariableSet _variables;
		private VariablesProxy _proxy;

		public override void Setup(VariableSet target, SerializedProperty property, FieldInfo fieldInfo, PropertyAttribute attribute)
		{
			_property = property;
			_variables = target;
			_proxy = new VariablesProxy { List = target };

			if (property.serializedObject.targetObject is ISchemaOwner owner)
				owner.SetupSchema();

			if (_variables.Owner != null && _variables.NeedsUpdate)
			{
				_variables.Update();
				EditorUtility.SetDirty(property.serializedObject.targetObject);
			}

			_list.Setup(_proxy)
				.MakeDrawable(DrawVariable)
				.MakeCustomHeight(GetVariableHeight)
				.MakeCollapsable(property.isExpanded, OnCollapse)
				.MakeEmptyLabel(_emptyLabel);
		}

		private void OnCollapse(bool visible)
		{
			_property.isExpanded = visible;
		}

		public override float GetHeight(GUIContent label)
		{
			return _list.GetHeight();
		}

		public override void Draw(Rect position, GUIContent label)
		{
			_list.Draw(position, label);
		}

		private float GetVariableHeight(int index)
		{
			var name = _variables.GetVariableName(index);
			var value = _variables.GetVariableValue(index);
			var definition = _variables.Schema != null && index < _variables.Schema.Count ? _variables.Schema[index].Definition : ValueDefinition.Create(VariableType.Empty);

			return VariableValueDrawer.GetHeight(value, definition, true);
		}

		private void DrawVariable(Rect rect, IList list, int index)
		{
			var name = _variables.GetVariableName(index);
			var value = _variables.GetVariableValue(index);
			var definition = _variables.Schema != null && index < _variables.Schema.Count ? _variables.Schema[index].Definition : ValueDefinition.Create(VariableType.Empty);

			using (var changes = new EditorGUI.ChangeCheckScope())
			{
				if (_variables.Owner != null)
				{
					var variableRect = new Rect(rect.x, rect.y, rect.width - EditorGUIUtility.singleLineHeight, rect.height);
					var buttonRect = new Rect(variableRect.xMax, rect.y, EditorGUIUtility.singleLineHeight, rect.height);

					value = VariableValueDrawer.Draw(variableRect, new GUIContent(name), value, definition, true);

					if (_variables.Schema != null)
					{
						if (GUI.Button(buttonRect, _refreshButton.Content, GUIStyle.none))
							value = _variables.Schema[index].Definition.Generate(_variables.Owner);
					}
				}
				else
				{
					value = VariableValueDrawer.Draw(rect, new GUIContent(name), value, definition, true);
				}

				if (changes.changed)
					_variables.SetVariableValue(index, value);
			}
		}

		private class VariablesProxy : ListProxy
		{
			public VariableSet List;

			public override int Count => List.VariableCount;
			public override object this[int index] { get => List.GetVariableValue(index); set => List.SetVariableValue(index, (VariableValue)value); }
		}
	}

	[CustomPropertyDrawer(typeof(VariableSet))]
	public class VariableSetDrawer : PropertyDrawer<VariableSetControl>
	{
	}
}
