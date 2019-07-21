using System;
using UnityEngine;

namespace PiRhoSoft.Utilities
{
	[AddComponentMenu("PiRho Soft/Examples/Containers")]
	public class ContainersExample : MonoBehaviour
	{
		[Serializable] public class TestList : SerializedList<int> { }

		[List]
		public TestList List;
	}
}