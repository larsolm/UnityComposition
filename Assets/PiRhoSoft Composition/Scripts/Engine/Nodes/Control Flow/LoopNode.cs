using System.Collections;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[CreateInstructionGraphNodeMenu("Control Flow/Loop", 20)]
	[HelpURL(Composition.DocumentationUrl + "loop-node")]
	public class LoopNode : InstructionGraphNode, ILoopNode
	{
		[Tooltip("The statement to execute to check if the loop should continue")]
		public Expression Condition = new Expression();

		[Tooltip("The node to repeatedly go to while Condition evaluates to true")]
		public InstructionGraphNode Loop = null;

		public override Color NodeColor => Colors.Loop;

		public override IEnumerator Run(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			var condition = Condition.Execute(this, variables, VariableType.Boolean);

			if (condition.Boolean && Loop != null)
				graph.GoTo(Loop, nameof(Loop));

			yield break;
		}
	}
}
