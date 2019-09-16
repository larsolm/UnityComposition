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
		public VariableLookupReference Control = new VariableLookupReference();

		public override Color NodeColor => Colors.InterfaceDark;

		public override IEnumerator Run(IGraphRunner graph, IVariableCollection variables)
		{
			if (variables.ResolveObject(this, Control, out InterfaceControl control))
				control.Deactivate();

			graph.GoTo(Next, nameof(Next));

			yield break;
		}
	}
}
