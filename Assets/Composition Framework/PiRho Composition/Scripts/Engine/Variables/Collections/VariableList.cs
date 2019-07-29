using System.Collections.Generic;

namespace PiRhoSoft.Composition
{
	public interface IVariableList : IVariableArray
	{
		SetVariableResult AddVariable(Variable value);
		SetVariableResult RemoveVariable(int index);
	}

	public class VariableList : IVariableList
	{
		public VariableList()
		{
		}

		public VariableList(int count)
		{
			for (var i = 0; i < count; i++)
				Values.Add(Variable.Empty);
		}

		public List<Variable> Values { get; private set; } = new List<Variable>();
		public int VariableCount => Values.Count;

		public Variable GetVariable(int index)
		{
			return index >= 0 && index < Values.Count ? Values[index] : Variable.Empty;
		}

		public SetVariableResult AddVariable(Variable value)
		{
			Values.Add(value);
			return SetVariableResult.Success;
		}

		public SetVariableResult RemoveVariable(int index)
		{
			if (index >= 0 && index < Values.Count)
			{
				Values.RemoveAt(index);
				return SetVariableResult.Success;
			}

			return SetVariableResult.NotFound;
		}

		public SetVariableResult SetVariable(int index, Variable value)
		{
			if (index >= 0 && index < Values.Count)
			{
				Values[index] = value;
				return SetVariableResult.Success;
			}

			return SetVariableResult.NotFound;
		}
	}
}
