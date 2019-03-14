using System.Collections;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[CreateInstructionGraphNodeMenu("Control Flow/Iterate", 21)]
	[HelpURL(Composition.DocumentationUrl + "iterate-node")]
	public class IterateNode : InstructionGraphNode, ILoopNode
	{
		private const string _invalidStoreWarning = "(CCFINIS) Unable to iterate objects for {0}: the given variables must be an IIndexedVariableStore";

		[Tooltip("The node to go to for each object in the iteration")]
		public InstructionGraphNode Loop = null;

		public override Color NodeColor => Colors.Loop;

		public override IEnumerator Run(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			if (variables.Root is IIndexedVariableStore store)
			{
				var item = store.GetItem(iteration);

				if (Loop != null && item != null)
				{
					graph.ChangeRoot(item);
					graph.GoTo(Loop, nameof(Loop));
				}
			}
			else
			{
				Debug.LogFormat(this, _invalidStoreWarning, Name);
			}

			yield break;
		}
	}
}
