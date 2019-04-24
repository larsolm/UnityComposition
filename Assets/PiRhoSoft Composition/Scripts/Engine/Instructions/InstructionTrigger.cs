using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[HelpURL(Composition.DocumentationUrl + "instruction-trigger")]
	[AddComponentMenu("PiRho Soft/Composition/Instruction Trigger")]
	public class InstructionTrigger : MonoBehaviour
	{
		[Tooltip("The graph to run when this object is triggered")]
		public InstructionCaller Graph = new InstructionCaller();

		public void Run()
		{
			if (Graph.Instruction && !Graph.IsRunning)
				CompositionManager.Instance.RunInstruction(Graph, CompositionManager.Instance.DefaultStore, VariableValue.Create(this));
		}
	}
}
