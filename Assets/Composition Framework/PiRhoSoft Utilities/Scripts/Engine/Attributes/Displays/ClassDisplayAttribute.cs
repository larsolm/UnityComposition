using UnityEngine;

namespace PiRhoSoft.UtilityEngine
{
	public enum ClassDisplayType
	{
		Indented,
		Inline,
		Propogated,
		Foldout
	}

	public class ClassDisplayAttribute : PropertyAttribute
	{
		public ClassDisplayType Type { get; private set; }

		public ClassDisplayAttribute(ClassDisplayType type) => Type = type;
	}
}
