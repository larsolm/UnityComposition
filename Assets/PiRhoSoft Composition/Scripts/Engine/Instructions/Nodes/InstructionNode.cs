using System.Collections;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[CreateInstructionGraphNodeMenu("General/Instruction")]
	[HelpURL(Composition.DocumentationUrl + "instruction-node")]
	public class InstructionNode : InstructionGraphNode
	{
		[Tooltip("The instruction to run when this node is reached")]
		public InstructionCaller Instruction = new InstructionCaller();

		[Tooltip("The node to move to when this node is finished")]
		public InstructionGraphNode Next = null;

		[Tooltip("Whether to wait for the instruction to finish before moving to Next")]
		public bool WaitForCompletion = false;

		public override bool IsExecutionImmediate => !WaitForCompletion || Instruction.IsExecutionImmediate;
		public override InstructionGraphExecutionMode ExecutionMode => InstructionGraphExecutionMode.Normal;

		protected override IEnumerator Run_(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			if (WaitForCompletion)
				yield return Instruction.Execute(variables.Context, variables.This);
			else
				CompositionManager.Instance.RunInstruction(Instruction, variables.Context, variables.This);

			graph.GoTo(Next);
		}
	}
}
