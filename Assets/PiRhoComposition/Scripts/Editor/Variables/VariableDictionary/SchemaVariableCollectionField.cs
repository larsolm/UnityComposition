using PiRhoSoft.Utilities;
using PiRhoSoft.Utilities.Editor;
using System;
using UnityEditor;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace PiRhoSoft.Composition.Editor
{
	public class SchemaVariableCollectionField : SerializedVariableDictionaryField
	{
		public new const string Stylesheet = "Variables/VariableDictionary/SchemaVariableCollectionStyle.uss";
		public new const string UssClassName = "pirho-schema-variable-collection";
		public const string RefreshUssClassName = UssClassName + "__refresh";

		private static readonly Icon _refreshIcon = Icon.BuiltIn("preAudioLoopOff");

		public SchemaVariableCollectionField(SerializedProperty property)
		{
			var variables = property.GetObject<SchemaVariableCollection>();
			variables.UpdateSchema();
			property.serializedObject.Update();

			var schemaProperty = property.FindPropertyRelative(SchemaVariableCollection.SchemaProperty);
			var proxy = new SchemaVariableCollectionProxy(property, variables);

			Setup(property, proxy);

			var picker = new ObjectPickerField(null, variables.Schema, null, typeof(VariableSchema)) { bindingPath = schemaProperty.propertyPath };
			picker.RegisterCallback<ChangeEvent<Object>>(evt => variables.SetSchema(evt.newValue as VariableSchema));

			var schemaWatcher = new ChangeTriggerControl<Object>(schemaProperty, (oldSchema, newShema) => _dictionaryField.Control.Refresh());

			_dictionaryField.Control.Header.Add(picker);

			Add(schemaWatcher);

			this.AddStyleSheet(Configuration.EditorPath, Stylesheet);
			AddToClassList(UssClassName);
		}

		private class SchemaVariableCollectionProxy : VariableDictionaryProxy
		{
			public override string Label => "Schema Variables";
			public override string Tooltip => "The list of variables in this schema";
			public override string EmptyLabel => "No variables exist in this schema";
			public override string EmptyTooltip => "Add variables to the schema asset to edit them";

			private SchemaVariableCollection _collection => _variables as SchemaVariableCollection;

			public SchemaVariableCollectionProxy(SerializedProperty property, SchemaVariableCollection variables) : base(property, variables)
			{
			}

			public override VisualElement CreateField(int index)
			{
				var entry = _collection.Schema.GetEntry(index);
				var variable = _variables.GetVariable(index);

				var entriesProperty = new SerializedObject(_collection.Schema)
					.FindProperty(VariableSchema.EntriesField)
					.FindPropertyRelative(SerializedList<string>.ItemsProperty);

				var definitionProperty = entriesProperty
					.GetArrayElementAtIndex(index)
					.FindPropertyRelative(nameof(VariableSchemaEntry.Definition));

				var container = CreateContainer(index);
				var label = CreateLabel(definitionProperty);
				var control = CreateVariable(index, variable, entry.Definition);
				var refreshButton = new IconButton(_refreshIcon.Texture, "Recompute this variable based on the schema initializer", () =>
				{
					var newValue = entry.GenerateVariable(_variables);
					control.SetValue(newValue);
				});

				refreshButton.AddToClassList(RefreshUssClassName);

				container.Add(label);
				container.Add(control);
				container.Add(refreshButton);

				WatchDefinition(container, control, definitionProperty);

				return container;
			}
		}
	}
}
