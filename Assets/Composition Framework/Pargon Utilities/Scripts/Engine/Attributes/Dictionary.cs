using UnityEngine;

namespace PiRhoSoft.PargonUtilities.Engine
{
	public class DictionaryAttribute : PropertyAttribute
	{
		public bool AllowAdd = true;
		public bool AllowRemove = true;

		public string AddLabel = null;
		public string EmptyText = "Dictionary is Empty";

		public DictionaryAttribute()
		{
			order = int.MaxValue - 100;
		}
	}
}