using PiRhoSoft.Composition;
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
		[VariableReference(typeof(Controller))]
		public VariableLookupReference Controller = new VariableLookupReference();

		public override Color NodeColor => Colors.SequencingDark;

		public override IEnumerator Run(IGraphRunner graph, IVariableCollection variables)
		{
			if (variables.ResolveObject<Controller>(this, Controller, out var controller))
				controller.Freeze();

			graph.GoTo(Next, nameof(Next));

			yield break;
		}
	}
}
