using UnityEngine;

namespace PiRhoSoft.Utilities
{
	public enum GroupStyle
	{
		Frame,
		Rollout,
		Foldout,
	}

	public class GroupAttribute : PropertyTraitAttribute
	{
		public const int Order = 0;

		public string Name { get; private set; }
		public GroupStyle Style { get; set; }

		public GroupAttribute(string name) : base(Order)
		{
			Name = name;
		}
	}
}