﻿using System.Collections;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	[HelpURL(Configuration.DocumentationUrl + "show-control-node")]
	[CreateGraphNodeMenu("Interface/Show Control", 101)]
	public class ShowControlNode : GraphNode
	{
		[Tooltip("The node to go to once the control is shown")]
		public GraphNode Next = null;

		[Tooltip("The InterfaceControl to show")]
		[VariableReference(typeof(InterfaceControl))]
		public VariableReference Control = new VariableReference();

		public override Color NodeColor => Colors.InterfaceLight;

		public override IEnumerator Run(IGraphRunner graph, IVariableCollection variables)
		{
			if (variables.ResolveObject(this, Control, out InterfaceControl control))
				control.Activate();

			graph.GoTo(Next, nameof(Next));

			yield break;
		}
	}
}
