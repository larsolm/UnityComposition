using PiRhoSoft.Utilities.Editor;
using UnityEngine.UIElements;

namespace PiRhoSoft.Composition.Editor
{
	public class VariableField : BaseField<Variable>
	{
		public static readonly string UssClassName = "pirho-variable-field";
		public static readonly string LabelUssClassName = UssClassName + "__label";
		public static readonly string InputUssClassName = UssClassName + "__input";

		private VariableControl _control;

		public VariableField(string label, Variable value, VariableDefinition definition) : base(label, null)
		{
			Setup(value, definition);
		}

		private void Setup(Variable value, VariableDefinition definition)
		{
			_control = new VariableControl(value, definition);
			_control.AddToClassList(InputUssClassName);
			_control.RegisterCallback<ChangeEvent<Variable>>(evt => base.value = evt.newValue);

			labelElement.AddToClassList(LabelUssClassName);

			this.SetVisualInput(_control);
			AddToClassList(UssClassName);
			SetValueWithoutNotify(value);
		}

		public override void SetValueWithoutNotify(Variable newValue)
		{
			base.SetValueWithoutNotify(newValue);
			_control.SetValueWithoutNotify(newValue);
		}
	}
}
