using System.Collections;
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
		private readonly static IconButton _addDefinitionButton = new IconButton(IconButton.CustomAdd, "Add a Definition to the Schema");
		private readonly static IconButton _removeDefinitionButton = new IconButton(IconButton.Remove, "Remove this Definition from the Schema");
		private readonly static GUIContent _addDefinitionLabel = new GUIContent("Add Definition");
		private readonly static GUIContent _emptyLabel = new GUIContent("This Schema is empty");

		private VariableSchema _schema;
		private DefinitionsProxy _proxy;

		private ObjectListControl _list = new ObjectListControl();

		void OnEnable()
		{
			_schema = target as VariableSchema;
			_proxy = new DefinitionsProxy { Schema = _schema };

			_list.Setup(_proxy)
				.MakeDrawable(DrawDefinition)
				.MakeRemovable(_removeDefinitionButton, RemoveDefinition)
				.MakeCollapsable("VariableSchema." + _schema.name + ".IsOpen")
				.MakeReorderable()
				.MakeHeaderButton(_addDefinitionButton, new AddPopup(new AddVariableContent(this), _addDefinitionLabel), Color.white)
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
			return ValueDefinitionControl.GetHeight(_schema[index].Definition, _schema.InitializerType, _schema.Tags);
		}

		private void DrawDefinition(Rect rect, IList list, int index)
		{
			using (var changes = new EditorGUI.ChangeCheckScope())
			{
				var schema = _schema[index];
				var definition = ValueDefinitionControl.Draw(rect, new GUIContent(schema.Name), schema.Definition, _schema.InitializerType, _schema.Tags, true);

				if (changes.changed)
					_schema[index] = new VariableDefinition { Name = schema.Name, Definition = definition };
			}
		}

		private class DefinitionsProxy : ListProxy
		{
			public VariableSchema Schema;

			public override int Count => Schema.Count;
			public override object this[int index] { get => Schema[index]; set => Schema[index] = (VariableDefinition)value; }
		}

		private class AddVariableContent : AddNamedItemContent
		{
			private VariableSchemaEditor _editor;
			private VariableType _type = VariableType.Empty;
			private bool _typeValid = true;

			public AddVariableContent(VariableSchemaEditor editor)
			{
				_editor = editor;
			}

			protected override float GetHeight_()
			{
				return EditorGUIUtility.singleLineHeight;
			}

			protected override bool Draw_(bool clean)
			{
				using (new InvalidScope(clean || _typeValid))
					_type = (VariableType)EditorGUILayout.EnumPopup(_type);

				return false;
			}

			protected override bool Validate_()
			{
				_typeValid = _type != VariableType.Empty;
				return _typeValid;
			}

			protected override void Add_(string name)
			{
				_editor.AddDefinition(name, _type);
			}

			protected override bool IsNameInUse(string name)
			{
				return _editor._schema.HasDefinition(name);
			}
		}
	}
}
