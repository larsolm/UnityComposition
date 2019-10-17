using PiRhoSoft.Utilities.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Composition.Editor
{
	public class SchemaVariableCollectionField : SerializedVariableDictionaryField
	{
		public void Setup(SchemaVariableCollectionProxy proxy)
		{
			Setup(new SchemaVariableCollectionControl(proxy));
		}

		protected override void ExecuteDefaultActionAtTarget(EventBase evt)
		{
			base.ExecuteDefaultActionAtTarget(evt);

			if (this.TryGetPropertyBindEvent(evt, out var property))
			{
				var schema = property.FindPropertyRelative(SchemaVariableCollection.SchemaProperty);
				var schemaBinding = new ChangeTriggerControl<Object>(null, (oldSchema, newShema) => _control.Refresh());
				schemaBinding.Watch(schema);
			}
		}
	}

	public class SchemaVariableCollectionProxy : SerializedVariableDictionaryProxy
	{
		public SchemaVariableCollection Collection => Variables as SchemaVariableCollection;
		public VariableSchema Schema => Collection.Schema;

		public override bool AllowAdd => false;
		public override bool AllowRemove => false;
		public override bool AllowReorder => false;

		public SchemaVariableCollectionProxy(SerializedProperty property) : base(property)
		{
		}

		public override VisualElement CreateField(int index)
		{
			var container = new VisualElement { userData = index };
			var entry = Schema?.GetEntry(index);

			if (entry != null)
			{
				var field = new VariableField(Property, Variables, index, entry.Definition);
				field.RegisterCallback<ChangeEvent<Variable>>(evt => { Variables.SetVariable(index, evt.newValue); });

				var refreshButton = new IconButton(Icon.Refresh.Texture, "Recompute this variable based on the schema initializer", () =>
				{
					var newValue = entry.GenerateVariable(Variables);
					field.SetValue(newValue);
				});

				container.Add(field);
				container.Add(refreshButton);
			}

			return container;
		}
	}
}
