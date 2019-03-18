using System;
using Object = UnityEngine.Object;

namespace PiRhoSoft.CompositionEngine
{
	public struct Variable
	{
		public string Name { get; private set; }
		public VariableValue Value { get; private set; }

		public static Variable Empty => Create(string.Empty, VariableValue.Empty);

		public static Variable Create(string name, VariableValue value)
		{
			return new Variable
			{
				Name = name,
				Value = value
			};
		}
	}

	[Serializable]
	public class SerializedVariable
	{
		public string Name;
		public VariableType Type;
		public string Data;
		public Object Object;

		public void SetVariable(Variable variable)
		{
			Name = variable.Name;
			SetValue(variable.Value);
		}

		public void SetValue(VariableValue value)
		{
			Type = value.Type;
			Data = value.Write();
			Object = value.Object;
		}

		public Variable GetVariable()
		{
			return Variable.Create(Name, GetValue());
		}

		public VariableValue GetValue()
		{
			if (Type == VariableType.Object)
			{
				return VariableValue.Create(Object);
			}
			else
			{
				var value = VariableValue.Create(Type);
				value.Read(Data);
				return value;
			}
		}
	}
}
