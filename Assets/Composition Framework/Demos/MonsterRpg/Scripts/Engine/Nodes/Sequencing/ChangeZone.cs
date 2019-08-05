using PiRhoSoft.Composition;
using System.Collections;
using UnityEngine;

namespace PiRhoSoft.MonsterRpg
{
	[CreateGraphNodeMenu("General/Change Zone", order: 0)]
	public class ChangeZone : GraphNode
	{
		[Tooltip("The node to move to when this node is finished")]
		public GraphNode Next = null;

		[Tooltip("The zone to transition to")]
		public ZoneVariableSource Zone = new ZoneVariableSource();

		[Tooltip("Whether to wait for connectioned zones to also load")]
		public BoolVariableSource WaitForConnections = new BoolVariableSource();

		public override Color NodeColor => Colors.Sequencing;

		public override IEnumerator Run(IGraphRunner graph, IVariableCollection variables)
		{
			if (ResolveObject(variables, Zone, out var zone))
			{
				var status = WorldManager.Instance.ChangeZone(zone);

				if (Resolve(variables, WaitForConnections, out var wait) && wait)
				{
					while (!status.IsDone)
						yield return null;
				}
			}

			graph.GoTo(Next, nameof(Next));
		}
	}
}
