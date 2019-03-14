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

		[Tooltip("The object to use as the root for Instruction")]
		public VariableReference Root = new VariableReference(InstructionStore.RootStoreName);

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

		public override IEnumerator Run(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			//if (!Resolve(variables, Root, out object root))
			//	root = variables.Root;

			if (WaitForCompletion)
				yield return Instruction.Execute(variables.Root);
			else
				CompositionManager.Instance.RunInstruction(Instruction, variables.Root);

			graph.GoTo(Next, nameof(Next));
		}
	}
}
