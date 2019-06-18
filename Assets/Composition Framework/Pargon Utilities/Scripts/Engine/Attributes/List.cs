using UnityEngine;

namespace PiRhoSoft.PargonUtilities.Engine
{
	public class ListAttribute : PropertyAttribute
	{
		public ListAttribute()
		{
			order = int.MaxValue - 100;
		}
	}
}