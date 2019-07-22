using UnityEngine;

namespace PiRhoSoft.Utilities
{
	[AddComponentMenu("PiRho Soft/Examples/Configuration")]
	public class ConfigurationExample : MonoBehaviour
	{
		[Multiline]
		public string MultilineText;

		[ReadOnly]
		public bool ReadOnly;

		[Stretch]
		public string Stretch;

		[Multiline] [Stretch]
		public string MultilineStretch;

		[CustomLabel("Show/Hide")]
		public bool Toggle;

		[Conditional(nameof(Toggle), true)]
		public int ConditionalInt;

		[Placeholder("placeholder")]
		public string Placeholder;
	}
}