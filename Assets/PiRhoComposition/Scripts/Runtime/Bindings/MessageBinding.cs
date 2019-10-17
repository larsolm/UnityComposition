using UnityEngine;

namespace PiRhoSoft.Composition
{
	[HelpURL(Configuration.DocumentationUrl + "message-binding")]
	[AddComponentMenu("PiRho Composition/Bindings/Message Binding")]
	public class MessageBinding : VariableBinding
	{
		[Tooltip("The message to resolve and display as text in this object")]
		public Message Message;

		protected override void UpdateBinding(IVariableMap variables, BindingAnimationStatus status)
		{
			var text = Message.GetText(variables, ErrorType != BindingErrorType.Log);
			SetBinding(Variable.String(text), true);
		}
	}
}
