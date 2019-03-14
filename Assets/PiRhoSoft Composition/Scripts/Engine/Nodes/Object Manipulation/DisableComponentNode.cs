using System.Collections;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[CreateInstructionGraphNodeMenu("Object Manipulation/Disable Component", 21)]
	[HelpURL(Composition.DocumentationUrl + "disable-component-node")]
	public class DisableComponentNode : InstructionGraphNode
	{
		private const string _missingComponentWarning = "(COMDCNMC) Unable to disable component for {0}: the given variables must be a MonoBehaviour";

		[Tooltip("The node to move to when this node is finished")]
		public InstructionGraphNode Next = null;

		public override Color NodeColor => Colors.SequencingDark;

		public override IEnumerator Run(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			if (variables.Root is Behaviour behaviour)
				behaviour.enabled = false;
			else
				Debug.LogWarningFormat(this, _missingComponentWarning, Name);

			graph.GoTo(Next, nameof(Next));

			yield break;
		}
	}
}
