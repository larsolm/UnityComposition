using PiRhoSoft.UtilityEngine;
using System.Collections.Generic;

namespace PiRhoSoft.CompositionEngine
{
	public interface IIndexedVariableStore : IVariableStore
	{
		int Count { get; }
		object GetItem(int index);
	}

	public static class IndexedVariableStore
	{
		public static VariableValue GetVariable(IIndexedVariableStore variables, string name)
		{
			if (name == nameof(IIndexedVariableStore.Count))
				return VariableValue.Create(variables.Count);

			return VariableValue.Empty;
		}

		public static SetVariableResult SetVariable(IIndexedVariableStore variables, string name, VariableValue value)
		{
			if (name == nameof(IIndexedVariableStore.Count)) return SetVariableResult.ReadOnly;
			else return SetVariableResult.NotFound;
		}

		public static IEnumerable<string> GetVariableNames(IIndexedVariableStore variables)
		{
			return new List<string> { nameof(IIndexedVariableStore.Count) };
		}
	}

	public class IndexedVariableStore<ItemType> : SerializedList<ItemType>, IIndexedVariableStore where ItemType : class
	{
		public object GetItem(int index) => index >= 0 && index < List.Count ? List[index] : null;
		public VariableValue GetVariable(string name) => IndexedVariableStore.GetVariable(this, name);
		public SetVariableResult SetVariable(string name, VariableValue value) => IndexedVariableStore.SetVariable(this, name, value);
		public IEnumerable<string> GetVariableNames() => IndexedVariableStore.GetVariableNames(this);
	}
}
