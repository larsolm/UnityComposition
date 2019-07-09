using PiRhoSoft.Utilities.Engine;
using System.Collections;
using UnityEngine;

namespace PiRhoSoft.Composition.Engine
{
	[CreateGraphNodeMenu("Control Flow/Sequence", 10)]
	[HelpURL(Composition.DocumentationUrl + "sequence-node")]
	public class SequenceNode : GraphNode, ISequenceNode
	{
		private const string _invalidSequenceError = "(CSQIS) Unable to run sequence for node '{0}': index '{1}' has no connection";

		[Tooltip("The nodes to visit in order")]
		[List]
		public GraphNodeList Sequence = new GraphNodeList();

		public override Color NodeColor => Colors.Sequence;

		public override IEnumerator Run(Graph graph, GraphStore variables, int iteration)
		{
			if (iteration < Sequence.Count)
			{
				if (Sequence[iteration] == null)
					Debug.LogErrorFormat(this, _invalidSequenceError, Name, iteration);

				graph.GoTo(Sequence[iteration], nameof(Sequence), iteration);
			}

			yield break;
		}
	}
}
