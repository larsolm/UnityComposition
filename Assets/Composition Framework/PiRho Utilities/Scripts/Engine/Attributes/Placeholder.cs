using UnityEngine;

namespace PiRhoSoft.Utilities
{
	public class PlaceholderAttribute : PropertyAttribute
	{
		public string Placeholder { get; private set; }

		public PlaceholderAttribute(string placeholder)
		{
			Placeholder = placeholder;
		}
	}
}
