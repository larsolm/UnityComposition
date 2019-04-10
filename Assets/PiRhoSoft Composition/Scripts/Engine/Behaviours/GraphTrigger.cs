using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[HelpURL(Composition.DocumentationUrl + "graph-trigger")]
	[AddComponentMenu("PiRho Soft/Composition/Graph Trigger")]
	public class GraphTrigger : MonoBehaviour
	{
		[Tooltip("The graph to run when this object is triggered")]
		public InstructionCaller Graph = new InstructionCaller();

		public void Run()
		{
			if (Graph.Instruction)
				CompositionManager.Instance.RunInstruction(Graph, VariableValue.Create(this));
		}
	}
}
