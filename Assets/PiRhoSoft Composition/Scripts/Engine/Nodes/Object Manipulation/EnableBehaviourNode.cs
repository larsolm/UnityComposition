using System.Collections;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[CreateInstructionGraphNodeMenu("Object Manipulation/Enable Behaviour", 20)]
	[HelpURL(Composition.DocumentationUrl + "enable-behaviour-node")]
	public class EnableBehaviourNode : InstructionGraphNode
	{
		[Tooltip("The node to move to when this node is finished")]
		public InstructionGraphNode Next = null;

		[Tooltip("The target Behaviour to enable")]
		[VariableConstraint(typeof(Behaviour))]
		public VariableReference Target = new VariableReference();

		public override Color NodeColor => Colors.SequencingLight;

		public override IEnumerator Run(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			if (ResolveObject(variables, Target, out Behaviour behaviour))
				behaviour.enabled = true;

			graph.GoTo(Next, nameof(Next));

			yield break;
		}
	}
}
