using UnityEngine;

namespace PiRhoSoft.Utilities
{
	public class InlineAttribute : PropertyAttribute
	{
		public bool ShowMemberLabels { get; private set; }

		public InlineAttribute(bool showMemberLabels = false)
		{
			ShowMemberLabels = showMemberLabels;
		}
	}
}
