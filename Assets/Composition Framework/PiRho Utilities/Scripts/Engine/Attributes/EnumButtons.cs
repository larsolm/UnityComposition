using UnityEngine;

namespace PiRhoSoft.Utilities.Engine
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
