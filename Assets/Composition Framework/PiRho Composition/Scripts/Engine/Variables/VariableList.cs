using System.Collections.Generic;

namespace PiRhoSoft.Composition.Engine
{
	public interface IVariableList
	{
		int Count { get; }
		VariableValue GetVariable(int index);
		SetVariableResult SetVariable(int index, VariableValue value);
		SetVariableResult AddVariable(VariableValue value);
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
				Values.Add(VariableValue.Empty);
		}

		public List<VariableValue> Values { get; private set; } = new List<VariableValue>();
		public int Count => Values.Count;

		public VariableValue GetVariable(int index)
		{
			return index >= 0 && index < Values.Count ? Values[index] : VariableValue.Empty;
		}

		public SetVariableResult AddVariable(VariableValue value)
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

		public SetVariableResult SetVariable(int index, VariableValue value)
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
