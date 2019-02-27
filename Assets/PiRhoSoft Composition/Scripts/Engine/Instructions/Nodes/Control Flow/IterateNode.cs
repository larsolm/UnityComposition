using System.Collections;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[CreateInstructionGraphNodeMenu("Control Flow/Iterate", 21)]
	[HelpURL(Composition.DocumentationUrl + "iterate-node")]
	public class IterateNode : InstructionGraphNode, IImmediate, ILoopNode
	{
		private const string _invalidStoreWarning = "(CCFINIS) Unable to iterate objects for {0}: the given variables must be an IIndexedVariableStore";

		[Tooltip("The node to go to for each object in the iteration")]
		public InstructionGraphNode Loop = null;

		protected override IEnumerator Run_(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			if (variables.This is IIndexedVariableStore store)
			{
				if (Loop != null)
				{
					var item = store.GetItem(iteration);
					graph.GoTo(Loop, item, nameof(Loop));
				}
			}
			else
			{
				Debug.LogFormat(this, _invalidStoreWarning, name);
			}

			graph.Break();
			yield break;
		}
	}
}
