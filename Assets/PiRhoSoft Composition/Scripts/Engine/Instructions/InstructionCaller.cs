﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[Serializable]
	public class InstructionCaller
	{
		[Tooltip("The Instruction to execute when this is called")] [SerializeField] private Instruction _instruction;
		[Tooltip("The input values to set for the Instruction")] [SerializeField] private List<InstructionInput> _inputs = new List<InstructionInput>();
		[Tooltip("The output values from the Instruction to store")] [SerializeField] private List<InstructionOutput> _outputs = new List<InstructionOutput>();

		public Instruction Instruction
		{
			get { return _instruction; }
			set { _instruction = value; UpdateVariables(); }
		}

		public List<InstructionInput> Inputs => _inputs;
		public List<InstructionOutput> Outputs => _outputs;
		public bool IsRunning => Instruction != null && Instruction.IsRunning;
		public bool IsExecutionImmediate => Instruction == null || Instruction.IsExecutionImmediate;

		public IEnumerator Execute(InstructionContext context, IVariableStore thisStore)
		{
			if (Instruction)
			{
				var store = new InstructionStore(context, thisStore);

				store.WriteInputs(_inputs);
				yield return Instruction.Execute(store);
				store.ReadOutputs(_outputs);
			}
		}

		#region Inputs and Outputs

		public void UpdateVariables()
		{
			UpdateInputs();
			UpdateOutputs();
		}

		private void UpdateInputs()
		{
			var inputs = new List<InstructionInput>();
			var inputDefinitions = new List<VariableDefinition>();

			if (_instruction != null)
			{
				_instruction.GetInputs(inputDefinitions);

				for (var i = 0; i < inputDefinitions.Count; i++)
				{
					var definition = inputDefinitions[i];

					// ignore duplicates
					if (inputs.Any(input => input.Definition.Name == definition.Name)) // TODO: make sure definitions are compatible
						continue;

					inputs.Add(new InstructionInput
					{
						Type = InstructionInputType.Value,
						Definition = definition
					});
				}
			}

			_inputs = inputs;
		}

		private void UpdateOutputs()
		{
			var outputs = new List<InstructionOutput>();
			var outputDefinitions = new List<VariableDefinition>();

			if (_instruction != null)
			{
				_instruction.GetOutputs(outputDefinitions);

				for (var i = 0; i < outputDefinitions.Count; i++)
				{
					var definition = outputDefinitions[i];

					// ignore duplicates
					if (outputs.Any(o => o.Definition.Name == definition.Name)) // TODO: duplicates should warn
						continue;

					outputs.Add(new InstructionOutput
					{
						Type = InstructionOutputType.Ignore,
						Definition = definition
					});
				}
			}

			_outputs = outputs;
		}

		#endregion
	}
}
