using PiRhoSoft.UtilityEngine;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[HelpURL(Composition.DocumentationUrl + "message-input")]
	[AddComponentMenu("PiRho Soft/Interface/Message Input")]
	[RequireComponent(typeof(MessageControl))]
	public class MessageInput : MonoBehaviour
	{
		[Tooltip("The input button to use to advance the message")]
		public string AcceptButton = "Submit";

		private MessageControl _message;

		void Awake()
		{
			_message = GetComponent<MessageControl>();
		}

		void Update()
		{
			if (InputHelper.GetWasButtonPressed(AcceptButton))
				_message.Advance();
		}
	}
}
