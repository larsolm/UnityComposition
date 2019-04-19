using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[HelpURL(Composition.DocumentationUrl + "message-binding")]
	[AddComponentMenu("PiRho Soft/Bindings/Message Binding")]
	public class MessageBinding : StringBinding
	{
		private const string _missingVariableWarning = "(CBTBMV) Unable to bind text for text binding '{0}': the variable '{1}' could not be found";

		[Tooltip("The message to resolve and display as text in this object")]
		public Message Message;

		protected override void UpdateBinding(IVariableStore variables, BindingAnimationStatus status)
		{
			var text = Message.GetText(variables, SuppressErrors);

			SetText(text, true);
		}
	}
}
