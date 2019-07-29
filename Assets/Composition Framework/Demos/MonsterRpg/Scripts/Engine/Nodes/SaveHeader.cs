using PiRhoSoft.Composition;
using System.Collections;
using UnityEngine;

namespace PiRhoSoft.MonsterRpg
{
	[CreateGraphNodeMenu("General/Save Header", order: 2)]
	public class SaveHeader : GraphNode
	{
		[Tooltip("The node to move to when this node is finished")]
		public GraphNode Next = null;

		[Tooltip("The name of the file to save")]
		public StringVariableSource Filename = new StringVariableSource();

		[Tooltip("The object that contains the variables to save")]
		public VariableSetComponentSource Header = new VariableSetComponentSource();

		public override Color NodeColor => Colors.ExecutionLight;

		public override IEnumerator Run(Graph graph, GraphStore variables, int iteration)
		{
			if (Resolve(variables, Filename, out var filename) && ResolveObject(variables, Header, out var header))
			{
				var info = WorldLoader.Instance.SaveHeader(filename, header.SchemaVariables);
				info.OnError += error => Debug.LogError(error, this);

				while (info.State == SaveState.Saving)
					yield return null;
			}

			graph.GoTo(Next, nameof(Next));

			yield break;
		}
	}
}
