using System.Collections;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[CreateInstructionGraphNodeMenu("Object Manipulation/Disable Component", 23)]
	[HelpURL(Composition.DocumentationUrl + "disable-component-node")]
	public class DisableComponentNode : InstructionGraphNode
	{
		private const string _missingComponentWarning = "(CDCNMC) Unable to disable component for {0}: the given variables must be a MonoBehaviour";

		[Tooltip("The node to move to when this node is finished")]
		public InstructionGraphNode Next = null;

		public override Color NodeColor => Colors.SequencingDark;

		protected override IEnumerator Run_(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			if (variables.This is Behaviour behaviour)
				behaviour.enabled = false;
			else
				Debug.LogWarningFormat(this, _missingComponentWarning, Name);

			graph.GoTo(Next, variables.This, nameof(Next));

			yield break;
		}
	}
}
