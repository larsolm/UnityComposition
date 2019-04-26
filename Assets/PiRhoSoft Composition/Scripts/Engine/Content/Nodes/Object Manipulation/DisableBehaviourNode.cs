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

		[Tooltip("The target Behaviour or Renderer to disable")]
		[VariableConstraint(typeof(Behaviour))]
		public VariableReference Target = new VariableReference();

		public override Color NodeColor => Colors.SequencingDark;

		public override IEnumerator Run(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			if (ResolveObject(variables, Target, out Behaviour behaviour))
				behaviour.enabled = false;
			else if (ResolveObject(variables, Target, out Renderer renderer))
				renderer.enabled = false;

			graph.GoTo(Next, nameof(Next));

			yield break;
		}
	}
}
