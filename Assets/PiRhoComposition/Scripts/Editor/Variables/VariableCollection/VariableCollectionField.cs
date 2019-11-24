using PiRhoSoft.Utilities;
using PiRhoSoft.Utilities.Editor;
using UnityEditor;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace PiRhoSoft.Composition.Editor
{
	public class VariableCollectionField : SerializedVariableDictionaryField
	{
		public new const string Stylesheet = "Variables/VariableCollection/VariableCollectionStyle.uss";
		public new const string UssClassName = "pirho-variable-collection";

		public const string RefreshUssClassName = UssClassName + "__refresh";
		public const string SettingsUssClassName = UssClassName + "__settings";
		public const string PopupUssClassName = UssClassName + "__popup";
		public const string PopupOpenUssClassName = PopupUssClassName + "--open";
		public const string PopupDefinitionUssClassName = PopupUssClassName + "__definition";
		public const string PopupCloseUssClassName = PopupUssClassName + "__close";

		private static readonly Icon _refreshIcon = Icon.BuiltIn("preAudioLoopOff");

		public VariableCollectionField(SerializedProperty property)
		{
			var variables = property.GetObject<VariableCollection>();
			var schemaProperty = property.FindPropertyRelative(nameof(VariableCollection.Schema));
			var proxy = new VariableCollectionProxy(property, variables);

			Setup(property, proxy);

			var picker = new ObjectPickerField(schemaProperty, typeof(VariableSchema));
			picker.SetLabel(null);
			picker.RegisterCallback<ChangeEvent<Object>>(evt =>
			{
				variables.Schema = evt.newValue as VariableSchema;
				property.serializedObject.Update();
				_dictionaryField.Control.Refresh();
			});

			_dictionaryField.Control.Header.Add(picker);

			this.AddStyleSheet(Configuration.EditorPath, Stylesheet);
			AddToClassList(UssClassName);
		}

		private class VariableCollectionProxy : VariableDictionaryProxy
		{
			public override string Label => "Schema Variables";
			public override string Tooltip => "The list of variables in this schema";
			public override string EmptyLabel => "No variables exist in this schema";
			public override string EmptyTooltip => "Add variables to the schema asset to edit them";

			private VariableCollection _collection => _variables as VariableCollection;

			public VariableCollectionProxy(SerializedProperty property, VariableCollection variables) : base(property, variables)
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
