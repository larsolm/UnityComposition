using PiRhoSoft.UtilityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[Serializable] public class InstructionList : SerializedList<Instruction> { }
	[Serializable] public class InstructionDictionary : SerializedDictionary<string, Instruction> { }

	public abstract class Instruction : ScriptableObject
	{
		private const string _alreadyRunningError = "(CIAR) Failed to run Instruction '{0}': the Instruction is already running";

		public bool IsRunning { get; private set; }

		void OnEnable()
		{
			IsRunning = false; // in case the editor exits play mode while the instruction is running
		}

		public IEnumerator Execute(InstructionStore variables)
		{
			if (IsRunning)
			{
				Debug.LogErrorFormat(this, _alreadyRunningError, name);
			}
			else
			{
				IsRunning = true;
				yield return Run(variables);
				IsRunning = false;
			}
		}

		public virtual void GetInputs(List<VariableDefinition> inputs) { }
		public virtual void GetOutputs(List<VariableDefinition> outputs) { }

		public abstract bool IsExecutionImmediate { get; }
		protected abstract IEnumerator Run(InstructionStore variables);
	}
}
