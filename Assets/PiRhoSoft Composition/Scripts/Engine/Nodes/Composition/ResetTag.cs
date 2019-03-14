using System.Collections;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[CreateInstructionGraphNodeMenu("Composition/Reset Tag", 21)]
	[HelpURL(Composition.DocumentationUrl + "reset-tag")]
	public class ResetTag : InstructionGraphNode
	{
		private const string _invalidVariablesWarning = "(CCRTTNF) Unable to reset tag for {0}: the given variables must be an IVariableReset";

		[Tooltip("The node to move to when this node is finished")]
		public InstructionGraphNode Next = null;

		[Tooltip("The tag in in which to reset")]
		public string Tag;

		public override Color NodeColor => Colors.ExecutionDark;

		public override IEnumerator Run(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			if (variables.Root is IVariableReset reset)
				reset.ResetTag(Tag);
			else
				Debug.LogWarningFormat(this, _invalidVariablesWarning, Name);

			graph.GoTo(Next, nameof(Next));

			yield break;
		}
	}
}
