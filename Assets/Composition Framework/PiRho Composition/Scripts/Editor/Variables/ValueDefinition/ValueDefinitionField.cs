using PiRhoSoft.Utilities.Editor;
using UnityEngine.UIElements;

namespace PiRhoSoft.Composition.Editor
{
	public class ValueDefinitionField : BaseField<ValueDefinition>
	{
		public static readonly string UssClassName = "pirho-value-definition-field";
		public static readonly string LabelUssClassName = UssClassName + "__label";
		public static readonly string InputUssClassName = UssClassName + "__input";

		private ValueDefinitionControl _control;

		public ValueDefinitionField(string label, ValueDefinition value, VariableInitializerType initializer, TagList tags, bool showConstraintLabel) : base(label, null)
		{
			Setup(value, initializer, tags, showConstraintLabel);
		}

		private void Setup(ValueDefinition value, VariableInitializerType initializer, TagList tags, bool showConstraintLabel)
		{
			_control = new ValueDefinitionControl(value, initializer, tags, showConstraintLabel);
			_control.AddToClassList(InputUssClassName);
			_control.RegisterCallback<ChangeEvent<ValueDefinition>>(evt => base.value = evt.newValue);

			labelElement.AddToClassList(LabelUssClassName);

			this.SetVisualInput(_control);
			AddToClassList(UssClassName);
			SetValueWithoutNotify(value);
		}

		public override void SetValueWithoutNotify(ValueDefinition newValue)
		{
			base.SetValueWithoutNotify(newValue);
			_control.SetValueWithoutNotify(newValue);
		}
	}
}
