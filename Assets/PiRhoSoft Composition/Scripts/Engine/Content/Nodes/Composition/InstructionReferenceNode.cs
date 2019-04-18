using System.Collections;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[CreateInstructionGraphNodeMenu("Composition/Instruction - Reference", 2)]
	[HelpURL(Composition.DocumentationUrl + "instruction-reference-node")]
	public class InstructionReferenceNode : InstructionGraphNode
	{
		[Tooltip("The node to move to when this node is finished")]
		public InstructionGraphNode Next = null;

		[Tooltip("The instruction to run when this node is reached")]
		[VariableConstraint(typeof(Instruction))]
		public VariableReference Instruction = new VariableReference();

		[Tooltip("The object to use as the root for Instruction")]
		public VariableValueSource Context = new VariableValueSource { Type = VariableSourceType.Reference, Reference = new VariableReference { Variable = "context" } };

		[Tooltip("Whether to wait for the instruction to finish before moving to Next")]
		public bool WaitForCompletion = true;

		public override Color NodeColor => Colors.ExecutionLight;

		public override IEnumerator Run(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			if (!Resolve(variables, Context, out var context))
				context = variables.Context;

			if (ResolveObject(variables, Instruction, out Instruction instruction))
			{
				if (WaitForCompletion)
				{
					var store = new InstructionStore(instruction, context);
					yield return instruction.Execute(store);
				}
				else
				{
					CompositionManager.Instance.RunInstruction(instruction, context);
				}
			}

			graph.GoTo(Next, nameof(Next));
		}
	}
}
