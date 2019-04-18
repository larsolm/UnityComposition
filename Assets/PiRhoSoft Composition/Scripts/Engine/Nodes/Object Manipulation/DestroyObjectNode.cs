using PiRhoSoft.UtilityEngine;
using System.Collections;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[CreateInstructionGraphNodeMenu("Object Manipulation/Destroy Object", 2)]
	[HelpURL(Composition.DocumentationUrl + "destroy-object-node")]
	public class DestroyObjectNode : InstructionGraphNode
	{
		[Tooltip("The node to move to when this node is finished")]
		public InstructionGraphNode Next = null;

		[Tooltip("The target Object to destroy")]
		[VariableConstraint(typeof(Object))]
		public VariableReference Target = new VariableReference();

		public override Color NodeColor => Colors.SequencingDark;

		public override IEnumerator Run(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			if (ResolveObject(variables, Target, out Object target))
				Destroy(ComponentHelper.GetAsBaseObject(target));

			graph.GoTo(Next, nameof(Next));

			yield break;
		}
	}
}
