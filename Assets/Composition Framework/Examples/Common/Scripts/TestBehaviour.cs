using PiRhoSoft.Utilities;
using System;
using System.Collections;
using UnityEngine;

namespace PiRhoSoft.CompositionExample
{
	[Flags]
	public enum TestEnum
	{
		Zero = 0x0,
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
	public class Subclass
	{
		[ChangeTrigger(nameof(IntChanged))]
		public int Int;

		[EnumButtons]
		public TestEnum Enum;

		private static void IntChanged(int oldValue, int newValue)
		{
			Debug.Log($"{nameof(Int)} changed from {oldValue} to {newValue}");
		}
	}

	[Serializable]
	public class IntList : SerializedList<int> { }

	[Serializable]
	public class TestEnumList : SerializedList<TestEnum> { }

	[AddComponentMenu("PiRho Soft/Examples/Test")]
	public class TestBehaviour : MonoBehaviour
	{
		[Conditional(nameof(Bool1), true)] public int Int1;
		[CustomLabel("Float1 (ms)")] public float Float1;
		public float Float2;
		public bool Bool1;
		public bool Bool2;
		public int Int2;

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

		private void Trigger()
		{
			Debug.Log("Clicked");
		}
	}
}
