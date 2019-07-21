using UnityEngine;

namespace PiRhoSoft.Utilities
{
	public enum TraitLocation
	{
		Before,
		After
	}

	public enum TraitMessageType
	{
		Info,
		Warning,
		Error
	}

	public abstract class PropertyTraitAttribute : PropertyAttribute
	{
		protected PropertyTraitAttribute(int drawOrder)
		{
			order = int.MaxValue - drawOrder;
		}
	}
}