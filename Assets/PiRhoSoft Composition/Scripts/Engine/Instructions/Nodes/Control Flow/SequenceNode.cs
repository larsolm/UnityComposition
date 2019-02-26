using System.Collections;
using PiRhoSoft.UtilityEngine;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[CreateInstructionGraphNodeMenu("Control Flow/Sequence", 10)]
	[HelpURL(Composition.DocumentationUrl + "sequence-node")]
	public class SequenceNode : InstructionGraphNode, IImmediate, ISequenceNode
	{
		private const string _invalidSequenceError = "(CSQIS) failed to run sequence: node {0} has not been connected";

		[Tooltip("The nodes to visit in order")]
		[ListDisplay(AllowCollapse = false)]
		public InstructionGraphNodeList Sequence = new InstructionGraphNodeList();

		protected override IEnumerator Run_(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			if (iteration < Sequence.Count)
			{
				if (Sequence[iteration] == null)
					Debug.LogErrorFormat(this, _invalidSequenceError, iteration);

				graph.GoTo(Sequence[iteration], variables.This, nameof(Sequence), iteration);
			}

			yield break;
		}
	}
}
