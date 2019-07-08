using UnityEngine;

namespace PiRhoSoft.PargonUtilities.Engine
{
	public abstract class PropertyTraitAttribute : PropertyAttribute
	{
		protected PropertyTraitAttribute(int drawOrder)
		{
			order = drawOrder;
		}
	}
}
