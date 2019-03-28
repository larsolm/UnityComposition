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

		[Tooltip("The variable that will hold the number of times the loop has run")]
		public VariableReference Index = new VariableReference();

		public override Color NodeColor => Colors.Loop;

		public override IEnumerator Run(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			var condition = Condition.Execute(this, variables, VariableType.Bool);

			if (condition.Bool && Loop != null)
			{
				if (Index.IsAssigned)
					Index.SetValue(variables, VariableValue.Create(iteration));

				graph.GoTo(Loop, nameof(Loop));
			}

			yield break;
		}
	}
}
