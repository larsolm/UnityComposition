﻿namespace PiRhoSoft.Utilities
{
	public class ChangeTriggerAttribute : PropertyTraitAttribute
	{
		public string Method { get; private set; }

		public ChangeTriggerAttribute(string method) : base(FieldPhase, 0)
		{
			Method = method;
		}
	}
}