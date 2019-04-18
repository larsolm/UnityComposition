using System.Collections;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[CreateInstructionGraphNodeMenu("Object Manipulation/Enable Game Object", 10)]
	[HelpURL(Composition.DocumentationUrl + "enable-game-object-node")]
	public class EnableGameObjectNode : InstructionGraphNode
	{
		[Tooltip("The node to move to when this node is finished")]
		public InstructionGraphNode Next = null;

		[Tooltip("The target GameObject to enable")]
		[VariableConstraint(typeof(GameObject))]
		public VariableReference Target = new VariableReference();

		public override Color NodeColor => Colors.SequencingLight;

		public override IEnumerator Run(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			if (ResolveObject(variables, Target, out GameObject target))
				target.SetActive(true);

			graph.GoTo(Next, nameof(Next));

			yield break;
		}
	}
}
