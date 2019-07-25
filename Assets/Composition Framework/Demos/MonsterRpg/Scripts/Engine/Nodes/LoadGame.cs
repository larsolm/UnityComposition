using PiRhoSoft.Composition;
using System.Collections;
using UnityEngine;

namespace PiRhoSoft.MonsterRpg
{
	[CreateGraphNodeMenu("General/Load Game", order: 0)]
	public class LoadGame : GraphNode
	{
		[Tooltip("The node to move to when this node is finished")]
		public GraphNode Next = null;

		[Tooltip("The filename to load")]
		public StringVariableSource Filename = new StringVariableSource();

		[Tooltip("The header variables to load")]
		public VariableSetComponentSource Header = new VariableSetComponentSource();

		[Tooltip("The zone to load if it is a new game")]
		public ZoneVariableSource DefaultZone = new ZoneVariableSource();

		[Tooltip("The spawn to spawn at if it is a new game")]
		public StringVariableSource DefaultSpawn = new StringVariableSource();

		[Tooltip("The variable to assign the loaded starting zone to")]
		[VariableReference(typeof(Zone))]
		public VariableReference StartingZone = new VariableReference();

		public override Color NodeColor => Colors.ExecutionLight;

		public override IEnumerator Run(Graph graph, GraphStore variables, int iteration)
		{
			if (Resolve(variables, Filename, out var filename) && ResolveObject(variables, Header, out var header) && ResolveObject(variables, DefaultZone, out var zone) && Resolve(variables, DefaultSpawn, out var spawn))
			{
				var info = WorldLoader.Instance.Load(header.Variables, zone.name, spawn, filename);
				info.OnError += error => Debug.LogError(error, this);

				while (info.State == LoadState.Loading)
					yield return null;

				Assign(variables, StartingZone, Variable.Object(info.StartingZone));
			}

			graph.GoTo(Next, nameof(Next));
		}
	}
}
