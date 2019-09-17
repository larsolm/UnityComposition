using PiRhoSoft.Utilities;
using System;
using System.Collections.Generic;
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
	public class TestComponent : MonoBehaviour
	{
		[Inline] public TestClass Test = new TestClass();
	}
}