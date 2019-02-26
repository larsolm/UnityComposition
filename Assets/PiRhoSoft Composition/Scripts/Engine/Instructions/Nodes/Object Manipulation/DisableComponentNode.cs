using System.Collections;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[CreateInstructionGraphNodeMenu("Object Manipulation/Disable Component", 23)]
	[HelpURL(Composition.DocumentationUrl + "disable-component-node")]
	public class DisableComponentNode : InstructionGraphNode, IImmediate
	{
		private const string _missingComponentWarning = "(CDCNMC) failed to disable component {0}: the component could not be found";

		[Tooltip("The node to move to when this node is finished")]
		public InstructionGraphNode Next = null;

		protected override IEnumerator Run_(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			if (variables.This is MonoBehaviour behaviour)
				behaviour.enabled = false;
			else
				Debug.LogWarningFormat(this, _missingComponentWarning, This);

			graph.GoTo(Next, variables.This, nameof(Next));
			yield break;
		}
	}
}
