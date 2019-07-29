using System.Collections;
using System.Collections.Generic;

namespace PiRhoSoft.Composition
{
	public class DictionaryAdapter : IVariableDictionary
	{
		private IDictionary _dictionary;

		private bool _allowSet;
		private bool _allowAdd;
		private bool _allowRemove;

		public DictionaryAdapter(IDictionary dictionary)
		{
			_dictionary = dictionary;

			_allowSet = !dictionary.IsReadOnly;
			_allowAdd = !dictionary.IsReadOnly && !dictionary.IsFixedSize;
			_allowRemove = !dictionary.IsReadOnly && !dictionary.IsFixedSize;
		}

		public VariableSchema Schema => null;
		public IReadOnlyList<string> VariableNames => (IReadOnlyList<string>)_dictionary.Keys;

		public Variable GetVariable(string name)
		{
			if (_dictionary.Contains(name))
				return Get(name);
			else
				return Variable.Empty;
		}

		public SetVariableResult SetVariable(string name, Variable value)
		{
			if (!_allowSet)
			{
				return SetVariableResult.ReadOnly;
			}
			else if (_dictionary.Contains(name))
			{
				try
				{
					Set(name, value);
					return SetVariableResult.Success;
				}
				catch
				{
					return SetVariableResult.TypeMismatch;
				}
			}

			return SetVariableResult.NotFound;
		}

		protected Variable Get(string key) => Variable.Unbox(_dictionary[key]);
		protected void Set(string key, Variable value) => _dictionary[key] = value.Box();
		protected void Add(string key, Variable value) => _dictionary.Add(key, value.Box());
		protected void Remove(string key) => _dictionary.Remove(key);
	}
}