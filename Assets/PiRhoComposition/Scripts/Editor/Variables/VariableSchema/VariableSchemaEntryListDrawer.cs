using PiRhoSoft.Utilities;
using PiRhoSoft.Utilities.Editor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace PiRhoSoft.Composition.Editor
{
	[CustomPropertyDrawer(typeof(VariableSchema.EntryList))]
	public class VariableSchemaEntryListDrawer : PropertyDrawer
	{
		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var itemsProperty = property.FindPropertyRelative(SerializedList<string>.ItemsProperty);
			var schema = property.GetParentObject<VariableSchema>();
			var proxy = new EntryProxy(itemsProperty, schema);
			var field = new DictionaryField();
			field.Setup(itemsProperty, proxy);

			return field;
		}

		private class EntryProxy : IDictionaryProxy
		{
			private readonly SerializedProperty _property;
			private readonly VariableSchema _schema;

			public int KeyCount => _schema.EntryCount;

			public string Label => "Definitions";
			public string Tooltip => "The variables available to objects using this schema";
			public string EmptyLabel => "Add variable definitions to be accessable by objects that use this schema";
			public string EmptyTooltip => "No definitions defined";
			public string AddPlaceholder => "(Add definition)";
			public string AddTooltip => "Add a definition with the specified name to the schema";
			public string RemoveTooltip => "Remove this definition from the schema";
			public string ReorderTooltip => "Drag to move the location of this definition in the schema";

			public bool AllowAdd => true;
			public bool AllowRemove => true;
			public bool AllowReorder => true;

			public EntryProxy(SerializedProperty property, VariableSchema schema)
			{
				_property = property;
				_schema = schema;
			}

			public VisualElement CreateField(int index)
			{
				var property = _property.GetArrayElementAtIndex(index);
				var value = _schema.GetEntry(index);
				var field = new VariableSchemaEntryField(property, _schema, value) { userData = index };
				field.BindProperty(property);
				return field;
			}

			public bool NeedsUpdate(VisualElement item, int index)
			{
				var entry = _schema.GetEntry(index);
				return !(item.userData is int i) || i != index || !_schema.Tags.Contains(entry.Tag);
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
				using (new ChangeScope(_schema))
					_schema.AddEntry(key);

				_property.serializedObject.Update();
			}

			public void RemoveItem(int index)
			{
				using (new ChangeScope(_schema))
					_schema.RemoveEntry(index);

				_property.serializedObject.Update();
			}

			public void ReorderItem(int from, int to)
			{
				using (new ChangeScope(_schema))
					_schema.MoveEntry(from, to);
				
				_property.serializedObject.Update();
			}
		}
	}
}
