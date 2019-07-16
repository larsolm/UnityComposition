using UnityEngine;

namespace PiRhoSoft.Utilities
{
	public class ListAttribute : PropertyAttribute
	{
		public bool AllowAdd = true;
		public bool AllowRemove = true;
		public bool AllowReorder = true;

		public string EmptyLabel = "List is empty";
	}
}