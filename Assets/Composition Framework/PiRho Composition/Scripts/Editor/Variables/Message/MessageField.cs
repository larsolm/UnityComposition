using PiRhoSoft.Utilities.Editor;
using UnityEngine.UIElements;

namespace PiRhoSoft.Composition.Editor
{
	public class MessageField : BaseField<string>
	{
		public static readonly string UssClassName = "pirho-message-field";
		public static readonly string LabelUssClassName = UssClassName + "__label";
		public static readonly string InputUssClassName = UssClassName + "__input";

		private MessageControl _control;

		public MessageField(string label, Message value, IAutocompleteItem autocomplete) : base(label, null)
		{
			Setup(value, autocomplete);
		}

		private void Setup(Message value, IAutocompleteItem autocomplete)
		{
			_control = new MessageControl(value, autocomplete);
			_control.AddToClassList(InputUssClassName);
			_control.RegisterCallback<ChangeEvent<string>>(evt => base.value = evt.newValue);

			labelElement.AddToClassList(LabelUssClassName);

			this.SetVisualInput(_control);
			AddToClassList(UssClassName);
			SetValueWithoutNotify(value.Text);
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
				var textProperty = property.FindPropertyRelative(nameof(Message.Text));
				bindingPath = textProperty.propertyPath;
			}
		}
	}
}
