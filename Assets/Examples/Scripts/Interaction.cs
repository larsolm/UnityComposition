using PiRhoSoft.CompositionEngine;
using UnityEngine;

namespace PiRhoSoft.CompositionExample
{
	[AddComponentMenu("PiRho Soft/Examples/Interaction")]
	public class Interaction : MonoBehaviour
	{
		public bool RequireKeypress = true;
		public InstructionCaller Caller = new InstructionCaller();
	}
}
