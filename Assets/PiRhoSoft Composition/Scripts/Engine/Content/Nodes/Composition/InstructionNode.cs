using PiRhoSoft.UtilityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[CreateInstructionGraphNodeMenu("Composition/Instruction", 1)]
	[HelpURL(Composition.DocumentationUrl + "instruction-node")]
	public class InstructionNode : InstructionGraphNode
	{
		public enum InstructionSource
		{
			Value,
			Reference
		}

		[Tooltip("The node to move to when this node is finished")]
		public InstructionGraphNode Next = null;

		[Tooltip("The source of the instruction to run")]
		[EnumDisplay]
		public InstructionSource Source = InstructionSource.Value;

		[Tooltip("The instruction to run when this node is reached")]
		[ConditionalDisplaySelf(nameof(Source), EnumValue = (int)InstructionSource.Value)]
		public InstructionCaller Instruction = new InstructionCaller();

		[Tooltip("The instruction to run when this node is reached")]
		[VariableConstraint(typeof(Instruction))]
		[ConditionalDisplaySelf(nameof(Source), EnumValue = (int)InstructionSource.Reference)]
		public VariableReference Reference = new VariableReference();

		[Tooltip("The object to use as the root for Instruction")]
		public VariableValueSource Context = new VariableValueSource { Type = VariableSourceType.Reference, Reference = new VariableReference { Variable = "context" } };

		[Tooltip("Whether to wait for the instruction to finish before moving to Next")]
		public bool WaitForCompletion = true;

		public override Color NodeColor => Colors.ExecutionLight;

		public override void GetInputs(IList<VariableDefinition> inputs)
		{
			if (Source == InstructionSource.Value)
			{
				foreach (var input in Instruction.Inputs)
				{
					if (InstructionStore.IsInput(input))
						inputs.Add(Instruction.GetInputDefinition(input));
				}
			}
			else if (Source == InstructionSource.Reference)
			{
				if (InstructionStore.IsInput(Reference))
					inputs.Add(new VariableDefinition { Name = Reference.RootName, Definition = ValueDefinition.Create<Instruction>() });
			}
		}

		public override void GetOutputs(IList<VariableDefinition> outputs)
		{
			if (Source == InstructionSource.Value)
			{
				foreach (var output in Instruction.Outputs)
				{
					if (InstructionStore.IsOutput(output))
						outputs.Add(Instruction.GetOutputDefinition(output));
				}
			}
		}

		public override IEnumerator Run(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			if (!Resolve(variables, Context, out var context))
				context = variables.Context;

			if (Source == InstructionSource.Value)
			{
				if (WaitForCompletion)
					yield return Instruction.Execute(variables, context);
				else
					CompositionManager.Instance.RunInstruction(Instruction, variables, context);
			}
			else if (Source == InstructionSource.Reference)
			{
				if (ResolveObject(variables, Reference, out Instruction instruction))
				{
					if (WaitForCompletion)
						yield return instruction.Execute(variables);
					else
						CompositionManager.Instance.RunInstruction(instruction, context);
				}
			}

			graph.GoTo(Next, nameof(Next));
		}
	}
}
