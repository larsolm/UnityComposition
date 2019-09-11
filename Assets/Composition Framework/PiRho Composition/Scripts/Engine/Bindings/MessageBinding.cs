using UnityEngine;

namespace PiRhoSoft.Composition
{
	[HelpURL(Configuration.DocumentationUrl + "message-binding")]
	[AddComponentMenu("PiRho Soft/Bindings/Message Binding")]
	public class MessageBinding : StringBinding
	{
		[Tooltip("The message to resolve and display as text in this object")]
		public Message Message;

		protected override void UpdateBinding(IVariableCollection variables, BindingAnimationStatus status)
		{
			var text = Message.GetText(variables, SuppressErrors);
			SetText(text, true);
		}
	}
}
