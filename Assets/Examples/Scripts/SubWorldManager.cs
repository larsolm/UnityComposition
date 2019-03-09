using PiRhoSoft.CompositionEngine;
using UnityEngine;

namespace PiRhoSoft.CompositionExample
{
	[AddComponentMenu("PiRho Soft/Examples/Sub World Manager")]
	public class SubWorldManager : WorldManager
	{
		[MappedVariable] public int IntField = 7;
		[MappedVariable] public int IntProperty { get; set; }

		[MappedVariable] public float FloatField = 17.0f;
		[MappedVariable] public float FloatProperty { get; set; }

		[MappedVariable] public string StringField = "Hello";
		[MappedVariable] public string StringProperty { get; set; }

		public override void OnEnable()
		{
			Store.Setup(this, Schema, Variables);
		}
	}
}
