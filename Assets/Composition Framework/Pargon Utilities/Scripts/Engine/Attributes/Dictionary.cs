using UnityEngine;

namespace PiRhoSoft.PargonUtilities.Engine
{
	public class DictionaryAttribute : PropertyAttribute
	{
		public DictionaryAttribute()
		{
			order = int.MaxValue - 100;
		}
	}
}