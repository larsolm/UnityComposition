using PiRhoSoft.Composition;
using PiRhoSoft.Utilities;
using System.Collections;
using UnityEngine;

namespace PiRhoSoft.MonsterRpg
{
	[CreateGraphNodeMenu("General/Load Header", order: 1)]
	public class LoadHeader : GraphNode
	{
		[Tooltip("The node to move to when this node is finished")]
		public GraphNode Next = null;

		[Tooltip("The filename to load")]
		public StringVariableSource Filename = new StringVariableSource();

		[Tooltip("The object to load the variables into")]
		public VariableSetComponentSource Header = new VariableSetComponentSource();

		public override Color NodeColor => Colors.ExecutionLight;

		public override IEnumerator Run(Graph graph, GraphStore variables, int iteration)
		{
			if (Resolve(variables, Filename, out var filename) && ResolveObject(variables, Header, out var header))
			{
				var info = WorldLoader.Instance.LoadHeader(filename, header.SchemaVariables);
				info.OnError += error => Debug.LogError(error, this);

				while (info.State == LoadState.Loading)
					yield return null;
			}

			graph.GoTo(Next, nameof(Next));

			yield break;
		}
	}
}
