using System.Collections;
using System.Reflection;
using PiRhoSoft.CompositionEngine;
using PiRhoSoft.UtilityEditor;
using UnityEditor;
using UnityEngine;

namespace PiRhoSoft.CompositionEditor
{
	public class VariableListControl : ObjectControl<VariableList>
	{
		private readonly static IconButton _addButton = new IconButton(IconButton.CustomAdd, "Add a variable");
		private readonly static IconButton _removeButton = new IconButton(IconButton.Remove, "Remove this variable");
		private readonly static GUIContent _emptyLabel = new GUIContent("No variables have been added");

		private ObjectListControl _list = new ObjectListControl();
		private VariableList _variables;
		private VariablesProxy _proxy;

		public override void Setup(VariableList target, SerializedProperty property, FieldInfo fieldInfo, PropertyAttribute attribute)
		{
			_variables = target;
			_proxy = new VariablesProxy { List = target };

			_list.Setup(_proxy)
				.MakeDrawable(DrawVariable)
				.MakeRemovable(_removeButton)
				.MakeHeaderButton(_addButton, new AddPopup(new AddVariableContent(this), GUIContent.none), Color.white)
				.MakeCollapsable(property.serializedObject.targetObject.GetType().Name + "." + property.propertyPath + ".IsOpen")
				.MakeEmptyLabel(_emptyLabel);
		}

		public override float GetHeight(GUIContent label)
		{
			return _list.GetHeight();
		}

		public override void Draw(Rect position, GUIContent label)
		{
			_list.Draw(position, label);
		}

		private void DrawVariable(Rect rect, IList list, int index)
		{
			var name = _variables.GetVariableName(index);
			var value = _variables.GetVariableValue(index);
			var definition = VariableDefinition.Create("", VariableType.Empty);

			using (var changes = new EditorGUI.ChangeCheckScope())
			{
				value = VariableValueDrawer.Draw(rect, new GUIContent(name), value, definition);

				if (changes.changed)
					_variables.SetVariableValue(index, value);
			}
		}

		private class VariablesProxy : ListProxy
		{
			public VariableList List;

			public override int Count => List.VariableCount;
			public override object this[int index] { get => List.GetVariableValue(index); set => List.SetVariableValue(index, (VariableValue)value); }
		}

		private class AddVariableContent : AddNamedItemContent
		{
			private VariableListControl _control;
			private VariableType _type = VariableType.Empty;
			private bool _typeValid = true;

			public AddVariableContent(VariableListControl control)
			{
				_control = control;
			}

			protected override void Add_(string name)
			{
				_control._variables.AddVariable(name, VariableValue.Create(_type));
			}

			protected override bool IsNameInUse(string name)
			{
				return _control._variables.GetVariableIndex(name) >= 0;
			}

			protected override float GetHeight_()
			{
				return base.GetHeight_() + EditorGUIUtility.standardVerticalSpacing + EditorGUIUtility.singleLineHeight;
			}

			protected override bool Draw_(bool clean)
			{
				var create = base.Draw_(clean);
				_type = (VariableType)EditorGUILayout.EnumPopup(_type);
				return create;
			}

			protected override void Reset_()
			{
				base.Reset_();
				_type = VariableType.Empty;
			}

			protected override bool Validate_()
			{
				_typeValid = _type != VariableType.Empty;
				return base.Validate_() && _typeValid;
			}
		}
	}

	[CustomPropertyDrawer(typeof(VariableList))]
	public class VariableListDrawer : ControlDrawer<VariableListControl>
	{
	}
}
