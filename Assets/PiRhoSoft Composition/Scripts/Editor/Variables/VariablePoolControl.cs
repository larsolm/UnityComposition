using PiRhoSoft.CompositionEngine;
using PiRhoSoft.UtilityEditor;
using System.Collections;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace PiRhoSoft.CompositionEditor
{
	public class VariablePoolControl : ObjectControl<VariablePool>
	{
		private static Label _addButton = new Label(Icon.BuiltIn(Icon.CustomAdd), string.Empty, "Add a variable");
		private static Label _removeButton = new Label(Icon.BuiltIn(Icon.Remove), string.Empty, "Remove this variable");
		private static Label _editButton = new Label(Icon.BuiltIn("_Popup"), string.Empty, "Modify this variable");
		private const float _labelWidth = 100.0f;

		private SerializedProperty _property;
		private ObjectListControl _list = new ObjectListControl();
		private VariablePool _pool;

		private CreateNamedPopup _createPopup = new CreateNamedPopup();
		private EditVariablePopup _editPopup = new EditVariablePopup();

		public override void Setup(VariablePool target, SerializedProperty property, FieldInfo fieldInfo, PropertyAttribute attribute)
		{
			_property = property;
			_pool = target;

			if (property.serializedObject.targetObject is ISchemaOwner owner)
				owner.SetupSchema();

			_createPopup.Setup(new GUIContent("Add Variable"), PopupCreate, PopupValidate);

			_list.Setup(_pool.Variables)
				.MakeRemovable(_removeButton, RemoveVariable)
				.MakeDrawable(DrawVariable)
				.MakeReorderable(VariablesReordered)
				.MakeCustomHeight(GetVariableHeight)
				.MakeHeaderButton(_addButton, _createPopup, Color.white)
				.MakeCollapsable(property.isExpanded, OnCollapse);
		}

		private bool PopupValidate()
		{
			_createPopup.IsNameValid = !_pool.Map.ContainsKey(_createPopup.Name);
			return _createPopup.IsNameValid;
		}

		private void PopupCreate()
		{
			using (new UndoScope(_property.serializedObject.targetObject, true))
				_pool.AddVariable(_createPopup.Name, VariableValue.Empty);
		}

		private void ApplyDefinition(int index, string name, ValueDefinition definition)
		{
			using (new UndoScope(_property.serializedObject.targetObject, true))
			{
				_pool.ChangeName(index, name);
				_pool.ChangeDefinition(index, definition);
			}
		}

		private void RemoveVariable(IList list, int index)
		{
			_pool.RemoveVariable(index);
		}

		private void VariablesReordered(int from, int to)
		{
			_pool.VariableMoved(from, to);
		}

		private float GetVariableHeight(int index)
		{
			var variable = _pool.Variables[index];
			var definition = ValueDefinition.Create(VariableType.Empty);

			return VariableValueDrawer.GetHeight(variable.Value, definition, true);
		}

		private void DrawVariable(Rect rect, IList list, int index)
		{
			var variable = _pool.Variables[index];
			var definition = _pool.Definitions[index];

			var labelRect = RectHelper.TakeWidth(ref rect, _labelWidth);
			labelRect = RectHelper.TakeHeight(ref labelRect, EditorGUIUtility.singleLineHeight);
			var editRect = RectHelper.TakeLeadingIcon(ref labelRect);

			if (GUI.Button(editRect, _editButton.Content, GUIStyle.none))
			{
				_editPopup.Setup(this, index);
				PopupWindow.Show(editRect, _editPopup);
			}

			EditorGUI.LabelField(labelRect, variable.Name);

			using (var changes = new EditorGUI.ChangeCheckScope())
			{
				var value = VariableValueDrawer.Draw(rect, GUIContent.none, variable.Value, definition, true);

				if (changes.changed)
					_pool.SetVariable(index, value);
			}
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

		private class EditVariablePopup : PopupWindowContent
		{
			private VariablePoolControl _control;
			private int _index;

			private string _name;
			private ValueDefinition _definition;

			public void Setup(VariablePoolControl control, int index)
			{
				_control = control;
				_index = index;

				_name = control._pool.Variables[index].Name;
				_definition = control._pool.Definitions[index];
			}

			public override Vector2 GetWindowSize()
			{
				var size = base.GetWindowSize();

				size.y = 10.0f; // padding
				size.y += RectHelper.LineHeight; // name
				size.y += ValueDefinitionControl.GetHeight(_definition, VariableInitializerType.None, null, false);
				size.y += RectHelper.LineHeight; // button

				return size;
			}

			public override void OnGUI(Rect rect)
			{
				rect = RectHelper.Inset(rect, 5.0f);
				var nameRect = RectHelper.TakeLine(ref rect);
				var buttonRect = RectHelper.TakeTrailingHeight(ref rect, EditorGUIUtility.singleLineHeight);
				RectHelper.TakeTrailingHeight(ref rect, RectHelper.VerticalSpace);

				var isExpanded = false;
				_name = EditorGUI.TextField(nameRect, _name);
				_definition = ValueDefinitionControl.Draw(rect, GUIContent.none, _definition, VariableInitializerType.None, null, false, ref isExpanded);

				if (GUI.Button(buttonRect, "Apply"))
				{
					_control.ApplyDefinition(_index, _name, _definition);
					editorWindow.Close();
				}
			}
		}
	}

	[CustomPropertyDrawer(typeof(VariablePool))]
	public class VariablePoolDrawer : PropertyDrawer<VariablePoolControl>
	{
	}
}
