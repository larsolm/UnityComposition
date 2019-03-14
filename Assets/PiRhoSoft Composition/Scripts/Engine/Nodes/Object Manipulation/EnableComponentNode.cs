using System.Collections;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[CreateInstructionGraphNodeMenu("Object Manipulation/Enable Component", 20)]
	[HelpURL(Composition.DocumentationUrl + "enable-component-node")]
	public class EnableComponentNode : InstructionGraphNode
	{
		private const string _missingComponentWarning = "(COMECNMC) Unable to enable component for {0}: the given variables must be a MonoBehaviour";

		[Tooltip("The node to move to when this node is finished")]
		public InstructionGraphNode Next = null;

		public override Color NodeColor => Colors.SequencingLight;

		public override IEnumerator Run(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			if (variables.Root is Behaviour behaviour)
				behaviour.enabled = true;
			else
				Debug.LogWarningFormat(this, _missingComponentWarning, Name);

			graph.GoTo(Next, nameof(Next));

			yield break;
		}
	}
}
