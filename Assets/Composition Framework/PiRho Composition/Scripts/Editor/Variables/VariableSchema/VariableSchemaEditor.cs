using PiRhoSoft.Utilities.Editor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace PiRhoSoft.Composition.Editor
{
	[CustomEditor(typeof(VariableSchema))]
	public class VariableSchemaEditor : UnityEditor.Editor
	{
		public override VisualElement CreateInspectorGUI()
		{
			var tagsProperty = serializedObject.FindProperty(nameof(VariableSchema.Tags));
			var tagsControl = new PropertyField(tagsProperty, tagsProperty.displayName);

			var schema = target as VariableSchema;
			var proxy = new DefinitionsProxy(schema) { Label = "Definitions", Tooltip = "The variables available to objects using this schema" };
			var dictionaryControl = new DictionaryControl(proxy);

			var container = new VisualElement();
			container.Add(tagsControl);
			container.Add(dictionaryControl);
			container.Bind(serializedObject);

			return container;
		}

		private class DefinitionsProxy : DictionaryProxy
		{
			private VariableSchema _schema;

			public override int KeyCount => _schema.Count;

			public DefinitionsProxy(VariableSchema schema)
			{
				_schema = schema;
			}

			public override VisualElement CreateField(int index)
			{
				var entry = _schema.GetEntry(index);
				var control = new VariableSchemaEntryControl(_schema.Tags.List, entry) { userData = index };
				return control;
			}

			public override bool IsKeyValid(string key)
			{
				return !_schema.HasDefinition(key);
			}

			public override bool NeedsUpdate(VisualElement item, int index)
			{
				return !(item.userData is int i) || i != index;
			}

			public override void AddItem(string key)
			{
				_schema.AddDefinition(key, VariableType.Empty);
			}

			public override void RemoveItem(int index)
			{
				_schema.RemoveDefinition(index);
			}
		}
	}
}

