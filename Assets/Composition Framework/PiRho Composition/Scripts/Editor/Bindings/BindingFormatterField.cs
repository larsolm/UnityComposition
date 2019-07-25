using PiRhoSoft.Utilities.Editor;
using UnityEngine.UIElements;

namespace PiRhoSoft.Composition.Editor
{
	public class BindingFormatterField : BaseField<BindingFormatter>
	{
		public static readonly string UssClassName = "pirho-binding-formatter-field";
		public static readonly string LabelUssClassName = UssClassName + "__label";
		public static readonly string InputUssClassName = UssClassName + "__input";

		private BindingFormatterControl _control;

		public BindingFormatterField(string label, BindingFormatter value) : base(label, null)
		{
			Setup(value);
		}

		private void Setup(BindingFormatter value)
		{
			_control = new BindingFormatterControl(value);
			_control.AddToClassList(InputUssClassName);
			_control.RegisterCallback<ChangeEvent<BindingFormatter>>(evt => base.value = evt.newValue);

			labelElement.AddToClassList(LabelUssClassName);

			this.SetVisualInput(_control);
			AddToClassList(UssClassName);
			SetValueWithoutNotify(value);
		}

		public override void SetValueWithoutNotify(BindingFormatter newValue)
		{
			base.SetValueWithoutNotify(newValue);
			_control.SetValueWithoutNotify(newValue);
		}
	}
}
