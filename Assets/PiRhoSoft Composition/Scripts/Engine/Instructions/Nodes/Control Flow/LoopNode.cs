using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[CreateInstructionGraphNodeMenu("Control Flow/Loop", 20)]
	[HelpURL(Composition.DocumentationUrl + "loop-node")]
	public class LoopNode : InstructionGraphNode, IImmediate, ILoopNode
	{
		[Tooltip("The statement to execute to check if the loop should continue")]
		public Expression Condition = new Expression();

		[Tooltip("The node to repeatedly go to while Condition evaluates to true")]
		public InstructionGraphNode Loop = null;

		[Tooltip("The node to go to when Condition evaluates to false")]
		public InstructionGraphNode Next = null;

		public (InstructionGraphNode Node, string Name) GetBreakNode()
		{
			return (Next, nameof(Next));
		}

		public override void GetInputs(List<VariableDefinition> inputs)
		{
			Condition.GetInputs(inputs, InstructionStore.InputStoreName);
		}

		protected override IEnumerator Run_(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			var condition = Condition.Execute(variables, VariableType.Boolean);

			if (condition.Boolean && Loop != null)
				graph.GoTo(Loop, variables.This, nameof(Loop));
			else
				graph.Break();

			yield break;
		}
	}
}
