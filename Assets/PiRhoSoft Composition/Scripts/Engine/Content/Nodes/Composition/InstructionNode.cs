﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[CreateInstructionGraphNodeMenu("Composition/Instruction", 1)]
	[HelpURL(Composition.DocumentationUrl + "instruction-node")]
	public class InstructionNode : InstructionGraphNode
	{
		[Tooltip("The node to move to when this node is finished")]
		public InstructionGraphNode Next = null;

		[Tooltip("The instruction to run when this node is reached")]
		public InstructionCaller Instruction = new InstructionCaller();

		[Tooltip("The object to use as the root for Instruction")]
		public VariableValueSource Context = new VariableValueSource { Type = VariableSourceType.Reference, Reference = new VariableReference { Variable = "context" } };

		[Tooltip("Whether to wait for the instruction to finish before moving to Next")]
		public bool WaitForCompletion = true;

		public override Color NodeColor => Colors.ExecutionLight;

		public override void GetInputs(IList<VariableDefinition> inputs)
		{
			foreach (var input in Instruction.Inputs)
			{
				if (InstructionStore.IsInput(input))
					inputs.Add(Instruction.GetInputDefinition(input));
			}
		}

		public override void GetOutputs(IList<VariableDefinition> outputs)
		{
			foreach (var output in Instruction.Outputs)
			{
				if (InstructionStore.IsOutput(output))
					outputs.Add(Instruction.GetOutputDefinition(output));
			}
		}

		public override IEnumerator Run(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			if (!Resolve(variables, Context, out var context))
				context = variables.Context;

			if (WaitForCompletion)
				yield return Instruction.Execute(variables, context);
			else
				CompositionManager.Instance.RunInstruction(Instruction, variables, context);

			graph.GoTo(Next, nameof(Next));
		}
	}
}