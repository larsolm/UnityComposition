using System;
using UnityEngine;

namespace PiRhoSoft.Utilities
{
	[AddComponentMenu("PiRho Soft/Examples/Class")]
	public class ClassExample : MonoBehaviour
	{
		[Serializable]
		public class Subclass
		{
			public bool Bool;
			public int Int;
			public float Float;
			public string String;
		}

		[Rollout]
		public Subclass Rollout;

		[Inline]
		public Subclass Inline;
	}
}