﻿using System.Collections;
using PiRhoSoft.CompositionEngine;
using PiRhoSoft.UtilityEditor;
using UnityEditor;
using UnityEngine;

namespace PiRhoSoft.CompositionEditor
{
	[CustomEditor(typeof(VariableSchema))]
	public class VariableSchemaEditor : Editor
	{
		private readonly static GUIContent _label = new GUIContent("Definitions", "The variables available to objects using this schema");
		private readonly static Label _addDefinitionButton = new Label(Icon.BuiltIn(Icon.CustomAdd), "", "Add a Definition to the Schema");
		private readonly static Label _removeDefinitionButton = new Label(Icon.BuiltIn(Icon.Remove), "", "Remove this Definition from the Schema");
		private readonly static GUIContent _addDefinitionLabel = new GUIContent("Add Definition");
		private readonly static GUIContent _emptyLabel = new GUIContent("This Schema is empty");

		private VariableSchema _schema;
		private DefinitionsProxy _proxy;
		private SerializedProperty _definitions;

		private ObjectListControl _list = new ObjectListControl();
		private CreateVariablePopup _createPopup = new CreateVariablePopup();

		void OnEnable()
		{
			_schema = target as VariableSchema;
			_proxy = new DefinitionsProxy { Schema = _schema };
			_definitions = serializedObject.FindProperty("_definitions._items");

			_createPopup.Setup(_addDefinitionLabel, PopupCreate, PopupValidate);

			_list.Setup(_proxy)
				.MakeDrawable(DrawDefinition)
				.MakeRemovable(_removeDefinitionButton, RemoveDefinition)
				.MakeReorderable()
				.MakeHeaderButton(_addDefinitionButton, _createPopup, Color.white)
				.MakeCustomHeight(GetDefinitionHeight)
				.MakeEmptyLabel(_emptyLabel);
		}
		
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			using (new UndoScope(_schema, false))
				_list.Draw(_label);
		}

		private void AddDefinition(string name, VariableType type)
		{
			using (new UndoScope(_schema, true))
				_schema.AddDefinition(name, type);
		}

		private void RemoveDefinition(IList list, int index)
		{
			_schema.RemoveDefinition(index);
		}

		private float GetDefinitionHeight(int index)
		{
			var property = _definitions.GetArrayElementAtIndex(index);
			return ValueDefinitionControl.GetHeight(_schema[index].Definition, _schema.InitializerType, _schema.Tags, property.isExpanded);
		}

		private void DrawDefinition(Rect rect, IList list, int index)
		{
			var property = _definitions.GetArrayElementAtIndex(index);
			var expanded = property.isExpanded;

			using (var changes = new EditorGUI.ChangeCheckScope())
			{
				var schema = _schema[index];
				var definition = ValueDefinitionControl.Draw(rect, new GUIContent(schema.Name), schema.Definition, _schema.InitializerType, _schema.Tags, true, ref expanded);

				if (changes.changed)
					_schema[index] = new VariableDefinition { Name = schema.Name, Definition = definition };
			}

			property.isExpanded = expanded;
		}

		private void PopupCreate()
		{
			AddDefinition(_createPopup.Name, _createPopup.Type);
		}

		private bool PopupValidate()
		{
			_createPopup.IsNameValid = !_schema.HasDefinition(_createPopup.Name) && !string.IsNullOrEmpty(_createPopup.Name);
			_createPopup.IsTypeValid = _createPopup.Type != VariableType.Empty;

			return _createPopup.IsNameValid && _createPopup.IsTypeValid;
		}

		private class DefinitionsProxy : ListProxy
		{
			public VariableSchema Schema;

			public override int Count => Schema.Count;
			public override object this[int index] { get => Schema[index]; set => Schema[index] = (VariableDefinition)value; }
		}

		private class CreateVariablePopup : CreateNamedPopup
		{
			public VariableType Type = VariableType.Empty;
			public bool IsTypeValid = true;

			protected override float GetContentHeight()
			{
				return base.GetContentHeight() + EditorGUIUtility.standardVerticalSpacing + EditorGUIUtility.singleLineHeight;
			}

			protected override bool DrawContent()
			{
				var create = base.DrawContent();

				using (new InvalidScope(!HasChanged || IsTypeValid))
					Type = (VariableType)EditorGUILayout.EnumPopup(Type);

				return create;
			}
		}
	}
}
