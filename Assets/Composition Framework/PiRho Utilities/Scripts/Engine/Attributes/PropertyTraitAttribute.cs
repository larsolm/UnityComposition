using UnityEngine;

namespace PiRhoSoft.Utilities
{
	public abstract class PropertyTraitAttribute : PropertyAttribute
	{
		protected PropertyTraitAttribute(int drawOrder)
		{
			order = int.MaxValue - drawOrder;
		}
	}
}