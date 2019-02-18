using System;

namespace PiRhoSoft.CompositionEngine
{
	public class Property<OwnerType>
	{
		public string Name;
		public Func<OwnerType, VariableValue> Getter;
		public Func<OwnerType, VariableValue, SetVariableResult> Setter;
	}
}
