using System;
using System.Text;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[Serializable]
	public class VariableReference
	{
		public const char Separator = '.';
		public const char LookupOpen = '<';
		public const char LookupClose = '>';

		private static string[] _emptyVariable = new string[0];
		private static string[] _emptyLookups = new string[0];
		private static char[] _splitter = new char[] { Separator };
		private static string _joiner = _splitter[0].ToString();

		[SerializeField] private string[] _variable = _emptyVariable;
		[SerializeField] private string[] _lookups = _emptyLookups;

		public bool IsAssigned => _variable != null && _variable.Length > 0;
		public string StoreName => _variable != null && _variable.Length > 1 ? _variable[0] : string.Empty;
		public string RootName => IsAssigned ? (_variable.Length > 1 ? _variable[1] : _variable[0]) : null;

		public VariableReference()
		{
		}

		public VariableReference(string variable)
		{
			Update(variable);
		}

		public void Update(string variable)
		{
			if (string.IsNullOrEmpty(variable))
			{
				_variable = _emptyVariable;
				_lookups = _emptyLookups;
			}
			else
			{
				_variable = variable.Split(_splitter);
				_lookups = new string[_variable.Length];

				for (var i = 0; i < _variable.Length; i++)
				{
					var v = _variable[i];
					var open = v.IndexOf(LookupOpen);
					var close = v.IndexOf(LookupClose);

					// the invalid case will result in a variable name including lookup tokens which will ultimately
					// print an error when the lookup fails

					if (open > 0 && close == v.Length - 1)
					{
						_variable[i] = v.Substring(0, open);
						_lookups[i] = v.Substring(open + 1, close - open - 1);
					}
				}
			}
		}

		public VariableValue GetValue(IVariableStore variables)
		{
			var store = GetStore(variables);

			if (store != null)
				return GetValue(store, _variable.Length - 1);
			else
				return VariableValue.Empty;
		}

		public SetVariableResult SetValue(IVariableStore variables, VariableValue value)
		{
			var store = GetStore(variables);

			if (store != null)
				return SetValue(store, _variable.Length - 1, value);
			else
				return SetVariableResult.NotFound;
		}

		private IVariableStore GetStore(IVariableStore variables)
		{
			if (IsAssigned)
			{
				var store = variables;

				for (var i = 0; i < _variable.Length - 1 && store != null; i++)
					 store = GetValue(store, i).Store;

				return store;
			}

			return null;
		}

		private VariableValue GetValue(IVariableStore variables, int index)
		{
			var variable = _variable[index];
			var lookup = _lookups[index];

			var value = variables.GetVariable(variable);

			if (!string.IsNullOrEmpty(lookup))
				value = value.Handler.Lookup(value, lookup);

			return value;
		}

		private SetVariableResult SetValue(IVariableStore variables, int index, VariableValue value)
		{
			var variable = _variable[index];
			var lookup = _lookups[index];

			if (!string.IsNullOrEmpty(lookup))
			{
				var owner = variables.GetVariable(variable);
				var result = owner.Handler.Apply(ref owner, lookup, value);

				if (result == SetVariableResult.Success)
					value = owner;
				else
					return result;
			}

			return variables.SetVariable(variable, value);
		}

		public override string ToString()
		{
			var builder = new StringBuilder();

			for (var i = 0; i < _variable.Length; i++)
			{
				if (i != 0)
					builder.Append(Separator);

				builder.Append(_variable[i]);

				if (!string.IsNullOrEmpty(_lookups[i]))
					builder.AppendFormat("{0}{1}{2}", LookupOpen, _lookups[i], LookupClose);
			}

			return builder.ToString();
		}
	}
}
