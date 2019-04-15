using System.Collections;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[CreateInstructionGraphNodeMenu("Composition/Set Variable", 12)]
	[HelpURL(Composition.DocumentationUrl + "set-variable-node")]
	public class SetVariableNode : InstructionGraphNode
	{
		[Tooltip("The node to move to when this node is finished")]
		public InstructionGraphNode Next = null;

		[Tooltip("The variable to set")]
		public VariableReference Variable = new VariableReference();

		[Tooltip("The value to set the variable to")]
		public VariableValueSource Value = new VariableValueSource();

		public override Color NodeColor => Colors.ExecutionDark;

		public override IEnumerator Run(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			if (Resolve(variables, Value, out var value))
				Variable.SetValue(variables, value);

			graph.GoTo(Next, nameof(Next));
			yield break;
		}
	}
}