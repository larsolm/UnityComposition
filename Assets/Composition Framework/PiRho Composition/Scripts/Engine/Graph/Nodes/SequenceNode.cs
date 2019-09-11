using PiRhoSoft.Utilities;
using System.Collections;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	[CreateGraphNodeMenu("Control Flow/Sequence", 10)]
	[OutputListNode(nameof(Sequence))]
	[HelpURL(Configuration.DocumentationUrl + "sequence-node")]
	public class SequenceNode : GraphNode
	{
		[Tooltip("The nodes to visit in order")]
		[List]
		public GraphNodeList Sequence = new GraphNodeList();

		public override Color NodeColor => Colors.Sequence;

		public override IEnumerator Run(IGraphRunner graph, IVariableCollection variables)
		{
			for (var i = 0; i < Sequence.Count; i++)
			{
				var node = Sequence[i];

				if (node != null)
					yield return graph.Run(node, variables, GetConnectionName(nameof(Sequence), i));
			}
		}
	}
}