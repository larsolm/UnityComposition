using System.Collections;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[CreateAssetMenu(fileName = nameof(SimpleGraph), menuName = "PiRho Soft/Graphs/Simple", order = 100)]
	[HelpURL(Composition.DocumentationUrl + "simple-graph")]
	public class SimpleGraph : InstructionGraph
	{
		public InstructionGraphNode Process = null;

		protected override IEnumerator Run(InstructionStore variables)
		{
			yield return Run(variables, Process, nameof(Process));
		}
	}
}
