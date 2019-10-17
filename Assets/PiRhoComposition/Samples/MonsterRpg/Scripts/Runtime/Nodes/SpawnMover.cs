using PiRhoSoft.Composition;
using System.Collections;
using UnityEngine;

namespace PiRhoSoft.MonsterRpg
{
	[CreateGraphNodeMenu("Monster RPG/Spawn Mover", 21)]
	public class SpawnMover : GraphNode
	{
		[Tooltip("The node to move to when this node is finished")]
		public GraphNode Next = null;

		[Tooltip("The mover to spawn")]
		[VariableConstraint(typeof(Mover))]
		public VariableLookupReference Mover = new VariableLookupReference();

		[Tooltip("The map with the spawn point to spawn the player at")]
		public VariableLookupReference Map = new VariableLookupReference();

		[Tooltip("The name of the spawn point to spawn the player at")]
		public StringVariableSource SpawnPoint = new StringVariableSource();
		
		public override Color NodeColor => Colors.Sequencing;

		public override IEnumerator Run(IGraphRunner graph, IVariableMap variables)
		{
			if (variables.ResolveObject<Map>(this, Map, out var map) && variables.ResolveObject<Mover>(this, Mover, out var mover))
			{
				variables.Resolve(this, SpawnPoint, out var spawn);

				var spawnPoint = map.GetSpawnPoint(spawn);
				mover.WarpToPosition(spawnPoint.Position, spawnPoint.Direction);
			}

			graph.GoTo(Next, nameof(Next));

			yield break;
		}
	}
}
