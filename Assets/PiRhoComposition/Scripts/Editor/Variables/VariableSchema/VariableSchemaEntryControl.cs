using PiRhoSoft.Utilities.Editor;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Composition.Editor
{
	public class VariableSchemaEntryControl : VisualElement
	{
		public const string Stylesheet = "Variables/VariableSchema/VariableSchemaEntryStyle.uss";
		public const string UssClassName = "pirho-variable-schema-entry";
		public const string TagUssClassName = UssClassName + "__tag";
		public const string DefaultUssClassName = UssClassName + "--default";
		public const string ExpressionUssClassName = UssClassName + "--expression";
		public const string InitializerUssClassName = UssClassName + "__initializer";
		public const string InitializerTypeUssClassName = InitializerUssClassName + "__type";
		public const string DefaultTypeUssClassName = InitializerUssClassName + "__default";
		public const string ExpressionTypeUssClassName = InitializerUssClassName + "__expression";
		public const string InitializerLabelUssClassName = InitializerUssClassName + "__label";

		private static readonly Icon _modeIcon = Icon.BuiltIn("d_CustomSorting");

		public VariableSchemaEntry Value;
		public List<string> Tags;

		private VariableDefinitionControl _definitionControl;
		private VisualElement _tag;
		private VisualElement _initializer;
		private TextElement _initializerLabel;

		private IconButton _typeToggle;
		private VariableControl _defaultControl;
		private ExpressionControl _expressionControl;

		public VariableSchemaEntryControl(List<string> tags, VariableSchemaEntry value)
		{
			this.AddStyleSheet(Configuration.EditorPath, Stylesheet);
			AddToClassList(UssClassName);

			Value = value;
			Tags = tags;

			_definitionControl = new VariableDefinitionControl(Value.Definition, null);
			_definitionControl.RegisterCallback<ChangeEvent<VariableDefinition>>(evt => Refresh());

			_tag = new VisualElement();
			_tag.AddToClassList(TagUssClassName);

			_initializer = new VisualElement();
			_initializer.AddToClassList(InitializerUssClassName);

			Add(_definitionControl);
			Add(_tag);
			Add(_initializer);

			Refresh();
		}

		private void Refresh()
		{
			_definitionControl.SetValueWithoutNotify(Value.Definition);
			_tag.Clear();
			_initializer.Clear();

			if (Tags.Count > 0)
				CreateTag();

			if (HasInitializer())
			{
				CreateInitializer();
				RefreshInitializer();
			}
		}

		private void RefreshInitializer()
		{
			_defaultControl.SetDefinition(Value.Definition);
			_initializerLabel.text = Value.Type == VariableSchemaInitializerType.Default ? "Default:" : "Initializer";

			this.AlternateClass(DefaultUssClassName, ExpressionUssClassName, Value.Type == VariableSchemaInitializerType.Default);
		}

		private void CreateTag()
		{
			if (!Tags.Contains(Value.Tag))
				Value.Tag = Tags[0];

			var popup = new PopupField<string>("Tag:", Tags, Value.Tag);
			_tag.Add(popup);
		}

		private void CreateInitializer()
		{
			_typeToggle = new IconButton(_modeIcon.Texture, "Toggle initilazing by a default value or an expression", () =>
			{
				Value.Type = Value.Type == VariableSchemaInitializerType.Default ? VariableSchemaInitializerType.Initializer : VariableSchemaInitializerType.Default;
				RefreshInitializer();
			});

			_typeToggle.AddToClassList(InitializerTypeUssClassName);

			_initializerLabel = new TextElement();
			_initializerLabel.AddToClassList(InitializerLabelUssClassName);

			_defaultControl = new VariableControl(Value.Default, Value.Definition, null) { tooltip = "The default value to assign when initializing, resetting, or updating the variable" };
			_defaultControl.AddToClassList(DefaultTypeUssClassName);
			_defaultControl.RegisterCallback<ChangeEvent<Variable>>(evt => Value.Default = evt.newValue);

			_expressionControl = new ExpressionControl(Value.Initializer, null) { tooltip = "The expression to execute and assign when initializing, resetting, or updating the variable" }; // TODO: Add autocomplete
			_expressionControl.AddToClassList(ExpressionTypeUssClassName);

			_initializer.Add(_typeToggle);
			_initializer.Add(_initializerLabel);
			_initializer.Add(_defaultControl);
			_initializer.Add(_expressionControl);
		}

		private bool HasInitializer()
		{
			switch (Value.Definition.Type)
			{
				case VariableType.Bool:
				case VariableType.Float:
				case VariableType.Int:
				case VariableType.Vector2Int:
				case VariableType.Vector3Int:
				case VariableType.RectInt:
				case VariableType.BoundsInt:
				case VariableType.Vector2:
				case VariableType.Vector3:
				case VariableType.Vector4:
				case VariableType.Quaternion:
				case VariableType.Rect:
				case VariableType.Bounds:
				case VariableType.Color:
				case VariableType.String: return true;
				default: return false;
			}
		}
	}
}
