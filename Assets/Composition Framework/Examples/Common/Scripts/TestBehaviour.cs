using PiRhoSoft.PargonUtilities.Engine;
using System;
using System.Collections;
using System.Collections.Generic;
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

	[Serializable]
	public class IntList : SerializedList<int> { }

	[AddComponentMenu("PiRho Soft/Examples/Test")]
	public class TestBehaviour : MonoBehaviour
	{
		[EnumButtons]
		public TestEnum TestEnum = TestEnum.Two;

		//[Euler]
		public int TestInt = 5;

		[Euler]
		public Quaternion TestQuaternion = Quaternion.Euler(45, 0, 0);

		[List]
		public IntList TestList = new IntList { 0, 1, 2, 3, 4, 5 };
		
		[Euler]
		public List<Quaternion> TestList2 = new List<Quaternion>();

		private void Start()
		{
			StartCoroutine(Increment());
		}

		private IEnumerator Increment()
		{
			while (TestList.Count > 1)
			{
				yield return new WaitForSeconds(2.0f);
				TestList.RemoveAt(1);
			}
		}
	}
}
