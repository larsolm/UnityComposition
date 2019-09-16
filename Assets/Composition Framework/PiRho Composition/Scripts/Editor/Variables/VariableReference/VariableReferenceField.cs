using PiRhoSoft.Utilities;
using PiRhoSoft.Utilities.Editor;
using UnityEngine.UIElements;

namespace PiRhoSoft.Composition.Editor
{
	public class VariableReferenceField : BaseField<VariableReference>
	{
		public static readonly string UssClassName = "pirho-variable-reference-field";
		public static readonly string LabelUssClassName = UssClassName + "__label";
		public static readonly string InputUssClassName = UssClassName + "__input";

		private VariableReferenceControl _control;

		public VariableReferenceField(string label, VariableReference value, IAutocompleteItem autocomplete) : base(label, null)
		{
			Setup(value, autocomplete);
		}

		private void Setup(VariableReference value, IAutocompleteItem autocomplete)
		{
			_control = new VariableReferenceControl(value, autocomplete);
			_control.AddToClassList(InputUssClassName);
			_control.RegisterCallback<ChangeEvent<VariableReference>>(evt => base.value = evt.newValue);

			labelElement.AddToClassList(LabelUssClassName);

			this.SetVisualInput(_control);
			AddToClassList(UssClassName);
			SetValueWithoutNotify(value);
		}

		public override void SetValueWithoutNotify(VariableReference newValue)
		{
			base.SetValueWithoutNotify(newValue);
			_control.SetValueWithoutNotify(newValue);
		}
	}
}
