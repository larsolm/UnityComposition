using PiRhoSoft.CompositionEngine;
using UnityEngine;

namespace PiRhoSoft.CompositionDemo
{
	[AddComponentMenu("PiRho Soft/Examples/Interaction")]
	public class Interaction : MonoBehaviour
	{
		public InstructionCaller Caller = new InstructionCaller();
	}
}
