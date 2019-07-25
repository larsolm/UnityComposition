using System.Collections;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	[CreateGraphNodeMenu("Control Flow/Loop", 20)]
	[HelpURL(Composition.DocumentationUrl + "loop-node")]
	public class LoopNode : GraphNode, ILoopNode
	{
		[Tooltip("The node to repeatedly go to while Condition evaluates to true")]
		public GraphNode Loop = null;

		[Tooltip("The variable that will hold the number of times the loop has run")]
		public VariableReference Index = new VariableReference();

		[Tooltip("The loop will continue to execute while this expression is true")]
		public Expression Condition = new Expression();

		public override Color NodeColor => Colors.Loop;

		public override IEnumerator Run(Graph graph, GraphStore variables, int iteration)
		{
			var condition = Condition.Execute(this, variables, VariableType.Bool);

			if (condition.AsBool && Loop != null)
			{
				if (Index.IsAssigned)
					Index.SetValue(variables, Variable.Int(iteration));

				graph.GoTo(Loop, nameof(Loop));
			}

			yield break;
		}
	}
}
