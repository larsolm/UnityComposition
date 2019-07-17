using PiRhoSoft.Utilities;
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
		[EnumButtons]
		public TestEnum Enum;

		[List] [Stretch] [EnumButtons]
		public TestEnumList EnumList;

		[Stretch]
		public int IntList;

		[TypePicker(typeof(Component), false)]
		public string TypeTest;

		[ObjectPicker]
		[Required(MessageBoxType.Error, "Required")]
		public Texture ObjectTest;

		[ScenePicker]
		public SceneReference SceneText;

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
	}
}
