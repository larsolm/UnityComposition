using PiRhoSoft.CompositionEngine;
using PiRhoSoft.UtilityEditor;
using System.Collections;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace PiRhoSoft.CompositionEditor
{
	public class PoolVariableStoreControl : ObjectControl<PoolVariableStore>
	{
		private static Label _addButton = new Label(Icon.BuiltIn(Icon.CustomAdd), string.Empty, "Add a variable");
		private static Label _removeButton = new Label(Icon.BuiltIn(Icon.Remove), "", "Remove this Variable");
		private const float _labelWidth = 100.0f;

		private SerializedProperty _property;
		private ObjectListControl _list = new ObjectListControl();
		private PoolVariableStore _variables;

		private CreateVariablePopup _createPopup = new CreateVariablePopup();

		public override void Setup(PoolVariableStore target, SerializedProperty property, FieldInfo fieldInfo, PropertyAttribute attribute)
		{
			_property = property;
			_variables = target;

			if (property.serializedObject.targetObject is ISchemaOwner owner)
				owner.SetupSchema();

			_createPopup.Setup(new GUIContent("Add Variable"), PopupCreate, PopupValidate);

			_list.Setup(_variables.Variables)
				.MakeRemovable(_removeButton, RemoveVariable)
				.MakeDrawable(DrawVariable)
				.MakeCustomHeight(GetVariableHeight)
				.MakeHeaderButton(_addButton, _createPopup, Color.white)
				.MakeCollapsable(property.isExpanded, OnCollapse);
		}

		private bool PopupValidate()
		{
			_createPopup.IsNameValid = !_variables.Map.ContainsKey(_createPopup.Name);
			_createPopup.IsTypeValid = _createPopup.Type != VariableType.Empty;

			return _createPopup.IsNameValid && _createPopup.IsTypeValid;
		}

		private void PopupCreate()
		{
			_variables.AddVariable(_createPopup.Name, VariableHandler.CreateDefault(_createPopup.Type, null));
		}

		private void RemoveVariable(IList list, int index)
		{
			_variables.RemoveVariable(index);
		}

		private float GetVariableHeight(int index)
		{
			var variable = _variables.Variables[index];
			var definition = ValueDefinition.Create(VariableType.Empty);

			return VariableValueDrawer.GetHeight(variable.Value, definition);
		}

		private void DrawVariable(Rect rect, IList list, int index)
		{
			var variable = _variables.Variables[index];
			var definition = ValueDefinition.Create(VariableType.Empty);

			if (variable.Value.IsEmpty)
			{
				EditorGUI.LabelField(rect, variable.Name, EmptyVariableHandler.EmptyText);
			}
			else
			{
				using (var changes = new EditorGUI.ChangeCheckScope())
				{
					var labelRect = RectHelper.TakeWidth(ref rect, _labelWidth);
					labelRect = RectHelper.TakeHeight(ref labelRect, EditorGUIUtility.singleLineHeight);

					EditorGUI.LabelField(labelRect, variable.Name);
					var value = VariableValueDrawer.Draw(rect, GUIContent.none, variable.Value, definition);

					if (changes.changed)
						_variables.SetVariable(variable.Name, value);
				}
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

	[CustomPropertyDrawer(typeof(PoolVariableStore))]
	public class PoolVariableStoreDrawer : PropertyDrawer<PoolVariableStoreControl>
	{
	}
}
