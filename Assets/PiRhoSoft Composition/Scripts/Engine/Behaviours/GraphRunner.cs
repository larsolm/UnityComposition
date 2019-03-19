using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[HelpURL(Composition.DocumentationUrl + "graph-runner")]
	[AddComponentMenu("PiRho Soft/Composition/Graph Runner")]
	public class GraphRunner : MonoBehaviour
	{
		public InstructionCaller OnAwake = new InstructionCaller();

		void Start()
		{
			if (OnAwake.Instruction)
				CompositionManager.Instance.RunInstruction(OnAwake, this);
		}
	}
}
