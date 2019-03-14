using System.Collections;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[CreateInstructionGraphNodeMenu("Object Manipulation/Disable Game Object", 11)]
	[HelpURL(Composition.DocumentationUrl + "disable-game-object-node")]
	public class DisableGameObjectNode : InstructionGraphNode
	{
		[Tooltip("The node to move to when this node is finished")]
		public InstructionGraphNode Next = null;

		[Tooltip("The target GameObject to enable")]
		[VariableConstraint(typeof(GameObject))]
		public VariableReference Target = new VariableReference();

		public override Color NodeColor => Colors.SequencingDark;

		public override IEnumerator Run(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			if (Resolve<GameObject>(variables, Target, out var target))
				target.SetActive(false);

			graph.GoTo(Next, nameof(Next));

			yield break;
		}
	}
}
