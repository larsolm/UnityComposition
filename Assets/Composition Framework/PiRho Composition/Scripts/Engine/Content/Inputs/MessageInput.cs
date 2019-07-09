using PiRhoSoft.Utilities.Engine;
using UnityEngine;

namespace PiRhoSoft.Composition.Engine
{
	[HelpURL(Composition.DocumentationUrl + "message-input")]
	[AddComponentMenu("PiRho Soft/Interface/Message Input")]
	[RequireComponent(typeof(MessageControl))]
	public class MessageInput : MonoBehaviour
	{
		[Tooltip("The input button to use to advance the message")]
		public string AcceptButton = "Submit";

		protected MessageControl Message { get; private set; }

		protected virtual void Awake()
		{
			Message = GetComponent<MessageControl>();
		}

		protected virtual void Update()
		{
			if (InputHelper.GetWasButtonPressed(AcceptButton))
				Message.Advance();
		}
	}
}
