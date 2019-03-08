using PiRhoSoft.CompositionEngine;
using UnityEngine;

namespace PiRhoSoft.CompositionExample
{
	[AddComponentMenu("PiRho Soft/Examples/Interaction")]
	public class Interaction : MonoBehaviour
	{
		public InstructionCaller OnInteract = new InstructionCaller();

		public InstructionCaller OnEnter = new InstructionCaller();
		public InstructionCaller OnLeave = new InstructionCaller();
	}
}
