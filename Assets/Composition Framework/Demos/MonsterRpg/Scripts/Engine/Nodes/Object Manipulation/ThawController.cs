using PiRhoSoft.Composition;
using System.Collections;
using UnityEngine;

namespace PiRhoSoft.MonsterRpg
{
	[CreateGraphNodeMenu("Monster RPG/Thaw Controller", 111)]
	public class ThawController : GraphNode
	{
		[Tooltip("The node to move to when this node is finished")]
		public GraphNode Next = null;

		[Tooltip("The controller to thaw")]
		[VariableReference(typeof(Controller))]
		public VariableLookupReference Controller = new VariableLookupReference();

		public override Color NodeColor => Colors.SequencingLight;

		public override IEnumerator Run(IGraphRunner graph, IVariableCollection variables)
		{
			if (variables.ResolveObject<Controller>(this, Controller, out var controller))
				controller.Thaw();

			graph.GoTo(Next, nameof(Next));

			yield break;
		}
	}
}
