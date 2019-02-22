using System.Collections;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[CreateInstructionGraphNodeMenu("Control Flow/Iterate", 21)]
	[HelpURL(Composition.DocumentationUrl + "iterate-node")]
	public class IterateNode : InstructionGraphNode
	{
		private const string _invalidStoreWarning = "(CCFINIS) Unable to iterate objects for {0}: the given variables must be an IIndexedVariableStore";

		[Tooltip("The node to go to for each object in the iteration")]
		public InstructionGraphNode Loop = null;

		[Tooltip("The node to go to when the iteration is finished")]
		public InstructionGraphNode Next = null;

		public override bool IsExecutionImmediate => true;
		public override InstructionGraphExecutionMode ExecutionMode => InstructionGraphExecutionMode.Loop;

		protected override IEnumerator Run_(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			if (variables.This is IIndexedVariableStore store)
			{
				if (Loop != null)
				{
					var item = store.GetItem(iteration);
					graph.GoTo(Loop, item);
				}
			}
			else
			{
				Debug.LogFormat(this, _invalidStoreWarning, name);
			}

			graph.BreakTo(Next);

			yield break;
		}
	}
}
