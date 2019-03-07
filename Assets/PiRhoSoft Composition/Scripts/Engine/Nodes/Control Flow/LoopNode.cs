using System.Collections;
using System.Collections.Generic;
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

		public override void GetInputs(List<VariableDefinition> inputs)
		{
			Condition.GetInputs(inputs, InstructionStore.InputStoreName);
		}

		public override void GetOutputs(List<VariableDefinition> outputs)
		{
			Condition.GetOutputs(outputs, InstructionStore.OutputStoreName);
		}

		protected override IEnumerator Run_(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			var condition = Condition.Execute(this, variables, VariableType.Boolean);

			if (condition.Boolean && Loop != null)
				graph.GoTo(Loop, variables.This, nameof(Loop));

			yield break;
		}
	}
}
