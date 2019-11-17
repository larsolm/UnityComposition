using System;

namespace PiRhoSoft.DocGen.Editor
{
	[Flags]
	public enum Modifier
	{
		None = 0,
		All = ~0,

		// Declaration
		Namespace = 0x1,
		Class = 0x2,
		Struct = 0x4,
		Enum = 0x8,
		Interface = 0x10,

		// Member
		Constructor = 0x20,
		Field = 0x40,
		Property = 0x80,
		Method = 0x100,

		// Argument
		Template = 0x200,
		Parameter = 0x400,

		// Access
		Public = 0x800,
		Protected = 0x1000,
		Private = 0x2000,
		Internal = 0x4000,

		// Inheritance
		Abstract = 0x8000,
		Virtual = 0x10000,
		Override = 0x20000,
		Sealed = 0x40000,

		// Other
		Static = 0x80000,
		Const = 0x100000,
		ReadOnly = 0x200000,
		Serializable = 0x400000,
		Reference = 0x800000,
		Output = 0x1000000,

		// Unity
		Behaviour = 0x2000000,
		Asset = 0x4000000
	}
}
