using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[CreateInstructionGraphNodeMenu("Composition/Instruction", 10)]
	[HelpURL(Composition.DocumentationUrl + "instruction-node")]
	public class InstructionNode : InstructionGraphNode
	{
		[Tooltip("The instruction to run when this node is reached")]
		public InstructionCaller Instruction = new InstructionCaller();

		[Tooltip("The node to move to when this node is finished")]
		public InstructionGraphNode Next = null;

		[Tooltip("Whether to wait for the instruction to finish before moving to Next")]
		public bool WaitForCompletion = true;

		public override Color NodeColor => Colors.ExecutionLight;

		public override void GetInputs(List<VariableDefinition> inputs)
		{
			foreach (var input in Instruction.Inputs)
			{
				if (InstructionStore.IsInput(input))
					inputs.Add(input.Definition);
			}
		}

		public override void GetOutputs(List<VariableDefinition> outputs)
		{
			foreach (var output in Instruction.Outputs)
			{
				if (InstructionStore.IsOutput(output))
					outputs.Add(output.Definition);
			}
		}

		protected override IEnumerator Run_(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			if (WaitForCompletion)
				yield return Instruction.Execute(variables.Context, variables.This);
			else
				InstructionManager.Instance.RunInstruction(Instruction, variables.Context, variables.This);

			graph.GoTo(Next, variables.This, nameof(Next));
		}
	}
}
