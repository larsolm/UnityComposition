using PiRhoSoft.CompositionEngine;
using PiRhoSoft.UtilityEditor;
using System.Collections;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace PiRhoSoft.CompositionEditor
{
	[CustomEditor(typeof(VariableLink))]
	public class VariableLinkEditor : Editor
	{
		private static readonly Label _variablesLabel = new Label(typeof(VariableLink), nameof(VariableLink.Variables));
		private readonly static Label _addVariableButton = new Label(Icon.BuiltIn(Icon.CustomAdd), "", "Add a Variable to the list");
		private readonly static Label _removeVariableButton = new Label(Icon.BuiltIn(Icon.Remove), "", "Remove this Variable from the list");
		private readonly static GUIContent _addVariableContent = new GUIContent("Add Variable");
		private static readonly GUIContent _emptyContent = new GUIContent("Add Variables to be inserted into the global variable store");

		private ObjectListControl _list = new ObjectListControl();
		private CreateVariablePopup _createPopup = new CreateVariablePopup();
		private VariableLink _variables;

		void OnEnable()
		{
			_createPopup.Setup(_addVariableContent, PopupCreate, PopupValidate);
			_variables = target as VariableLink;
			_list.Setup(_variables.Variables)
				.MakeDrawable(DrawVariable)
				.MakeRemovable(_removeVariableButton, RemoveVariable)
				.MakeReorderable(Reorder)
				.MakeHeaderButton(_addVariableButton, _createPopup, Color.white)
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

			return VariableValueDrawer.GetHeight(variable.Value, definition, true);
		}

		private void DrawVariable(Rect rect, IList list, int index)
		{
			var variable = _variables.Variables[index];
			var definition = _variables.Constraints[index];
			var nameRect = RectHelper.TakeLabel(ref rect);
			var valueRect = RectHelper.TakeHorizontalSpace(ref rect);

			nameRect.height = EditorGUIUtility.singleLineHeight;

			var name = EditorGUI.TextField(nameRect, variable.Name);
			var value = VariableValueDrawer.Draw(rect, GUIContent.none, variable.Value, definition, true);

			_variables.Variables[index] = Variable.Create(name, value);
		}

		private void AddVariable(string name, ValueDefinition definition)
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
			_variables.Constraints.RemoveAt(from);
			_variables.Constraints.Insert(to, previous);
		}

		private void PopupCreate()
		{
			AddVariable(_createPopup.Name, _createPopup.Definition);
		}

		private bool PopupValidate()
		{
			_createPopup.IsNameValid = !_variables.Variables.Any(v => v.Name == _createPopup.Name);
			_createPopup.IsTypeValid = _createPopup.Definition.Type != VariableType.Empty;

			return _createPopup.IsNameValid && _createPopup.IsTypeValid;
		}

		private class CreateVariablePopup : CreateNamedPopup
		{
			public ValueDefinition Definition;
			public bool IsTypeValid = true;

			public CreateVariablePopup()
			{
				Definition = ValueDefinition.Create(VariableType.Empty);
			}

			protected override float GetContentHeight()
			{
				return base.GetContentHeight() + ValueDefinitionControl.GetHeight(Definition, VariableInitializerType.None, null, false);
			}

			protected override bool DrawContent()
			{
				var create = base.DrawContent();
				var expanded = false;

				using (new InvalidScope(!HasChanged || IsTypeValid))
					Definition = ValueDefinitionControl.Draw(GUIContent.none, Definition, VariableInitializerType.None, null, false, ref expanded);

				return create;
			}
		}
	}
}
