using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[HelpURL(Composition.DocumentationUrl + "message-node")]
	[CreateInstructionGraphNodeMenu("Interface/Message", 1)]
	public class MessageNode : InstructionGraphNode
	{
		[Tooltip("The node to follow when the message is dismissed")]
		public InstructionGraphNode Next = null;

		[Tooltip("The control to display the message in")]
		public InterfaceReference Control = new InterfaceReference();

		[Tooltip("Controls when the interface waits for interaction from the user")]
		public MessageInteractionType Interaction = MessageInteractionType.WaitForInput;

		[Tooltip("If true the interface will consider this message the last in a sequence, altering the way the interaction cues are displayed")]
		public bool IsLast = true;

		[Tooltip("The message to display when this node is traversed")]
		public Message Message = new Message();

		public override Color GetNodeColor()
		{
			return new Color(0.0f, 0.3f, 0.5f);
		}

		public override void GetInputs(List<VariableDefinition> inputs)
		{
			Message.GetInputs(inputs);
		}

		protected override IEnumerator Run_(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			var control = Control.Activate<MessageControl>(this);
			var text = Message.GetText(variables);

			if (control != null)
				yield return control.Show(variables, text, Interaction, IsLast);
			else
				Debug.Log(text);

			Control.Deactivate(this);

			graph.GoTo(Next, variables.This, nameof(Next));
		}
	}
}
