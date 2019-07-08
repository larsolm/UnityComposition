using System.Collections;
using UnityEngine;

namespace PiRhoSoft.Composition.Engine
{
	[HelpURL(Composition.DocumentationUrl + "show-control-node")]
	[CreateGraphNodeMenu("Interface/Show Control", 101)]
	public class ShowControlNode : GraphNode
	{
		[Tooltip("The node to go to once the control is shown")]
		public GraphNode Next = null;

		[Tooltip("The InterfaceControl to show")]
		[VariableConstraint(typeof(InterfaceControl))]
		public VariableReference Control = new VariableReference();

		public override Color NodeColor => Colors.InterfaceLight;

		public override IEnumerator Run(Graph graph, GraphStore variables, int iteration)
		{
			if (ResolveObject(variables, Control, out InterfaceControl control))
				control.Activate();

			graph.GoTo(Next, nameof(Next));

			yield break;
		}
	}
}
