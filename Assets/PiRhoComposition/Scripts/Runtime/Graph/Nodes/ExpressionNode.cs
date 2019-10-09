using PiRhoSoft.Utilities;
using System.Collections;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	[CreateGraphNodeMenu("Composition/Expression", 0)]
	[HelpURL(Configuration.DocumentationUrl + "expression-node")]
	public class ExpressionNode : GraphNode
	{
		[Tooltip("The node to move to when this node is finished")]
		public GraphNode Next = null;

		[Tooltip("The expression to execute")]
		[Stretch]
		public Expression Expression = new Expression();

		public override Color NodeColor => Colors.ExecutionDark;

		public override IEnumerator Run(IGraphRunner graph, IVariableCollection variables)
		{
			Expression.Execute(this, variables);
			graph.GoTo(Next, nameof(Next));
			yield break;
		}
	}
}
