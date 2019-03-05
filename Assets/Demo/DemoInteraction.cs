using PiRhoSoft.CompositionEngine;
using UnityEngine;

namespace PiRhoSoft.CompositionDemo
{
	[RequireComponent(typeof(Rigidbody2D))]
	[AddComponentMenu("PiRho Soft/Demo/Demo Interaction")]
	public class DemoInteraction : MonoBehaviour
	{
		public InstructionCaller Caller = new InstructionCaller();
	}
}
