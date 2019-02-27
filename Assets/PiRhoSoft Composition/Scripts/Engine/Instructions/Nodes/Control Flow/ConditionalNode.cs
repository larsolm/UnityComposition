using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[CreateInstructionGraphNodeMenu("Control Flow/Conditional", 0)]
	public class ConditionalNode : InstructionGraphNode, IImmediate
	{
		[Tooltip("The node to follow if Condition is true")]
		public InstructionGraphNode OnTrue = null;

		[Tooltip("The node to follow if Condition is false")]
		public InstructionGraphNode OnFalse = null;

		[Tooltip("The expression to evaluate to determine which node to follow")]
		public Expression Condition = new Expression();

		public override Color GetNodeColor()
		{
			return new Color(0.2f, 0.1f, 0.1f);
		}

		public override void GetInputs(List<VariableDefinition> inputs)
		{
			Condition.GetInputs(inputs, InstructionStore.InputStoreName);
		}

		protected override IEnumerator Run_(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			var condition = Condition.Execute(variables, VariableType.Boolean).Boolean;

			if (condition)
				graph.GoTo(OnTrue, variables.This, nameof(OnTrue));
			else
				graph.GoTo(OnFalse, variables.This, nameof(OnFalse));

			yield break;
		}
	}
}
