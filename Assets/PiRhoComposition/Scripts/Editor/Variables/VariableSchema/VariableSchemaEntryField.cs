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
		public const string TagUssClassName = UssClassName + "__tag";
		public const string NoInitializerUssClassName = UssClassName + "--no-initializer";
		public const string DefaultUssClassName = UssClassName + "--default";
		public const string ExpressionUssClassName = UssClassName + "--expression";
		public const string InitializerUssClassName = UssClassName + "__initializer-container";
		public const string TypeUssClassName = UssClassName + "__type";
		public const string DefaultTypeUssClassName = UssClassName + "__default";
		public const string ExpressionTypeUssClassName = UssClassName + "__expression";

		private static readonly Icon _modeIcon = Icon.BuiltIn("d_CustomSorting");

		public VariableSchemaEntry Value { get; private set; }

		public VariableSchemaEntryField(SerializedProperty property, VariableSchema schema, VariableSchemaEntry value)
		{
			Value = value;

			var definitionProperty = property.FindPropertyRelative(nameof(VariableSchemaEntry.Definition));
			var tagProperty = property.FindPropertyRelative(nameof(VariableSchemaEntry.Tag));
			var typeProperty = property.FindPropertyRelative(nameof(VariableSchemaEntry.Type));
			var initializerProperty = property.FindPropertyRelative(nameof(VariableSchemaEntry.Initializer));
			var defaultProperty = property.FindPropertyRelative(nameof(VariableSchemaEntry.Default));

			var definitionField = new VariableDefinitionField(definitionProperty, false);
			var dataProperty = definitionProperty.FindPropertyRelative(VariableDefinition.TypeProperty);
			var dataWatcher = new ChangeTriggerControl<Enum>(dataProperty, (oldValue, newValue) => RefreshInitializer());

			Add(definitionField);
			Add(dataWatcher);

			if (schema.Tags.Count > 0)
			{
				if (!schema.Tags.Contains(Value.Tag))
					Value.Tag = schema.Tags[0];

				var popup = new PopupField<string>("Tag", schema.Tags, Value.Tag);
				popup.AddToClassList(TagUssClassName);

				Add(popup.ConfigureProperty(tagProperty, "The tag assigned to this variable"));
			}

			var initializer = new VisualElement();
			initializer.AddToClassList(BaseFieldExtensions.UssClassName);
			initializer.AddToClassList(InitializerUssClassName);

			var typeToggle = new IconButton(_modeIcon.Texture, "Toggle initilazing the variable by a value or an expression", () => Value.Type = Value.Type == VariableSchemaInitializerType.Default ? VariableSchemaInitializerType.Initializer : VariableSchemaInitializerType.Default);
			var typeWatcher = new ChangeTriggerControl<Enum>(typeProperty, (oldValue, newValue) => RefreshInitializer());

			typeToggle.AddToClassList(TypeUssClassName);
			RefreshInitializer();

			var defaultField = new SerializedVariableField("Default", Value.Definition);
			defaultField.AddToClassList(DefaultTypeUssClassName);

			var expressionField = new ExpressionField("Initializer", Value.Initializer, null); // TODO: Add autocomplete
			expressionField.AddToClassList(ExpressionTypeUssClassName);

			initializer.Add(typeToggle);
			initializer.Add(typeWatcher);
			initializer.Add(defaultField.ConfigureProperty(defaultProperty, "The default value to assign when initializing, resetting, or updating the variable"));
			initializer.Add(expressionField.ConfigureProperty(initializerProperty, "The expression to execute and assign when initializing, resetting, or updating the variable"));

			Add(initializer);

			this.AddStyleSheet(Configuration.EditorPath, Stylesheet);
			AddToClassList(UssClassName);
		}

		private void RefreshInitializer()
		{
			EnableInClassList(NoInitializerUssClassName, !HasInitializer());
			this.AlternateClass(DefaultUssClassName, ExpressionUssClassName, Value.Type == VariableSchemaInitializerType.Default);
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
