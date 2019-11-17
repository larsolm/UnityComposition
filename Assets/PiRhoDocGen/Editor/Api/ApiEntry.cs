using System;

namespace PiRhoSoft.DocGen.Editor
{
	public abstract class ApiEntry
	{
		public string Id;
		public string OwnerId;
		public string Name;
		public string Modifiers;

		public Modifier ModifierEnum
		{
			get => (Modifier)Enum.Parse(typeof(Modifier), Modifiers);
			set => Modifiers = value.ToString();
		}
	}
}
