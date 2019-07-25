using PiRhoSoft.Composition;
using System.Collections;
using UnityEngine;

namespace PiRhoSoft.MonsterRpg
{
	[CreateGraphNodeMenu("Object Manipulation/Thaw Controller", 111)]
	public class ThawController : GraphNode
	{
		[Tooltip("The node to move to when this node is finished")]
		public GraphNode Next = null;

		[Tooltip("The controller to thaw")]
		[VariableReference(typeof(Controller))]
		public VariableReference Controller = new VariableReference();

		public override Color NodeColor => Colors.SequencingLight;

		public override IEnumerator Run(Graph graph, GraphStore variables, int iteration)
		{
			if (ResolveObject<Controller>(variables, Controller, out var controller))
				controller.Thaw();

			graph.GoTo(Next, nameof(Next));

			yield break;
		}
	}
}
