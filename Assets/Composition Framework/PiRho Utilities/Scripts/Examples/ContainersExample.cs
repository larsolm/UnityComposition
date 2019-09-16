using PiRhoSoft.Composition;
using System;
using UnityEngine;

namespace PiRhoSoft.Utilities
{
	[AddComponentMenu("PiRho Soft/Examples/Containers")]
	public class ContainersExample : MonoBehaviour
	{
		[Serializable] public class TestList : SerializedList<int> { }
		[Serializable] public class TestDictionary : SerializedDictionary<string, string> { }

		[List]
		[Tooltip("A test list")]
		public TestList List;

		[Dictionary]
		[Stretch]
		[Tooltip("A test dictionary")]
		public TestDictionary Dictionary;
	}
}