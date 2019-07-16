using UnityEngine;

namespace PiRhoSoft.Utilities
{
	public class EnumButtonsAttribute : PropertyAttribute
	{
		public bool Flags { get; private set; }

		public EnumButtonsAttribute(bool flags = false)
		{
			Flags = flags;
		}
	}
}
