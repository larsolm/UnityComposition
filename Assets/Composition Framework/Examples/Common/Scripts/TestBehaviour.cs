using PiRhoSoft.PargonUtilities.Engine;
using PiRhoSoft.Utilities;
using System;
using System.Collections;
using UnityEngine;

namespace PiRhoSoft.CompositionExample
{
	public enum TestEnum
	{
		One = 0x1,
		Two = 0x2,
		Four = 0x4,
		Eight = 0x8
	}

	[Serializable]
	public class IntList : SerializedList<int> { }

	[Serializable]
	public class TestEnumList : SerializedList<TestEnum> { }

	[AddComponentMenu("PiRho Soft/Examples/Test")]
	public class TestBehaviour : MonoBehaviour
	{
		[EnumButtons]
		public TestEnum TestEnum;

		private void Start()
		{
			StartCoroutine(Increment());
		}

		private IEnumerator Increment()
		{
			while (true)
			{
				yield return new WaitForSeconds(1.0f);
			}
		}
	}
}
