using TMPro;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(TextMeshProUGUI))]
	[HelpURL(Composition.DocumentationUrl + "message-binding")]
	[AddComponentMenu("PiRho Soft/Interface/Message Binding")]
	public class MessageBinding : VariableBinding
	{
		[Tooltip("The message to resolve and display as text in this object")]
		public Message Message;

		public TextMeshProUGUI Text
		{
			get
			{
				if (!_text)
					_text = GetComponent<TextMeshProUGUI>();

				return _text;
			}
		}

		private TextMeshProUGUI _text;

		protected override void UpdateBinding(IVariableStore variables, BindingAnimationStatus status)
		{
			Text.text = Message.GetText(variables);
		}
	}
}
