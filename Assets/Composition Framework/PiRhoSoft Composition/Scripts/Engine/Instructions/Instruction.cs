using PiRhoSoft.PargonUtilities.Engine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	public abstract class Instruction : ScriptableObject
	{
		private const string _alreadyRunningError = "(CIAR) Failed to run Instruction '{0}': the Instruction is already running";

		[Tooltip("The name used to access the context object from nodes in this graph")]
		public string ContextName = "context";

		[Tooltip("The variable definition for the context object that runs this graph")]
		public ValueDefinition ContextDefinition = ValueDefinition.Create(VariableType.Empty);

		[Tooltip("The variables definitions used as inputs for this graph")]
		[List(AllowAdd = false, AllowRemove = false, AllowReorder = false, EmptyLabel = "Inputs defined in nodes will appear here")]
		public VariableDefinitionList Inputs = new VariableDefinitionList();

		[Tooltip("The variables definitions used as outputs for this graph")]
		[List(AllowAdd = false, AllowRemove = false, AllowReorder = false, EmptyLabel = "Outputs defined in nodes will appear here")]
		public VariableDefinitionList Outputs = new VariableDefinitionList();

		public IVariableStore Variables { get; private set; }
		public bool IsRunning { get; private set; }

		protected virtual void OnEnable()
		{
			// in case the editor exits play mode while the instruction is running
			Variables = null;
			IsRunning = false;
		}

		protected virtual void OnDisable()
		{
			// not really necessary but might as well
			Variables = null;
			IsRunning = false;
		}

		public IEnumerator Execute(VariableValue context)
		{
			var store = InstructionStore.Reserve(this, context);
			yield return Execute(store);
			InstructionStore.Release(store);
		}

		public IEnumerator Execute(InstructionStore variables)
		{
			if (IsRunning)
			{
				Debug.LogErrorFormat(this, _alreadyRunningError, name);
			}
			else
			{
				Variables = variables;
				IsRunning = true;
				yield return CompositionManager.Track(this, Run(variables));
				IsRunning = false;
				Variables = null;
			}
		}

		protected virtual void GetInputs(IList<VariableDefinition> inputs) { }
		protected virtual void GetOutputs(IList<VariableDefinition> outputs) { }

		protected abstract IEnumerator Run(InstructionStore variables);

		#region Input and Output Schemas

		public void RefreshInputs()
		{
			var inputs = new VariableDefinitionList();
			GetInputs(inputs);
			Inputs = RefreshDefinitions(inputs);
		}

		public void RefreshOutputs()
		{
			var outputs = new VariableDefinitionList();
			GetOutputs(outputs);
			Outputs = RefreshDefinitions(outputs);
		}

		private VariableDefinitionList RefreshDefinitions(VariableDefinitionList requests)
		{
			var results = new VariableDefinitionList();

			foreach (var request in requests)
				UpdateDefinition(results, request, true);

			foreach (var existing in Inputs)
				UpdateDefinition(results, existing, false);

			return results;
		}

		private void UpdateDefinition(VariableDefinitionList definitions, VariableDefinition definition, bool add)
		{
			for (var i = 0; i < definitions.Count; i++)
			{
				if (definitions[i].Name == definition.Name)
				{
					if (!definitions[i].Definition.IsTypeLocked || (definitions[i].Definition.Type == definition.Definition.Type && !definitions[i].Definition.IsConstraintLocked))
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
