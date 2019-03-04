using PiRhoSoft.UtilityEngine;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[RequireComponent(typeof(Rigidbody2D))]
	[AddComponentMenu("PiRho Soft/Composition/Demo Interaction")]
	public class DemoInteraction : MonoBehaviour
	{
		public InstructionCaller Caller = new InstructionCaller();
	}
}
