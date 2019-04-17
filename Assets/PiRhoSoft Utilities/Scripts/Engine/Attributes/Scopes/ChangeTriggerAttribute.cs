﻿namespace PiRhoSoft.UtilityEngine
{
	public class ChangeTriggerAttribute : PropertyScopeAttribute
	{
		public string Callback { get; private set; }

		public ChangeTriggerAttribute(string callback) : base(int.MaxValue - 10) => Callback = callback;
	}
}
