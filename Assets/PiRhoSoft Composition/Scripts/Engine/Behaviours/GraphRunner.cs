using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[HelpURL(Composition.DocumentationUrl + "graph-runner")]
	[AddComponentMenu("PiRho Soft/Composition/Graph Runner")]
	public class GraphRunner : MonoBehaviour
	{
		[Tooltip("The graph to run when this runner is triggered")]
		public InstructionCaller Graph = new InstructionCaller();

		[Tooltip("The event to run")]
		public string Event;

		public void Run()
		{
			if (Graph.Instruction is EventGraph graph)
				graph.CurrentEvent = Event;

			if (Graph.Instruction)
				CompositionManager.Instance.RunInstruction(Graph, VariableValue.Create(this));
		}
	}
}
