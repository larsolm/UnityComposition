using UnityEngine;

namespace PiRhoSoft.Utilities
{
	public class ListAttribute : PropertyAttribute
	{
		public const int Order = 100;

		public bool AllowAdd = true;
		public bool AllowRemove = true;
		public bool AllowReorder = true;
		public string EmptyLabel = null;

		public ListAttribute()
		{
			order = int.MaxValue - Order;
		}
	}
}