using System.Collections;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	[HelpURL(Composition.DocumentationUrl + "log-node")]
	[CreateGraphNodeMenu("Debug/Log", 400)]
	public class LogNode : GraphNode
	{
		[Tooltip("The message to log to the console when this node is traversed")]
		public Message Message = new Message();

		[Tooltip("The node to go to after logging the message")]
		public GraphNode Next = null;

		public override IEnumerator Run(Graph graph, GraphStore variables, int iteration)
		{
			var text = Message.GetText(variables, false);
			Debug.Log(text);
			graph.GoTo(Next, nameof(Next));
			yield break;
		}
	}
}
