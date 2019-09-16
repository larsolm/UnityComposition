using PiRhoSoft.Utilities.Editor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Composition.Editor
{
	public class VariableValueField : BaseField<VariableValue>
	{
		public static readonly string UssClassName = "pirho-variable-field";
		public static readonly string LabelUssClassName = UssClassName + "__label";
		public static readonly string InputUssClassName = UssClassName + "__input";

		private VariableControl _control;

		public VariableValueField(string label, VariableValue value, Object owner) : base(label, null)
		{
			Setup(value, owner);
		}

		private void Setup(VariableValue value, Object owner)
		{
			_control = new VariableControl(value.Variable, new VariableDefinition(), owner);
			_control.AddToClassList(InputUssClassName);
			_control.RegisterCallback<ChangeEvent<Variable>>(evt => { base.value.Variable = evt.newValue; this.SendChangeEvent(value, value); });

			labelElement.AddToClassList(LabelUssClassName);

			this.SetVisualInput(_control);
			AddToClassList(UssClassName);
			SetValueWithoutNotify(value);
		}

		public override void SetValueWithoutNotify(VariableValue newValue)
		{
			base.SetValueWithoutNotify(newValue);
			_control.SetValueWithoutNotify(newValue.Variable);
		}
	}
}
