﻿namespace PiRhoSoft.PargonUtilities.Engine
{
	public class ReadOnlyAttribute : PropertyTraitAttribute
	{
		public string Label { get; private set; }

		public ReadOnlyAttribute() : base(int.MaxValue - 10)
		{
		}
	}
}