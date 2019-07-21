using UnityEngine;

namespace PiRhoSoft.Utilities
{
	public class DictionaryAttribute : PropertyAttribute
	{
		public const int Order = 100;

		public bool AllowAdd = true;
		public bool AllowRemove = true;
		public string EmptyLabel = null;

		public DictionaryAttribute()
		{
			order = int.MaxValue - Order;
		}
	}
}