using System.Collections;
using PiRhoSoft.UtilityEngine;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[CreateInstructionGraphNodeMenu("Control Flow/Sequence", 10)]
	[HelpURL(Composition.DocumentationUrl + "sequence-node")]
	public class SequenceNode : InstructionGraphNode, IImmediate, ISequenceNode
	{
		private const string _invalidSequenceError = "(CSQIS) Unable to run sequence for {0}: index {1} has no been connection";

		[Tooltip("The nodes to visit in order")]
		[ListDisplay(AllowCollapse = false)]
		public InstructionGraphNodeList Sequence = new InstructionGraphNodeList();

		public override Color GetNodeColor()
		{
			return new Color(0.5f, 0.2f, 0.2f);
		}

		protected override IEnumerator Run_(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			if (iteration < Sequence.Count)
			{
				if (Sequence[iteration] == null)
					Debug.LogErrorFormat(this, _invalidSequenceError, Name, iteration);

				graph.GoTo(Sequence[iteration], variables.This, nameof(Sequence), iteration);
			}

			yield break;
		}
	}
}
