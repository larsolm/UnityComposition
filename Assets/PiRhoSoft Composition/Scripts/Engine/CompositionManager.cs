using PiRhoSoft.UtilityEngine;
using System.Collections;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	public interface IImmediate
	{
	}

	public interface IIsImmediate
	{
		bool IsExecutionImmediate { get; }
	}

	[DisallowMultipleComponent]
	[HelpURL(Composition.DocumentationUrl + "composition-manager")]
	[AddComponentMenu("Composition/Composition Manager")]
	public class CompositionManager : SingletonBehaviour<CompositionManager>
	{
		public const string _processFailedError = "(CCMPF) Failed to process Instruction '{0}': the Instruction yielded a value other than null or IEnumerator";

		[Tooltip("The Composition asset to load when this CompositionManager is loaded")]
		[AssetPopup]
		public CommandSet Commands; // this exists to provide a place to assign a Composition asset so that it will be loaded by Unity

		public bool IsImmediate(Instruction instruction)
		{
			if (instruction is IImmediate)
				return true;
			else if (instruction is IIsImmediate isImmediate)
				return isImmediate.IsExecutionImmediate;
			else
				return false;
		}

		public bool IsImmediate(InstructionCaller caller)
		{
			return caller.Instruction == null || IsImmediate(caller.Instruction);
		}

		public void RunInstruction(Instruction instruction, InstructionContext context, object thisObject)
		{
			var store = new InstructionStore(context, thisObject);
			var enumerator = instruction.Execute(store);

			if (IsImmediate(instruction))
				Process(instruction, enumerator);
			else
				StartCoroutine(enumerator);
		}

		public void RunInstruction(InstructionCaller caller, InstructionContext context, object thisObject)
		{
			var enumerator = caller.Execute(context, thisObject);

			if (IsImmediate(caller))
				Process(caller.Instruction, enumerator);
			else
				StartCoroutine(enumerator);
		}

		private void Process(Instruction instruction, IEnumerator enumerator)
		{
			while (enumerator.MoveNext())
			{
				switch (enumerator.Current)
				{
					case null: break;
					case IEnumerator child: Process(instruction, child); break;
					default: Debug.LogErrorFormat(instruction, _processFailedError, instruction.name); break;
				}
			}
		}
	}
}
