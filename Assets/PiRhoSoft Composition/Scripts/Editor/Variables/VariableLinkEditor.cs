using PiRhoSoft.CompositionEngine;
using PiRhoSoft.UtilityEditor;
using System.Collections;
using UnityEditor;
using UnityEngine;

namespace PiRhoSoft.CompositionEditor
{
	[CustomEditor(typeof(VariableLink))]
	public class VariableLinkEditor : Editor
	{
		private static readonly Label _variablesLabel = new Label(typeof(VariableLink), nameof(VariableLink.Variables));
		private readonly static IconButton _addVariableButton = new IconButton(IconButton.CustomAdd, "Add a Variable to the list");
		private readonly static IconButton _removeVariableButton = new IconButton(IconButton.Remove, "Remove this Variable from the list");
		private readonly static GUIContent _addVariableContent = new GUIContent("Add Variable");
		private static readonly GUIContent _emptyContent = new GUIContent("Add Variables to be inserted into the global variable store");

		private ObjectListControl _list = new ObjectListControl();
		private VariableLink _variables;

		void OnEnable()
		{
			if (!target)
				return;

			_variables = target as VariableLink;
			_list.Setup(_variables.Variables)
				.MakeDrawable(DrawVariable)
				.MakeRemovable(_removeVariableButton, RemoveVariable)
				.MakeCollapsable(serializedObject.targetObject.GetType().Name + ".IsOpen")
				.MakeReorderable(Reorder)
				.MakeHeaderButton(_addVariableButton, new AddPopup(new AddVariableContent(this), _addVariableContent), Color.white)
				.MakeCustomHeight(GetHeight)
				.MakeEmptyLabel(_emptyContent);
		}

		public override void OnInspectorGUI()
		{
			EditorGUILayout.Space();

			using (new UndoScope(_variables, false))
				_list.Draw(_variablesLabel.Content);
		}

		private float GetHeight(int index)
		{
			var variable = _variables.Variables[index];
			var definition = _variables.Constraints[index];

			return VariableValueDrawer.GetHeight(variable.Value, definition);
		}

		private void DrawVariable(Rect rect, IList list, int index)
		{
			var variable = _variables.Variables[index];
			var definition = _variables.Constraints[index];
			var nameRect = RectHelper.TakeLabel(ref rect);
			var valueRect = RectHelper.TakeHorizontalSpace(ref rect);

			nameRect.height = EditorGUIUtility.singleLineHeight;

			var name = EditorGUI.TextField(nameRect, variable.Name);
			var value = VariableValueDrawer.Draw(rect, GUIContent.none, variable.Value, definition);

			_variables.Variables[index] = Variable.Create(name, value);
		}

		private void AddVariable(string name, VariableDefinition definition)
		{
			using (new UndoScope(_variables, true))
			{
				var value = definition.Generate(null);
				_variables.Variables.Add(Variable.Create(name, value));
				_variables.Constraints.Add(definition);
			}
		}

		private void RemoveVariable(IList list, int index)
		{
			_variables.Variables.RemoveAt(index);
			_variables.Constraints.RemoveAt(index);
		}

		private void Reorder(int from, int to)
		{
			var previous = _variables.Constraints[from];
			_variables.Constraints[from] = _variables.Constraints[to];
			_variables.Constraints[to] = previous;
		}

		private class AddVariableContent : AddNamedItemContent
		{
			private VariableLinkEditor _editor;
			private VariableDefinition _definition;
			private bool _typeValid = true;

			public AddVariableContent(VariableLinkEditor editor)
			{
				_editor = editor;
				_definition = VariableDefinition.Create(string.Empty, VariableType.Empty);
			}

			protected override float GetHeight_()
			{
				return VariableDefinitionDrawer.GetHeight(_definition, VariableInitializerType.None, null);
			}

			protected override bool Draw_(bool clean)
			{
				using (new InvalidScope(clean || _typeValid))
					_definition = VariableDefinitionDrawer.Draw(_definition, VariableInitializerType.None, null, false);

				return false;
			}

			protected override bool Validate_()
			{
				_typeValid = _definition.Type != VariableType.Empty;
				return _typeValid;
			}

			protected override void Add_(string name)
			{
				_editor.AddVariable(name, _definition);
			}

			protected override bool IsNameInUse(string name)
			{
				return _editor._variables.Variables.Exists(variable => variable.Name == name);
			}
		}
	}
}
