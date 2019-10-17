using System.Collections;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	[CreateGraphNodeMenu("Control Flow/Loop", 20)]
	[HelpURL(Configuration.DocumentationUrl + "loop-node")]
	public class LoopNode : GraphNode
	{
		[Tooltip("The node to repeatedly go to while Condition evaluates to true")]
		public GraphNode Loop = null;

		[Tooltip("The variable that will hold the number of times the loop has run")]
		public VariableAssignmentReference Index = new VariableAssignmentReference();

		[Tooltip("The loop will continue to execute while this expression is true")]
		public Expression Condition = new Expression();

		public override Color NodeColor => Colors.Loop;

		public override IEnumerator Run(IGraphRunner graph, IVariableMap variables)
		{
			var index = 0;

			while (true)
			{
				var condition = Condition.Execute(this, variables, VariableType.Bool);

				if (!condition.AsBool || Loop == null)
					break;

				if (Index.IsAssigned)
					Index.SetValue(variables, Variable.Int(index++));

				yield return graph.Run(Loop, variables, nameof(Loop));
			}
		}
	}
}