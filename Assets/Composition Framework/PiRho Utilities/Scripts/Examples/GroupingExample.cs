using UnityEngine;

namespace PiRhoSoft.Utilities
{
	[AddComponentMenu("PiRho Soft/Examples/Grouping")]
	public class GroupingExample : MonoBehaviour
	{
		[Group("One")] public int Int1;
		[Group("One")] public float Float1;
		[Group("Two")] public float Float2;
		[Group("One")] public bool Bool1;
		[Group("Two")] public bool Bool2;
		[Group("Two")] public int Int2;
	}
}