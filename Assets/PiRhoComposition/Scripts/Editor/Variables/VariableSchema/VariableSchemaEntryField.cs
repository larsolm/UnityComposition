using PiRhoSoft.Utilities.Editor;
using System;
using UnityEditor;
using UnityEngine.UIElements;

namespace PiRhoSoft.Composition.Editor
{
	public class VariableSchemaEntryField : BindableElement
	{
		private VariableSchemaEntryControl _control;

		public VariableSchemaEntryField(SerializedProperty property, VariableSchema schema)
		{
			bindingPath = property.propertyPath;

			var value = property.GetObject<VariableSchemaEntry>();

			_control = new VariableSchemaEntryControl(schema.Tags, value);

			Add(_control);
		}

		protected override void ExecuteDefaultActionAtTarget(EventBase evt)
		{
			base.ExecuteDefaultActionAtTarget(evt);

			if (this.TryGetPropertyBindEvent(evt, out var property))
			{
				var tagBinding = new ChangeTriggerControl<string>(null, (oldTag, newTag) => _control.Refresh());
				tagBinding.Watch(property.FindPropertyRelative(nameof(VariableSchemaEntry.Tag)));

				var typeBinding = new ChangeTriggerControl<Enum>(null, (oldType, newType) => _control.Refresh());
				typeBinding.Watch(property.FindPropertyRelative(nameof(VariableSchemaEntry.Type)));

				var expressionBinding = new ChangeTriggerControl<string>(null, (oldExpression, newExpression) => _control.Refresh());
				expressionBinding.Watch(property.FindPropertyRelative(nameof(VariableSchemaEntry.Initializer)));

				var defaultBinding = new ChangeTriggerControl<Variable>(null, (oldDefault, newDefault) => _control.Refresh());
				defaultBinding.Watch(property.FindPropertyRelative(nameof(VariableSchemaEntry.Default)));
			}
		}
	}
}
