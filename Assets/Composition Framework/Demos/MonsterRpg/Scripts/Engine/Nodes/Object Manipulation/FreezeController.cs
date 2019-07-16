﻿using PiRhoSoft.Composition;
using System.Collections;
using UnityEngine;

namespace PiRhoSoft.MonsterRpg
{
	[CreateGraphNodeMenu("Object Manipulation/Freeze Controller", 110)]
	public class FreezeController : GraphNode
	{
		[Tooltip("The node to move to when this node is finished")]
		public GraphNode Next = null;

		[Tooltip("The controller to thaw")]
		[VariableConstraint(typeof(Controller))]
		public VariableReference Controller = new VariableReference();

		public override Color NodeColor => Colors.SequencingDark;

		public override IEnumerator Run(Graph graph, GraphStore variables, int iteration)
		{
			if (ResolveObject<Controller>(variables, Controller, out var controller))
				controller.Freeze();

			graph.GoTo(Next, nameof(Next));

			yield break;
		}
	}
}