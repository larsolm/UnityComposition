using UnityEngine;

namespace PiRhoSoft.Utilities.Engine
{
	public abstract class PropertyTraitAttribute : PropertyAttribute
	{
		protected PropertyTraitAttribute(int drawOrder)
		{
			order = drawOrder;
		}
	}
}
