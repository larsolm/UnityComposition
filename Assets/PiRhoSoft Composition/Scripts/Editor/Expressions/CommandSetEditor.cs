using System.Collections;
using PiRhoSoft.CompositionEngine;
using PiRhoSoft.UtilityEditor;
using UnityEditor;
using UnityEngine;

namespace PiRhoSoft.CompositionEditor
{
	[CustomEditor(typeof(CommandSet))]
	public class CommandSetEditor : Editor
	{
		private readonly static IconButton _addExpressionCommandButton = new IconButton(IconButton.CustomAdd, "Add an Expression Command");
		private readonly static IconButton _removeCommandButton = new IconButton(IconButton.Remove, "Remove this Command");

		private readonly static GUIContent _addCustomCommandLabel = new GUIContent("Add Command");
		private readonly static GUIContent _customCommandsEmptyLabel = new GUIContent("No custom Commands");
		private readonly static Label _customCommandsLabel = new Label(typeof(CommandSet), nameof(CommandSet.CustomCommands));

		private const float _labelWidth = 100.0f;

		private CommandSet _composition;
		private ObjectListControl _customCommandsList = new ObjectListControl();

		void OnEnable()
		{
			_composition = target as CommandSet;

			_customCommandsList.Setup(_composition.CustomCommands)
				.MakeRemovable(_removeCommandButton, Remove)
				.MakeDrawable(Draw)
				.MakeHeaderButton(_addExpressionCommandButton, new AddPopup(new AddExpressionContent(this), _addCustomCommandLabel), Color.white)
				.MakeCollapsable(typeof(CommandSet).Name + "." + nameof(CommandSet.CustomCommands) + ".IsOpen")
				.MakeEmptyLabel(_customCommandsEmptyLabel)
				.MakeReorderable()
				.MakeCustomHeight(GetCommandHeight);
		}

		public override void OnInspectorGUI()
		{
			using (new UndoScope(_composition, false))
				_customCommandsList.Draw(_customCommandsLabel.Content);
		}

		private void Draw(Rect rect, IList list, int index)
		{
			var command = _composition.CustomCommands[index];
			var labelRect = RectHelper.TakeWidth(ref rect, _labelWidth);

			EditorGUI.LabelField(labelRect, command.Name);

			using (var changes = new EditorGUI.ChangeCheckScope())
			{
				ExpressionControl.DrawFoldout(rect, command.Expression, GUIContent.none);

				if (changes.changed)
					_composition.SetExpression(index, command.Expression);
			}
		}

		private float GetCommandHeight(int index)
		{
			var command = _composition.CustomCommands[index];
			return ExpressionControl.GetHeight(command.Expression, true);
		}

		private void AddExpression(string name)
		{
			using (new UndoScope(_composition, true))
				_composition.AddExpression(name);
		}

		private void Remove(IList list, int index)
		{
			_composition.RemoveCommand(index);
		}

		private class AddExpressionContent : AddNamedItemContent
		{
			private CommandSetEditor _editor;

			public AddExpressionContent(CommandSetEditor editor) => _editor = editor;
			protected override void Add_(string name) => _editor.AddExpression(name);
			protected override bool IsNameInUse(string name) => !_editor._composition.IsNameAvailable(name);
		}
	}
}
