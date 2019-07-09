using PiRhoSoft.PargonUtilities.Engine;
using System;
using System.Collections;
using UnityEngine;

namespace PiRhoSoft.CompositionExample
{
	[Flags]
	public enum TestEnum
	{
		One = 0x1,
		Two = 0x2,
		Four = 0x4,
		Eight = 0x8
	}

	public class IntBindingAttribute : PropertyAttribute
	{
	}

	[AddComponentMenu("PiRho Soft/Examples/Test")]
	public class TestBehaviour : MonoBehaviour
	{
		[EnumButtons]
		public TestEnum TestEnum = TestEnum.Two;
		public int TestInt = 5;

		private void Start()
		{
			StartCoroutine(Increment());
		}

		private IEnumerator Increment()
		{
			while (true)
			{
				if ((int)TestEnum >= 15)
					TestEnum = 0;
				else
					TestEnum++;

				yield return new WaitForSeconds(1.0f);
			}
		}
	}
}
