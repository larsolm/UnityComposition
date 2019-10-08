using PiRhoSoft.Utilities.Editor;
using UnityEngine.UIElements;

namespace PiRhoSoft.Composition.Editor
{
	public class VariableReferenceField : BaseField<string>
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
			_control.RegisterCallback<ChangeEvent<string>>(evt => base.value = evt.newValue);

			labelElement.AddToClassList(LabelUssClassName);

			this.SetVisualInput(_control);
			AddToClassList(UssClassName);
			SetValueWithoutNotify(value.Variable);
		}

		public override void SetValueWithoutNotify(string newValue)
		{
			base.SetValueWithoutNotify(newValue);
			_control.SetValueWithoutNotify(newValue);
		}

		// Use this to set the bindingPath so that everywhere this field is used we don't have to step down into the "_variable" property
		protected override void ExecuteDefaultActionAtTarget(EventBase evt)
		{
			base.ExecuteDefaultActionAtTarget(evt);

			if (this.TryGetPropertyBindEvent(evt, out var property))
			{
				var variableProperty = property.FindPropertyRelative("_variable");
				bindingPath = variableProperty.propertyPath;
			}
		}
	}
}
