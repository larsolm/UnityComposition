using System.Collections;
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
		[ExpressionDisplay(MaximumLines = 20)]
		public Expression Expression = new Expression();

		public override Color NodeColor => Colors.ExecutionDark;

		public override IEnumerator Run(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			Expression.Execute(this, variables);
			graph.GoTo(Next, nameof(Next));
			yield break;
		}
	}
}
