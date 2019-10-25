using PiRhoSoft.Utilities;
using PiRhoSoft.Utilities.Editor;
using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace PiRhoSoft.Composition.Editor
{
	public class VariableSchemaEntryField : BindableElement
	{
		public const string Stylesheet = "Variables/VariableSchema/VariableSchemaEntryStyle.uss";
		public const string UssClassName = "pirho-variable-schema-entry";
		public const string NoInitializerUssClassName = UssClassName + "--no-initializer";
		public const string DefaultUssClassName = UssClassName + "--default";
		public const string ExpressionUssClassName = UssClassName + "--expression";
		public const string TagUssClassName = UssClassName + "__tag";
		public const string InitializerUssClassName = UssClassName + "__initializer";
		public const string InitializerLabelUssClassName = InitializerUssClassName + "__label";
		public const string InitializerTypeUssClassName = InitializerUssClassName + "__type";
		public const string InitializerDefaultTypeUssClassName = InitializerUssClassName + "__default";
		public const string InitializerExpressionTypeUssClassName = InitializerUssClassName + "__expression";

		private static readonly Icon _modeIcon = Icon.BuiltIn("d_CustomSorting");

		public VariableSchemaEntry Value { get; private set; }
		public VariableSchema Schema { get; private set; }

		private VisualElement _tags;
		private Label _initializerLabel;

		public VariableSchemaEntryField(SerializedProperty property, VariableSchema schema, VariableSchemaEntry value)
		{
			Value = value;
			Schema = schema;

			var definitionProperty = property.FindPropertyRelative(nameof(VariableSchemaEntry.Definition));
			var tagProperty = property.FindPropertyRelative(nameof(VariableSchemaEntry.Tag));
			var typeProperty = property.FindPropertyRelative(nameof(VariableSchemaEntry.Type));
			var defaultProperty = property.FindPropertyRelative(nameof(VariableSchemaEntry.Default));
			var initializerProperty = property.FindPropertyRelative(nameof(VariableSchemaEntry.Initializer));

			CreateDefinition(definitionProperty);
			CreateInitializer(definitionProperty, typeProperty, defaultProperty, initializerProperty);
			CreateTags(tagProperty);

			RefreshInitializer();

			this.AddStyleSheet(Configuration.EditorPath, Stylesheet);
			AddToClassList(UssClassName);
		}

		private void RefreshInitializer()
		{
			var isDefault = Value.Type == VariableSchemaInitializerType.Default;

			EnableInClassList(NoInitializerUssClassName, !HasInitializer());
			this.AlternateClass(DefaultUssClassName, ExpressionUssClassName, isDefault);

			_initializerLabel.text = isDefault ? "Default" : "Initializer";
			_initializerLabel.tooltip = isDefault ? "The default value to assign when initializing, resetting, or updating the variable" : "The expression to execute and assign when initializing, resetting, or updating the variable";
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

		private void CreateDefinition(SerializedProperty property)
		{
			var definitionField = new VariableDefinitionField(property, true);
			var dataProperty = property.FindPropertyRelative(VariableDefinition.TypeProperty);
			var dataWatcher = new ChangeTriggerControl<Enum>(dataProperty, (oldValue, newValue) => RefreshInitializer());

			Add(definitionField);
			Add(dataWatcher);
		}

		private void CreateTags(SerializedProperty property)
		{
			_tags = new VisualElement();
			_tags.AddToClassList(TagUssClassName);

			var tagsProperty = new SerializedObject(Schema)
				.FindProperty(VariableSchema.TagsField)
				.FindPropertyRelative(SerializedList<string>.ItemsProperty)
				.FindPropertyRelative("Array.size");

			var tagsWatcher = new ChangeTriggerControl<int>(tagsProperty, (from, to) => RebuildTags(property));

			RebuildTags(property);
			Add(_tags);
			Add(tagsWatcher);
		}

		private void RebuildTags(SerializedProperty property)
		{
			_tags.Clear();

			if (Schema.Tags.Count > 0)
			{
				if (!Schema.Tags.Contains(Value.Tag))
					Value.Tag = Schema.Tags[0];

				var popup = new PopupField<string>("Tag", Schema.Tags, Value.Tag);
				_tags.Add(popup.ConfigureProperty(property));
			}
		}

		private void CreateInitializer(SerializedProperty definitionProperty, SerializedProperty typeProperty, SerializedProperty defaultProperty, SerializedProperty initializerProperty)
		{
			var typeToggle = new IconButton(_modeIcon.Texture, "Toggle initilazing the variable by a value or an expression", () => Value.Type = Value.Type == VariableSchemaInitializerType.Default ? VariableSchemaInitializerType.Initializer : VariableSchemaInitializerType.Default);
			typeToggle.AddToClassList(InitializerTypeUssClassName);

			var typeWatcher = new ChangeTriggerControl<Enum>(typeProperty, (oldValue, newValue) => RefreshInitializer());

			_initializerLabel = new Label();
			_initializerLabel.AddToClassList(InitializerLabelUssClassName);

			var defaultField = new SerializedVariableField(defaultProperty, definitionProperty);
			defaultField.AddToClassList(InitializerDefaultTypeUssClassName);

			var expressionField = new ExpressionField(initializerProperty, null); // TODO: Add autocomplete
			expressionField.SetLabel(null);
			expressionField.AddToClassList(InitializerExpressionTypeUssClassName);

			var initializer = new VisualElement();
			initializer.AddToClassList(BaseFieldExtensions.UssClassName);
			initializer.AddToClassList(InitializerUssClassName);
			initializer.Add(typeToggle);
			initializer.Add(typeWatcher);
			initializer.Add(_initializerLabel);
			initializer.Add(defaultField);
			initializer.Add(expressionField);

			Add(initializer);
		}
	}
}
