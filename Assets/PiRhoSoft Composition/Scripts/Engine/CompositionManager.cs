using PiRhoSoft.UtilityEngine;
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

		public void RunInstruction(Instruction instruction, InstructionContext context, object thisObject)
		{
			var store = new InstructionStore(context, thisObject);
			var enumerator = instruction.Execute(store);

			StartCoroutine(new JoinEnumerator(enumerator));
		}

		public void RunInstruction(InstructionCaller caller, InstructionContext context, object thisObject)
		{
			var enumerator = caller.Execute(context, thisObject);
			StartCoroutine(new JoinEnumerator(enumerator));
		}
	}
}
