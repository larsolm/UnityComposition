using PiRhoSoft.Utilities.Editor;
using UnityEngine.UIElements;

namespace PiRhoSoft.Composition.Editor
{
	public class VariableValueField : BaseField<VariableValue>
	{
		public static readonly string UssClassName = "pirho-enum-buttons-field";
		public static readonly string LabelUssClassName = UssClassName + "__label";
		public static readonly string InputUssClassName = UssClassName + "__input";

		private VariableValueControl _control;

		public VariableValueField(string label, VariableValue value, VariableDefinition definition) : base(label, null)
		{
			Setup(value, definition);
		}

		private void Setup(VariableValue value, VariableDefinition definition)
		{
			_control = new VariableValueControl(value, definition);
			_control.AddToClassList(InputUssClassName);
			_control.RegisterCallback<ChangeEvent<VariableValue>>(evt => base.value = evt.newValue);

			labelElement.AddToClassList(LabelUssClassName);

			this.SetVisualInput(_control);
			AddToClassList(UssClassName);
			SetValueWithoutNotify(value);
		}

		public override void SetValueWithoutNotify(VariableValue newValue)
		{
			base.SetValueWithoutNotify(newValue);
			_control.SetValueWithoutNotify(newValue);
		}
	}
}
