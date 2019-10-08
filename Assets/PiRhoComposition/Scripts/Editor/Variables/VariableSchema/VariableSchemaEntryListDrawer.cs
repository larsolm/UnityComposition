using PiRhoSoft.Utilities.Editor;
using UnityEditor;
using UnityEngine.UIElements;

namespace PiRhoSoft.Composition.Editor
{
	[CustomPropertyDrawer(typeof(VariableSchema.EntryList))]
	public class VariableSchemaEntryListDrawer : PropertyDrawer
	{
		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var schema = property.GetParentObject<VariableSchema>();
			var proxy = new EntryProxy(schema);
			var field = new DictionaryControl(proxy);

			return field;
		}

		private class EntryProxy : IDictionaryProxy
		{
			private VariableSchema _schema;

			public int KeyCount => _schema.EntryCount;

			public string Label => "Definitions";
			public string Tooltip => "The variables available to objects using this schema";
			public string EmptyLabel => "Add variable definitions to be accessable by objects that use this schema";
			public string EmptyTooltip => "No definitions defined";
			public string AddPlaceholder => "(definition name)";
			public string AddTooltip => "Add a definition with the specified name to the schema";
			public string RemoveTooltip => "Remove this definition from the schema";
			public string ReorderTooltip => "Drag to move the location of this definition in the schema";

			public bool AllowAdd => true;
			public bool AllowRemove => true;
			public bool AllowReorder => true;

			public EntryProxy(VariableSchema schema)
			{
				_schema = schema;
			}

			public VisualElement CreateField(int index)
			{
				var entry = _schema.GetEntry(index);
				var control = new VariableSchemaEntryControl(_schema.Tags, entry) { userData = index };
				return control;
			}

			public bool NeedsUpdate(VisualElement item, int index)
			{
				return !(item.userData is int i) || i != index;
			}

			public bool CanAdd(string key)
			{
				return !_schema.HasEntry(key);
			}

			public bool CanRemove(int index)
			{
				return true;
			}

			public bool CanReorder(int from, int to)
			{
				return true;
			}

			public void AddItem(string key)
			{
				_schema.AddEntry(key);
			}

			public void RemoveItem(int index)
			{
				_schema.RemoveEntry(index);
			}

			public void ReorderItem(int from, int to)
			{
				_schema.MoveEntry(from, to);
			}
		}
	}
}
