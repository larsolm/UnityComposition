using System.Collections;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[CreateInstructionGraphNodeMenu("Object Manipulation/Enable Component", 22)]
	[HelpURL(Composition.DocumentationUrl + "enable-component-node")]
	public class EnableComponentNode : InstructionGraphNode, IImmediate
	{
		private const string _missingComponentWarning = "(CECNMC) Unable to enable component for {0}: the given variables must be a MonoBehaviour";

		[Tooltip("The node to move to when this node is finished")]
		public InstructionGraphNode Next = null;

		public override Color NodeColor => Colors.SequencingLight;

		protected override IEnumerator Run_(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			if (variables.This is MonoBehaviour behaviour)
				behaviour.enabled = true;
			else
				Debug.LogWarningFormat(this, _missingComponentWarning, Name);

			graph.GoTo(Next, variables.This, nameof(Next));

			yield break;
		}
	}
}
