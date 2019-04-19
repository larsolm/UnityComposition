using System.Collections;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[HelpURL(Composition.DocumentationUrl + "log-node")]
	[CreateInstructionGraphNodeMenu("Debug/Log", 401)]
	public class LogNode : InstructionGraphNode
	{
		[Tooltip("The message to log to the console when this node is traversed")]
		public Message Message = new Message();

		[Tooltip("The node to go to after logging the message")]
		public InstructionGraphNode Next = null;

		public override IEnumerator Run(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			var text = Message.GetText(variables, false);
			Debug.Log(text);
			graph.GoTo(Next, nameof(Next));
			yield break;
		}
	}
}
