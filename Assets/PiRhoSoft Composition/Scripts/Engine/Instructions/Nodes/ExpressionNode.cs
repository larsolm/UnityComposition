using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[CreateInstructionGraphNodeMenu("General/Expression")]
	[HelpURL(Composition.DocumentationUrl + "run-expression")]
	public class ExpressionNode : InstructionGraphNode
	{
		[Tooltip("The node to move to when this node is finished")]
		public InstructionGraphNode Next = null;

		[Tooltip("The expression to execute")]
		public Expression Expression = new Expression();

		public override bool IsExecutionImmediate => true;
		public override InstructionGraphExecutionMode ExecutionMode => InstructionGraphExecutionMode.Normal;

		public override void GetInputs(List<VariableDefinition> inputs)
		{
			Expression.GetInputs(inputs, InstructionStore.InputStoreName);
		}

		public override void GetOutputs(List<VariableDefinition> outputs)
		{
			Expression.GetOutputs(outputs, InstructionStore.OutputStoreName);
		}

		protected override IEnumerator Run_(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			Expression.Execute(variables);
			graph.GoTo(Next);
			yield break;
		}
	}
}
