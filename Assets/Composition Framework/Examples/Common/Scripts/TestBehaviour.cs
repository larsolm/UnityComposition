using PiRhoSoft.Utilities;
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
		Eight = 0x8,
		Sixteen = 0x10,
		ThirtyTwo = 0x20,
		SixtyFour = 0x40,
		OneTwentyEight = 0x80
	}

	[Serializable]
	public class IntList : SerializedList<int> { }

	[Serializable]
	public class TestEnumList : SerializedList<TestEnum> { }

	[AddComponentMenu("PiRho Soft/Examples/Test")]
	public class TestBehaviour : MonoBehaviour
	{
		[Stretch]
		[ChangeTrigger(nameof(EnumChanged))]
		[EnumButtons]
		public TestEnum Enum;

		[List] [Stretch] [ChangeTrigger(nameof(EnumChanged))] [EnumButtons]
		public TestEnumList EnumList;

		private void Start()
		{
			StartCoroutine(Increment());
		}

		private IEnumerator Increment()
		{
			while (true)
			{
				yield return new WaitForSeconds(1.0f);
				Enum = TestEnum.OneTwentyEight;
			}
		}

		private static void EnumChanged(Enum oldValue, Enum newValue)
		{
			Debug.LogFormat("Enum changed from {0} to {1}", oldValue, newValue);
		}
	}
}
