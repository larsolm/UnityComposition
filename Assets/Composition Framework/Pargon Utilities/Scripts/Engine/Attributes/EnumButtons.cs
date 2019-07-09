using UnityEngine;

namespace PiRhoSoft.PargonUtilities.Engine
{
	public class EnumButtonsAttribute : PropertyAttribute
	{
		public bool? Flags { get; private set; }

		public EnumButtonsAttribute() => Flags = null;
		public EnumButtonsAttribute(bool flags) => Flags = flags;
	}
}
