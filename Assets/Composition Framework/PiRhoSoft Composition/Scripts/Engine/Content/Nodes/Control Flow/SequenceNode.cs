using PiRhoSoft.PargonUtilities.Engine;
using System.Collections;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[CreateInstructionGraphNodeMenu("Control Flow/Sequence", 10)]
	[HelpURL(Composition.DocumentationUrl + "sequence-node")]
	public class SequenceNode : InstructionGraphNode, ISequenceNode
	{
		private const string _invalidSequenceError = "(CSQIS) Unable to run sequence for node '{0}': index '{1}' has no connection";

		[Tooltip("The nodes to visit in order")]
		[List]
		public InstructionGraphNodeList Sequence = new InstructionGraphNodeList();

		public override Color NodeColor => Colors.Sequence;

		public override IEnumerator Run(InstructionGraph graph, InstructionStore variables, int iteration)
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
