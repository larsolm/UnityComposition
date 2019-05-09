using PiRhoSoft.UtilityEngine;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[HelpURL(Composition.DocumentationUrl + "button-graph-trigger")]
	[AddComponentMenu("PiRho Soft/Composition/Button Graph Trigger")]
	public sealed class ButtonGraphTrigger : InstructionTrigger
	{
		[Tooltip("The name of the Input Button that will trigger the InstructionGraph")]
		public string Button;

		void Update()
		{
			if (InputHelper.GetWasButtonPressed(Button))
				Run();
		}
	}
}
