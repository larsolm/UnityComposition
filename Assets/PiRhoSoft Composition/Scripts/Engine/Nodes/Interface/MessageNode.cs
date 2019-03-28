using PiRhoSoft.UtilityEngine;
using System.Collections;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[HelpURL(Composition.DocumentationUrl + "message-node")]
	[CreateInstructionGraphNodeMenu("Interface/Show Message", 1)]
	public class MessageNode : InstructionGraphNode
	{
		[Tooltip("The node to follow when the message is dismissed")]
		public InstructionGraphNode Next = null;

		[Tooltip("The MessageControl to show")]
		[VariableConstraint(typeof(MessageControl))]
		public VariableReference Control = new VariableReference();

		[Tooltip("Specifies whether to wait for the message to complete before moving on to Next")]
		public bool WaitForCompletion = true;

		[Tooltip("Specifies whether to automatically hide the message when it is complete")]
		public bool AutoHide = false;

		[Tooltip("How long to wait after message is complete before hiding")]
		[ConditionalDisplaySelf(nameof(AutoHide))]
		public float WaitTime = 0.0f;

		[Tooltip("The message to display when this node is traversed")]
		public Message Message = new Message();

		public override Color NodeColor => Colors.InterfaceCyan;

		public override IEnumerator Run(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			var text = Message.GetText(variables);

			if (ResolveObject(variables, Control, out MessageControl message))
				yield return Show(message, text);
			else
				Debug.Log(text);

			graph.GoTo(Next, nameof(Next));
		}

		private IEnumerator Show(MessageControl message, string text)
		{
			message.Show(text);

			if (WaitForCompletion)
			{
				while (message.IsRunning)
					yield return null;
			}

			if (AutoHide)
			{
				if (WaitTime > 0.0f)
					yield return new WaitForSeconds(WaitTime);

				message.Deactivate();
			}
		}
	}
}
