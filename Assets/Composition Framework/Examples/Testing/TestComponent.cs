using System;
using UnityEngine;

namespace PiRhoSoft.CompositionExample
{
	[Serializable]
	public class TestClass
	{
		public int Int;
		public float Float;
	}

	[AddComponentMenu("PiRho Soft/Examples/Test")]
	[ExecuteAlways]
	public class TestComponent : MonoBehaviour
	{
		public TestClass Test = new TestClass();
	}
}