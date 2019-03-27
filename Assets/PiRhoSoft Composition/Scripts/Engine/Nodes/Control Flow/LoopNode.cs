using System.Collections;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[CreateInstructionGraphNodeMenu("Control Flow/Loop", 20)]
	[HelpURL(Composition.DocumentationUrl + "loop-node")]
	public class LoopNode : InstructionGraphNode, ILoopNode
	{
		[Tooltip("The node to repeatedly go to while Condition evaluates to true")]
		public InstructionGraphNode Loop = null;

		[Tooltip("The loop will continue to execute while this expression is true")]
		public Expression Condition = new Expression();

		public override Color NodeColor => Colors.Loop;

		public override IEnumerator Run(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			var condition = Condition.Execute(this, variables, VariableType.Bool);

			if (condition.Bool && Loop != null)
				graph.GoTo(Loop, nameof(Loop));

			yield break;
		}
	}
}
