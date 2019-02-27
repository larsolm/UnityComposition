using System.Collections;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[CreateInstructionGraphNodeMenu("Object Manipulation/Disable Component", 23)]
	[HelpURL(Composition.DocumentationUrl + "disable-component-node")]
	public class DisableComponentNode : InstructionGraphNode, IImmediate
	{
		private const string _missingComponentWarning = "(CDCNMC) Unable to disable component for {0}: the given variables must be a MonoBehaviour";

		[Tooltip("The node to move to when this node is finished")]
		public InstructionGraphNode Next = null;

		public override Color GetNodeColor()
		{
			return new Color(0.0f, 0.25f, 0.0f);
		}

		protected override IEnumerator Run_(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			if (variables.This is MonoBehaviour behaviour)
				behaviour.enabled = false;
			else
				Debug.LogWarningFormat(this, _missingComponentWarning, Name);

			graph.GoTo(Next, variables.This, nameof(Next));

			yield break;
		}
	}
}
