using System.Collections;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[CreateInstructionGraphNodeMenu("Object Manipulation/Enable Component", 22)]
	[HelpURL(Composition.DocumentationUrl + "enable-component-node")]
	public class EnableComponentNode : InstructionGraphNode, IImmediate
	{
		private const string _missingComponentWarning = "(CECNMC) failed to enable component {0}: the component could not be found";

		[Tooltip("The node to move to when this node is finished")]
		public InstructionGraphNode Next = null;

		protected override IEnumerator Run_(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			if (variables.This is MonoBehaviour behaviour)
				behaviour.enabled = true;
			else
				Debug.LogWarningFormat(this, _missingComponentWarning, This);

			graph.GoTo(Next, variables.This, nameof(Next));
			yield break;
		}
	}
}
