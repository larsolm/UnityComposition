using PiRhoSoft.Utilities;
using System.Collections;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	[CreateGraphNodeMenu("Control Flow/Branch", 1)]
	[HelpURL(Configuration.DocumentationUrl + "branch-node")]
	public class BranchNode : GraphNode
	{
		[Tooltip("The expression to evaluate to determine which node to follow")]
		public Expression Switch = new Expression();

		[Tooltip("The node to follow depending on the result of Switch")]
		[Dictionary]
		public GraphNodeDictionary Outputs = new GraphNodeDictionary();

		[Tooltip("The node to follow if the result of Switch is not in Outputs")]
		public GraphNode Default;

		public override Color NodeColor => Colors.Branch;

		public override IEnumerator Run(Graph graph, GraphStore variables, int iteration)
		{
			var value = Switch.Execute(this, variables, VariableType.String);

			if (value.TryGetString(out var name))
			{
				if (Outputs.TryGetValue(name, out var output))
					graph.GoTo(output, nameof(Outputs), name);
				else
					graph.GoTo(Default, nameof(Default));
			}

			yield break;
		}
	}
}
