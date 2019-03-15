using PiRhoSoft.UtilityEngine;
using System.Collections.Generic;
using System.Linq;

namespace PiRhoSoft.CompositionEngine
{
	public interface IIndexedVariableStore : IVariableStore
	{
		int Count { get; }
		object GetItem(int index);
	}

	public static class IndexedVariableStore
	{
		public const string ItemVariable = "Item";

		public static VariableValue GetVariable(IIndexedVariableStore variables, string name)
		{
			if (name == nameof(IIndexedVariableStore.Count))
			{
				return VariableValue.Create(variables.Count);
			}
			else if (name.StartsWith(ItemVariable))
			{
				var index = ParseIndex(name, ItemVariable.Length, name.Length);
				return VariableValue.Create(variables.GetItem(index));
			}

			return VariableValue.Empty;
		}

		public static SetVariableResult SetVariable(IIndexedVariableStore variables, string name, VariableValue value)
		{
			if (name == nameof(IIndexedVariableStore.Count) || name.StartsWith(ItemVariable)) return SetVariableResult.ReadOnly;
			else return SetVariableResult.NotFound;
		}

		public static IEnumerable<string> GetVariableNames(IIndexedVariableStore variables)
		{
			return Enumerable
				.Repeat("", variables.Count)
				.Select((value, index) => ItemVariable + index)
				.Concat(Enumerable.Repeat(nameof(IIndexedVariableStore.Count), 1));
		}

		private static int ParseIndex(string s, int start, int length)
		{
			// this exists as a way to parse an integer without having to instantiate a substring first

			var value = 0;
			for (var i = start; i < length; i++)
				value = value * 10 + (s[i] - '0');

			return value;
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
