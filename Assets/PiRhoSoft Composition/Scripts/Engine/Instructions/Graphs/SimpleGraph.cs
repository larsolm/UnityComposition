using System.Collections;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[CreateAssetMenu(fileName = nameof(SimpleGraph), menuName = "Composition/Instruction Graph - Simple", order = 125)]
	public class SimpleGraph : InstructionGraph
	{
		public InstructionGraphNode Process = null;

		protected override IEnumerator Run(InstructionStore variables)
		{
			yield return Run(variables, Process);
		}
	}
}
