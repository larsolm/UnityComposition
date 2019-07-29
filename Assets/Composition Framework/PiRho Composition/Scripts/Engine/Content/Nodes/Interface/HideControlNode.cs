using System.Collections;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	[HelpURL(Configuration.DocumentationUrl + "hide-control-node")]
	[CreateGraphNodeMenu("Interface/Hide Control", 102)]
	public class HideControlNode : GraphNode
	{
		[Tooltip("The node to go to once the control is hidden")]
		public GraphNode Next = null;

		[Tooltip("The InterfaceControl to hide")]
		[VariableReference(typeof(InterfaceControl))]
		public VariableReference Control = new VariableReference();

		public override Color NodeColor => Colors.InterfaceDark;

		public override IEnumerator Run(Graph graph, GraphStore variables, int iteration)
		{
			if (ResolveObject(variables, Control, out InterfaceControl control))
				control.Deactivate();

			graph.GoTo(Next, nameof(Next));

			yield break;
		}
	}
}
