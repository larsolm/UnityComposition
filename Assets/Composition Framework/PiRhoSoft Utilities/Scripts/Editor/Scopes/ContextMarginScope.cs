using UnityEngine;

namespace PiRhoSoft.UtilityEditor
{
	public class ContextMarginScope : GUI.Scope
	{
		private float _margin;

		public ContextMarginScope(float margin)
		{
			_margin = RectHelper.ContextMargin;
			RectHelper.ContextMargin = margin;
		}

		protected override void CloseScope()
		{
			// This should be resetting to _margin, but because there are situations where a GUI Scope does not get
			// disposed (like when an object picker is double clicked) ContextMargin can become permanently enabled
			RectHelper.ContextMargin = 0;
		}
	}
}
