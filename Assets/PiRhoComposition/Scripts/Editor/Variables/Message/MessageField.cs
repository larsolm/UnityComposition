using PiRhoSoft.Utilities.Editor;
using UnityEditor;
using UnityEngine.UIElements;

namespace PiRhoSoft.Composition.Editor
{
	public class MessageField : BaseField<string>
	{
		public const string UssClassName = "pirho-message-field";
		public const string LabelUssClassName = UssClassName + "__label";
		public const string InputUssClassName = UssClassName + "__input";

		private MessageControl _control;

		public MessageField(SerializedProperty property, IAutocompleteItem autocomplete) : base(property.displayName, null)
		{
			Setup(property.GetObject<Message>(), autocomplete);

			var textProperty = property.FindPropertyRelative(nameof(Message.Text));
			this.ConfigureProperty(textProperty);
			this.SetLabel(property.displayName);
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
	}
}
