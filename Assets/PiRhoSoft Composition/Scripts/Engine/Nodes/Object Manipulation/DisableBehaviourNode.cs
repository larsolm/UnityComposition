using System.Collections;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[CreateInstructionGraphNodeMenu("Object Manipulation/Disable Behaviour", 21)]
	[HelpURL(Composition.DocumentationUrl + "disable-behaviour-node")]
	public class DisableBehaviourNode : InstructionGraphNode
	{
		[Tooltip("The node to move to when this node is finished")]
		public InstructionGraphNode Next = null;

		[Tooltip("The target Behaviour to enable")]
		[VariableConstraint(typeof(Behaviour))]
		public VariableReference Target = new VariableReference();

		public override Color NodeColor => Colors.SequencingDark;

		public override IEnumerator Run(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			if (Resolve<Behaviour>(variables, Target, out var behaviour))
				behaviour.enabled = false;

			graph.GoTo(Next, nameof(Next));

			yield break;
		}
	}
}
