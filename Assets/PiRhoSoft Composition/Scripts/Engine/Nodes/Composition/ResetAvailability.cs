using System.Collections;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[CreateInstructionGraphNodeMenu("Composition/Reset Availability", 21)]
	[HelpURL(Composition.DocumentationUrl + "reset-availability")]
	public class ResetAvailability : InstructionGraphNode
	{
		private const string _invalidVariablesWarning = "(CCRTTNF) Unable to reset availability for {0}: the given variables must be an IVariableReset";

		[Tooltip("The node to move to when this node is finished")]
		public InstructionGraphNode Next = null;

		[Tooltip("The Variables to reset")]
		public string Availability;

		public override Color NodeColor => Colors.ExecutionDark;

		public override IEnumerator Run(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			if (variables.Root is IVariableReset reset)
				reset.ResetAvailability(Availability);
			else
				Debug.LogWarningFormat(this, _invalidVariablesWarning, Name);

			graph.GoTo(Next, nameof(Next));

			yield break;
		}
	}
}
