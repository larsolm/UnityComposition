using PiRhoSoft.CompositionEngine;
using UnityEngine;

namespace PiRhoSoft.CompositionDemo
{
	[RequireComponent(typeof(Rigidbody2D))]
	[AddComponentMenu("PiRho Soft/Examples/Interaction")]
	public class Interaction : MonoBehaviour
	{
		public InstructionCaller Caller = new InstructionCaller();
	}
}
