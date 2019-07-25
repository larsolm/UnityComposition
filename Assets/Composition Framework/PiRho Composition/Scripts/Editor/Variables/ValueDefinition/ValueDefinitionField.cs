using PiRhoSoft.Utilities.Editor;
using UnityEngine.UIElements;

namespace PiRhoSoft.Composition.Editor
{
	public class ValueDefinitionField : BaseField<VariableDefinition>
	{
		public static readonly string UssClassName = "pirho-value-definition-field";
		public static readonly string LabelUssClassName = UssClassName + "__label";
		public static readonly string InputUssClassName = UssClassName + "__input";

		private VariableDefinitionControl _control;

		public ValueDefinitionField(string label, VariableDefinition value, VariableInitializerType initializer, TagList tags, bool showConstraintLabel) : base(label, null)
		{
			Setup(value, initializer, tags, showConstraintLabel);
		}

		private void Setup(VariableDefinition value, VariableInitializerType initializer, TagList tags, bool showConstraintLabel)
		{
			_control = new VariableDefinitionControl(value, initializer, tags, showConstraintLabel);
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
	}
}
