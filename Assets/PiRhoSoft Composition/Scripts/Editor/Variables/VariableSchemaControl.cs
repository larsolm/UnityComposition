using System.Collections;
using System.Reflection;
using PiRhoSoft.CompositionEngine;
using PiRhoSoft.UtilityEditor;
using UnityEditor;
using UnityEngine;

namespace PiRhoSoft.CompositionEditor
{
	public class VariableSchemaControl : ObjectControl<VariableSchema>
	{
		private readonly static IconButton _addDefinitionButton = new IconButton(IconButton.CustomAdd, "Add a Definition to the Schema");
		private readonly static IconButton _removeDefinitionButton = new IconButton(IconButton.Remove, "Remove this Definition from the Schema");
		private readonly static GUIContent _addDefinitionLabel = new GUIContent("Add Definition");
		private readonly static GUIContent _emptyLabel = new GUIContent("This Schema is empty");

		public ObjectListControl List { get; private set; } = new ObjectListControl();

		private Object _owner;
		private VariableSchema _schema;
		private DefinitionsProxy _proxy;
		private VariableInitializerType _type;
		private string[] _availabilities;

		public override void Setup(VariableSchema target, SerializedProperty property, FieldInfo fieldInfo, PropertyAttribute attribute)
		{
			_owner = property.serializedObject.targetObject;
			_schema = target;
			_proxy = new DefinitionsProxy { Schema = target };

			List.Setup(_proxy)
				.MakeDrawable(DrawDefinition)
				.MakeRemovable(_removeDefinitionButton, RemoveDefinition)
				.MakeCollapsable(property.serializedObject.targetObject.GetType().Name + "." + property.propertyPath + ".IsOpen")
				.MakeReorderable()
				.MakeHeaderButton(_addDefinitionButton, new AddPopup(new AddVariableContent(this), _addDefinitionLabel), Color.white)
				.MakeCustomHeight(GetDefinitionHeight)
				.MakeEmptyLabel(_emptyLabel);

			_type = TypeHelper.GetAttribute<VariableInitializerAttribute>(fieldInfo)?.Type ?? VariableInitializerType.Expression;
			_availabilities = TypeHelper.GetAttribute<VariableAvailabilitiesAttribute>(fieldInfo)?.Availabilities;
		}

		public override float GetHeight(GUIContent label)
		{
			return List.GetHeight();
		}

		public override void Draw(Rect position, GUIContent label)
		{
			List.Draw(position, label);
		}

		private void AddDefinition(string name, VariableType type)
		{
			using (new UndoScope(_owner, true))
				_schema.AddDefinition(name, type);
		}

		private void RemoveDefinition(IList list, int index)
		{
			_schema.RemoveDefinition(index);
		}

		private float GetDefinitionHeight(int index)
		{
			return VariableDefinitionDrawer.GetHeight(_schema[index], _type, _availabilities);
		}

		private void DrawDefinition(Rect rect, IList list, int index)
		{
			using (var changes = new EditorGUI.ChangeCheckScope())
			{
				var definition = VariableDefinitionDrawer.Draw(rect, _schema[index], _type, _availabilities);

				if (changes.changed)
					_schema[index] = definition;
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
			private VariableSchemaControl _control;
			private VariableType _type = VariableType.Empty;
			private bool _typeValid = true;

			public AddVariableContent(VariableSchemaControl control)
			{
				_control = control;
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
				_control.AddDefinition(name, _type);
			}

			protected override bool IsNameInUse(string name)
			{
				return _control._schema.HasDefinition(name);
			}
		}
	}

	[CustomPropertyDrawer(typeof(VariableSchema))]
	public class VariableSchemaDrawer : ControlDrawer<VariableSchemaControl>
	{
	}
}
