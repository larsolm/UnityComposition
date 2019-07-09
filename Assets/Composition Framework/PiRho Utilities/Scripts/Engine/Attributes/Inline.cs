using UnityEngine;

namespace PiRhoSoft.Utilities.Engine
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
