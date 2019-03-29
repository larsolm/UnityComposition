﻿using PiRhoSoft.UtilityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	public abstract class Instruction : ScriptableObject
	{
		private const string _alreadyRunningError = "(CIAR) Failed to run Instruction '{0}': the Instruction is already running";

		public string ContextName = "context"; // this is drawn manually by the editor
		public VariableDefinition ContextDefinition = VariableDefinition.Create(string.Empty, VariableType.Empty);

		[ListDisplay(AllowAdd = false, AllowRemove = false, AllowReorder = false)] public VariableDefinitionList Inputs = new VariableDefinitionList();
		[ListDisplay(AllowAdd = false, AllowRemove = false, AllowReorder = false)] public VariableDefinitionList Outputs = new VariableDefinitionList();

		public bool IsRunning { get; private set; }

		protected virtual void OnEnable()
		{
			IsRunning = false; // in case the editor exits play mode while the instruction is running
		}

		protected virtual void OnDisable()
		{
			IsRunning = false; // not really necessary but might as well
		}

		public IEnumerator Execute(InstructionStore variables)
		{
			if (IsRunning)
			{
				Debug.LogErrorFormat(this, _alreadyRunningError, name);
			}
			else
			{
				CompositionManager.InstructionStarted(this, variables);

				IsRunning = true;
				yield return Run(variables);
				IsRunning = false;

				CompositionManager.InstructionComplete(this);
			}
		}

		protected virtual void GetInputs(IList<VariableDefinition> inputs) { }
		protected virtual void GetOutputs(IList<VariableDefinition> outputs) { }

		protected abstract IEnumerator Run(InstructionStore variables);

		#region Input and Output Schemas

		public void RefreshInputs()
		{
			var inputs = new VariableDefinitionList();
			var requests = new VariableDefinitionList();

			GetInputs(requests);

			foreach (var request in requests)
				UpdateDefinition(inputs, request, true);

			foreach (var existing in Inputs)
				UpdateDefinition(inputs, existing, false);

			Inputs = inputs;
		}

		public void RefreshOutputs()
		{
			Outputs = new VariableDefinitionList();
			GetOutputs(Outputs);
		}

		private void UpdateDefinition(VariableDefinitionList definitions, VariableDefinition definition, bool add)
		{
			for (var i = 0; i < definitions.Count; i++)
			{
				if (definitions[i].Name == definition.Name)
				{
					if (!definitions[i].IsTypeLocked || (definitions[i].Type == definition.Type && !definitions[i].IsConstraintLocked))
					{
						definitions[i] = definition;
						return;
					}
				}
			}

			if (add)
				definitions.Add(definition);
		}

		#endregion
	}
}
