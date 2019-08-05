using PiRhoSoft.Composition;
using System.Collections;
using UnityEngine;

namespace PiRhoSoft.MonsterRpg
{
	[CreateGraphNodeMenu("Sequencing/Spawn Player", 21)]
	public class SpawnPlayer : GraphNode
	{
		[Tooltip("The node to move to when this node is finished")]
		public GraphNode Next = null;
		
		[Tooltip("The spawn point to spawn the player at")]
		public StringVariableSource SpawnPoint = new StringVariableSource();
		
		public override Color NodeColor => Colors.Sequencing;

		public override IEnumerator Run(IGraphRunner graph, IVariableCollection variables)
		{
			Resolve(variables, SpawnPoint, out var spawn);

			var spawnPoint = WorldManager.Instance.GetSpawnPoint(spawn);
			Player.Instance.Mover.WarpToPosition(spawnPoint.Position, spawnPoint.Direction, spawnPoint.Layer);

			graph.GoTo(Next, nameof(Next));

			yield break;
		}
	}
}
