using System.Collections;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	[CreateGraphNodeMenu("Control Flow/Conditional", 0)]
	[HelpURL(Composition.DocumentationUrl + "conditional-node")]
	public class ConditionalNode : GraphNode
	{
		[Tooltip("The node to follow if Condition is true")]
		public GraphNode OnTrue = null;

		[Tooltip("The node to follow if Condition is false")]
		public GraphNode OnFalse = null;

		[Tooltip("The expression to evaluate to determine which node to follow")]
		public Expression Condition = new Expression();

		public override Color NodeColor => Colors.Branch;

		public override IEnumerator Run(Graph graph, GraphStore variables, int iteration)
		{
			var condition = Condition.Execute(this, variables, VariableType.Bool).Bool;

			if (condition)
				graph.GoTo(OnTrue, nameof(OnTrue));
			else
				graph.GoTo(OnFalse, nameof(OnFalse));

			yield break;
		}
	}
}
