using UnityEngine;

namespace PiRhoSoft.PargonUtilities.Engine
{
	public class ListAttribute : PropertyAttribute
	{
		public bool AllowAdd = true;
		public bool AllowRemove = true;
		public bool AllowReorder = true;
		public string EmptyLabel = null;
	}
}