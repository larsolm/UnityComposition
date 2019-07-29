using PiRhoSoft.Utilities;
using System;
using System.Collections;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	[Serializable]
	public class ResetVariableList : SerializedList<string> { }

	[CreateGraphNodeMenu("Composition/Reset Variables Node", 30)]
	[HelpURL(Configuration.DocumentationUrl + "reset-variables-node")]
	public class ResetVariablesNode : GraphNode
	{
		[Tooltip("The node to move to when this node is finished")]
		public GraphNode Next = null;

		[Tooltip("The object containing the variables to reset")]
		public VariableReference Object;

		[Tooltip("The list of variables to reset")]
		[List(EmptyLabel = "No variables will be reset")]
		public ResetVariableList Variables = new ResetVariableList();

		public override Color NodeColor => Colors.ExecutionDark;

		public override IEnumerator Run(Graph graph, GraphStore variables, int iteration)
		{
			if (ResolveInterface(variables, Object, out IResettableVariables reset))
				reset.ResetVariables(Variables);

			graph.GoTo(Next, nameof(Next));
			yield break;
		}
	}
}
