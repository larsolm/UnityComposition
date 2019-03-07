using PiRhoSoft.CompositionEngine;
using UnityEngine;

namespace PiRhoSoft.CompositionExample
{
	[AddComponentMenu("PiRho Soft/Examples/Interaction")]
	public class Interaction : MonoBehaviour
	{
		public InstructionCaller Caller = new InstructionCaller();
	}
}
