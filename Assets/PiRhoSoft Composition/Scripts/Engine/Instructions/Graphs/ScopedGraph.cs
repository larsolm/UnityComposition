using System.Collections;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[CreateAssetMenu(fileName = nameof(ScopedGraph), menuName = "Composition/Instruction Graph - Scoped", order = 126)]
	[HelpURL(Composition.DocumentationUrl + "scoped-graph")]
	public class ScopedGraph : InstructionGraph
	{
		public InstructionGraphNode Enter = null;
		public InstructionGraphNode Process = null;
		public InstructionGraphNode Exit = null;

		protected override IEnumerator Run(InstructionStore variables)
		{
			yield return Run(variables, Enter, nameof(Enter));
			yield return Run(variables, Process, nameof(Process));
			yield return Run(variables, Exit, nameof(Exit));
		}
	}
}
