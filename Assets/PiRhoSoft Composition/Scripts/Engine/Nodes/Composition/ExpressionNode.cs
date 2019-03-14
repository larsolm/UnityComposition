using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[CreateInstructionGraphNodeMenu("Composition/Expression", 0)]
	[HelpURL(Composition.DocumentationUrl + "expression-node")]
	public class ExpressionNode : InstructionGraphNode
	{
		[Tooltip("The node to move to when this node is finished")]
		public InstructionGraphNode Next = null;

		[Tooltip("The expression to execute")]
		public Expression Expression = new Expression();

		public override Color NodeColor => Colors.ExecutionDark;

		public override void GetInputs(List<VariableDefinition> inputs)
		{
			Expression.GetInputs(inputs, InstructionStore.InputStoreName);
		}

		public override void GetOutputs(List<VariableDefinition> outputs)
		{
			Expression.GetOutputs(outputs, InstructionStore.OutputStoreName);
		}

		public override IEnumerator Run(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			Expression.Execute(this, variables);
			graph.GoTo(Next, nameof(Next));
			yield break;
		}
	}
}
