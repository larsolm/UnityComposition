using UnityEngine;

namespace PiRhoSoft.Utilities
{
	public class EnumButtonsAttribute : PropertyAttribute
	{
		public bool? Flags { get; private set; }

		public EnumButtonsAttribute() => Flags = null;
		public EnumButtonsAttribute(bool flags) => Flags = flags;
	}
}