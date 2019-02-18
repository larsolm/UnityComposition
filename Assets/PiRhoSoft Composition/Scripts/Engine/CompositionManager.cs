using PiRhoSoft.UtilityEngine;
using System.Collections;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[DisallowMultipleComponent]
	[HelpURL(Composition.DocumentationUrl + "composition-manager")]
	[AddComponentMenu("Composition/Composition Manager")]
	public class CompositionManager : SingletonBehaviour<CompositionManager>
	{
		public const string _processFailedError = "(CCMPF) Failed to process Instruction '{0}': the Instruction yielded a value other than null or IEnumerator";

		[Tooltip("The Composition asset to load when this CompositionManager is loaded")]
		[AssetPopup]
		public CommandSet Commands; // this exists to provide a place to assign a Composition asset so that it will be loaded by Unity

		public void RunInstruction(Instruction instruction, InstructionContext context, IVariableStore thisStore)
		{
			var store = new InstructionStore(context, thisStore);
			var enumerator = instruction.Execute(store);

			if (instruction.IsExecutionImmediate)
				Process(instruction, enumerator);
			else
				StartCoroutine(enumerator);
		}

		public void RunInstruction(InstructionCaller caller, InstructionContext context, IVariableStore thisStore)
		{
			var enumerator = caller.Execute(context, thisStore);

			if (caller.IsExecutionImmediate)
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
