﻿using System.Collections;
using System.Collections.Generic;
using PiRhoSoft.UtilityEngine;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[CreateInstructionGraphNodeMenu("Control Flow/Branch", 1)]
	[HelpURL(Composition.DocumentationUrl + "branch-node")]
	public class BranchNode : InstructionGraphNode
	{
		[Tooltip("The expression to evaluate to determine which node to follow")]
		public Expression Switch = new Expression();

		[Tooltip("The node to follow if the result of Switch is not in Outputs")]
		public InstructionGraphNode Default;

		[Tooltip("The node to follow depending on the result of Switch")]
		[DictionaryDisplay(EmptyText = "No outputs", ShowEditButton = true)]
		public InstructionGraphNodeDictionary Outputs = new InstructionGraphNodeDictionary();

		public override bool IsExecutionImmediate => true;
		public override InstructionGraphExecutionMode ExecutionMode => InstructionGraphExecutionMode.Normal;

		public override void GetInputs(List<VariableDefinition> inputs)
		{
			Switch.GetInputs(inputs, InstructionStore.InputStoreName);
		}

		protected override IEnumerator Run_(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			var name = Switch.Execute(variables, VariableType.String).String;

			if (Outputs.TryGetValue(name, out var output))
				graph.GoTo(output);
			else
				graph.GoTo(Default);

			yield break;
		}
	}
}
