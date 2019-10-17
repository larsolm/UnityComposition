using PiRhoSoft.Utilities.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Composition.Editor
{
	public class CustomVariableCollectionField : SerializedVariableDictionaryField
	{
		public void Setup(CustomVariableCollectionProxy proxy)
		{
			Setup(new VariableDictionaryControl(proxy));
		}

		protected override void ExecuteDefaultActionAtTarget(EventBase evt)
		{
			base.ExecuteDefaultActionAtTarget(evt);

			if (this.TryGetPropertyBindEvent(evt, out var property))
			{
				var schema = property.FindPropertyRelative(CustomVariableCollection.DefinitionsProperty); // TODO
				var sizeBinding = new ChangeTriggerControl<Object>(null, (oldSize, size) => _control.Refresh());
				sizeBinding.Watch(schema);
			}
		}
	}

	public class CustomVariableCollectionProxy : SerializedVariableDictionaryProxy
	{
		public CustomVariableCollection Collection => Variables as CustomVariableCollection;

		public CustomVariableCollectionProxy(SerializedProperty property) : base(property)
		{
		}

		public override VisualElement CreateField(int index)
		{
			var variables = Variables.GetVariable(index);
			var definition = Collection.GetDefinition(index);

			var field = new VariableField(Property, Variables, index, definition) { userData = index };
			field.RegisterCallback<ChangeEvent<Variable>>(evt => { Variables.SetVariable(index, evt.newValue); });

			return field;
		}
	}
}
