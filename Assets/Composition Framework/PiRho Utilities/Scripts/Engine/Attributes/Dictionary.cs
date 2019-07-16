using UnityEngine;

namespace PiRhoSoft.Utilities
{
	public class DictionaryAttribute : PropertyAttribute
	{
		public bool AllowAdd = true;
		public bool AllowRemove = true;

		public string AddLabel = null;
		public string EmptyText = "Dictionary is Empty";
	}
}