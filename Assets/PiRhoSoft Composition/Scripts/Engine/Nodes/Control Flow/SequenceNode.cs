using System.Collections;
using PiRhoSoft.UtilityEngine;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[CreateInstructionGraphNodeMenu("Control Flow/Sequence", 10)]
	[HelpURL(Composition.DocumentationUrl + "sequence-node")]
	public class SequenceNode : InstructionGraphNode, ISequenceNode
	{
		private const string _invalidSequenceError = "(CSQIS) Unable to run sequence for {0}: index {1} has no been connection";

		[Tooltip("The nodes to visit in order")]
		[ListDisplay(AllowCollapse = false)]
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
