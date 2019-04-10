using System;
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

		public IList<InstructionInput> Inputs => _inputs;
		public IList<InstructionOutput> Outputs => _outputs;
		public bool IsRunning => Instruction != null && Instruction.IsRunning;

		public IEnumerator Execute(IVariableStore store, VariableValue context)
		{
			if (Instruction)
			{
				var localStore = new InstructionStore(Instruction, context);

				localStore.WriteInputs(this, _inputs, store);
				localStore.WriteOutputs(_outputs);
				yield return Instruction.Execute(localStore);
				localStore.ReadOutputs(_outputs, store);
			}
		}

		#region Inputs and Outputs

		public void UpdateVariables()
		{
			UpdateInputs();
			UpdateOutputs();
		}

		public VariableDefinition GetInputDefinition(InstructionInput input)
		{
			if (Instruction)
			{
				foreach (var definition in Instruction.Inputs)
				{
					if (input.Name == definition.Name)
						return definition;
				}
			}

			return new VariableDefinition { Name = input.Name, Definition = ValueDefinition.Create(VariableType.Empty) };
		}

		public VariableDefinition GetOutputDefinition(InstructionOutput output)
		{
			if (Instruction)
			{
				foreach (var definition in Instruction.Outputs)
				{
					if (output.Name == definition.Name)
						return definition;
				}
			}

			return new VariableDefinition { Name = output.Name, Definition = ValueDefinition.Create(VariableType.Empty) };
		}

		private void UpdateInputs()
		{
			var inputs = new List<InstructionInput>();

			if (_instruction != null)
			{
				_instruction.RefreshInputs();

				foreach (var definition in _instruction.Inputs)
				{
					var existing = _inputs.Where(input => input.Name == definition.Name).FirstOrDefault();

					if (existing != null && (existing.Type == InstructionInputType.Reference || definition.Definition.IsValid(existing.Value)))
					{
						if (!inputs.Any(input => input.Name == definition.Name))
							inputs.Add(existing);
					}
					else
					{
						inputs.Add(new InstructionInput
						{
							Name = definition.Name,
							Type = InstructionInputType.Value,
							Value = definition.Definition.Generate(null)
						});
					}
				}
			}

			_inputs = inputs;
		}

		private void UpdateOutputs()
		{
			var outputs = new List<InstructionOutput>();

			if (_instruction != null)
			{
				_instruction.RefreshOutputs();

				foreach (var definition in _instruction.Outputs)
				{
					var existing = _outputs.Where(output => output.Name == definition.Name).FirstOrDefault();

					if (existing != null)
					{
						if (!outputs.Any(output => output.Name == definition.Name))
							outputs.Add(existing);
					}
					else
					{
						outputs.Add(new InstructionOutput
						{
							Name = definition.Name,
							Type = InstructionOutputType.Ignore
						});
					}
				}
			}

			_outputs = outputs;
		}

		#endregion
	}
}
