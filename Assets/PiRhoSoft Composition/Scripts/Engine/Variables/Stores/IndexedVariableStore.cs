using PiRhoSoft.UtilityEngine;

namespace PiRhoSoft.CompositionEngine
{
	public interface IIndexedVariableStore : IVariableStore
	{
		int Count { get; }
		IVariableStore GetItem(int index);
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
				var index = ParseIndex(name, ItemVariable.Length, name.Length - ItemVariable.Length);
				return VariableValue.Create(variables.GetItem(index));
			}

			return VariableValue.Empty;
		}

		public static SetVariableResult SetVariable(IIndexedVariableStore variables, string name, VariableValue value)
		{
			if (name == nameof(IIndexedVariableStore.Count) || name.StartsWith(ItemVariable)) return SetVariableResult.ReadOnly;
			else return SetVariableResult.NotFound;
		}

		private static int ParseIndex(string s, int start, int end)
		{
			// this exists as a way to parse an integer without having to instantiate a substring first

			var value = 0;
			for (var i = start; i < end; i++)
				value = value * 10 + (s[i] - '0');

			return value;
		}
	}

	public class IndexedVariableStore<ItemType> : SerializedList<ItemType>, IIndexedVariableStore where ItemType : class, IVariableStore
	{
		public IVariableStore GetItem(int index) => index >= 0 && index < List.Count ? List[index] : null;
		public VariableValue GetVariable(string name) => IndexedVariableStore.GetVariable(this, name);
		public SetVariableResult SetVariable(string name, VariableValue value) => IndexedVariableStore.SetVariable(this, name, value);
	}
}
