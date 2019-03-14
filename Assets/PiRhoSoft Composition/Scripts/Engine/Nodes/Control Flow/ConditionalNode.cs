using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[CreateInstructionGraphNodeMenu("Control Flow/Conditional", 0)]
	[HelpURL(Composition.DocumentationUrl + "conditional-node")]
	public class ConditionalNode : InstructionGraphNode
	{
		[Tooltip("The node to follow if Condition is true")]
		public InstructionGraphNode OnTrue = null;

		[Tooltip("The node to follow if Condition is false")]
		public InstructionGraphNode OnFalse = null;

		[Tooltip("The expression to evaluate to determine which node to follow")]
		public Expression Condition = new Expression();

		public override Color NodeColor => Colors.Branch;

		public override void GetInputs(List<VariableDefinition> inputs)
		{
			Condition.GetInputs(inputs, InstructionStore.InputStoreName);
		}

		public override void GetOutputs(List<VariableDefinition> outputs)
		{
			Condition.GetOutputs(outputs, InstructionStore.OutputStoreName);
		}

		public override IEnumerator Run(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			var condition = Condition.Execute(this, variables, VariableType.Boolean).Boolean;

			if (condition)
				graph.GoTo(OnTrue, nameof(OnTrue));
			else
				graph.GoTo(OnFalse, nameof(OnFalse));

			yield break;
		}
	}
}
