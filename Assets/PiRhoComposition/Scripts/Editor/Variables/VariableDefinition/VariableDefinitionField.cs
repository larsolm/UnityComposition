using PiRhoSoft.Utilities.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Composition.Editor
{
	public class VariableDefinitionField : BaseField<VariableDefinition>
	{
		public static readonly string UssClassName = "pirho-variable-definition-field";
		public static readonly string LabelUssClassName = UssClassName + "__label";
		public static readonly string InputUssClassName = UssClassName + "__input";

		private VariableDefinitionControl _control;

		public VariableDefinitionField(string label) : base(label, null)
		{
		}

		public void Setup(SerializedProperty property)
		{
			var definition = property.GetObject<VariableDefinition>();
			var owner = property.serializedObject.targetObject;

			// add watchers for Name, _type, and _constraintData for binding changes from code back to the control

			Setup(definition, owner);
		}

		public void Setup(VariableDefinition value, Object owner)
		{
			_control = new VariableDefinitionControl(value, owner);
			_control.AddToClassList(InputUssClassName);
			_control.RegisterCallback<ChangeEvent<VariableDefinition>>(evt => base.value = evt.newValue);

			labelElement.AddToClassList(LabelUssClassName);

			this.SetVisualInput(_control);
			AddToClassList(UssClassName);
			SetValueWithoutNotify(value);
		}

		public override void SetValueWithoutNotify(VariableDefinition newValue)
		{
			base.SetValueWithoutNotify(newValue);
			_control.SetValueWithoutNotify(newValue);
		}

		protected override void ExecuteDefaultActionAtTarget(EventBase evt)
		{
			base.ExecuteDefaultActionAtTarget(evt);

			if (this.TryGetPropertyBindEvent(evt, out var property))
			{
				Setup(property);
				evt.StopPropagation();
			}
		}
	}
}
