using PiRhoSoft.Composition;
using System.Collections;
using UnityEngine;

namespace PiRhoSoft.MonsterRpg
{
	[CreateGraphNodeMenu("General/Save Game", order: 3)]
	public class SaveGame : GraphNode
	{
		[Tooltip("The node to move to when this node is finished")]
		public GraphNode Next = null;

		public override Color NodeColor => Colors.ExecutionLight;

		public override IEnumerator Run(IGraphRunner graph, IVariableCollection variables)
		{
			var info = WorldLoader.Instance.Save();
			info.OnError += error => Debug.LogError(error, this);

			while (info.State == SaveState.Saving)
				yield return null;

			graph.GoTo(Next, nameof(Next));
		}
	}
}
