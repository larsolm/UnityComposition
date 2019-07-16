using PiRhoSoft.Composition;
using PiRhoSoft.Utilities.Editor;
using UnityEditor;
using UnityEngine.UIElements;

namespace PiRhoSoft.Composition.Editor
{
	[CustomEditor(typeof(VariableSchema))]
	public class VariableSchemaEditor : UnityEditor.Editor
	{
		public override VisualElement CreateInspectorGUI()
		{
			var schema = target as VariableSchema;
			var proxy = new DefinitionsProxy(schema);
			var dictionary = new DictionaryElement(proxy, "Definitions", "The variables available to objects using this schema");

			return dictionary;
		}

		private class DefinitionsProxy : DictionaryProxy
		{
			private VariableSchema _schema;
			private TextField _addText;

			public override int Count => _schema.Count;

			public DefinitionsProxy(VariableSchema schema)
			{
				_schema = schema;
			}

			public override VisualElement CreateAddElement()
			{
				_addText = new TextField();
				_addText.RegisterValueChangedCallback(evt =>
				{
					ElementHelper.ToggleClass(_addText, DictionaryElement.UssInvalidKeyClass, !IsValidKey(evt.newValue));
				});

				return new Placeholder<string>("(new key)", _addText);
			}

			public override VisualElement CreateKeyElement(int index)
			{
				var key = _schema[index].Name;
				var label = new Label(key);
				return label;
			}

			public override VisualElement CreateValueElement(int index)
			{
				var variable = _schema[index];
				var element = new ValueDefinitionElement(_schema, () => _schema[index].Definition, value => _schema[index] = new VariableDefinition { Name = variable.Name, Definition = value }, () => _schema.InitializerType, () => _schema.Tags, true);
				return element;
			}

			public override void AddItem()
			{
				var key = _addText.value;

				if (IsValidKey(key))
					_schema.AddDefinition(string.Empty, VariableType.Empty);
			}

			public override void RemoveItem(int index)
			{
				_schema.RemoveDefinition(index);
			}

			private bool IsValidKey(string key)
			{
				return !string.IsNullOrEmpty(key) && !_schema.HasDefinition(key);
			}
		}
	}
}

