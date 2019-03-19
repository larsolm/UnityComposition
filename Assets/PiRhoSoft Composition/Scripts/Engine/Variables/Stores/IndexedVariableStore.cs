using PiRhoSoft.UtilityEngine;
using System.Collections.Generic;

namespace PiRhoSoft.CompositionEngine
{
	public interface IIndexedVariableStore : IVariableStore
	{
		int Count { get; }
		object GetItem(int index);
		bool AddItem(object item);
		bool RemoveItem(int index);
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
		public object GetItem(int index)
		{
			if (index >= 0 && index < List.Count)
				return List[index];
			else
				return null;
		}

		public bool AddItem(object item)
		{
			if (item is ItemType typed)
			{
				List.Add(typed);
				return true;
			}

			return false;
		}

		public bool RemoveItem(int index)
		{
			if (index >= 0 && index < List.Count)
			{
				List.RemoveAt(index);
				return true;
			}

			return false;
		}

		public VariableValue GetVariable(string name) => IndexedVariableStore.GetVariable(this, name);
		public SetVariableResult SetVariable(string name, VariableValue value) => IndexedVariableStore.SetVariable(this, name, value);
		public IEnumerable<string> GetVariableNames() => IndexedVariableStore.GetVariableNames(this);
	}
}
