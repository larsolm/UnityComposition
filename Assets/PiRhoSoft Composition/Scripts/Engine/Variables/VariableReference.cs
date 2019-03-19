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
				value = LookupVariable(value, lookup);

			return value;
		}

		private SetVariableResult SetValue(IVariableStore variables, int index, VariableValue value)
		{
			var variable = _variable[index];
			var lookup = _lookups[index];

			if (!string.IsNullOrEmpty(lookup))
			{
				var owner = variables.GetVariable(variable);
				var result = ApplyVariable(ref owner, lookup, value);

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

		#region Variable Resolving

		private static VariableResolver[] _resolvers;

		static VariableReference()
		{
			_resolvers = new VariableResolver[(int)(VariableType.Other + 1)];
			_resolvers[(int)VariableType.Int2] = new Int2VariableResolver();
			_resolvers[(int)VariableType.Int3] = new Int3VariableResolver();
			_resolvers[(int)VariableType.IntRect] = new IntRectVariableResolver();
			_resolvers[(int)VariableType.IntBounds] = new IntBoundsVariableResolver();
			_resolvers[(int)VariableType.Vector2] = new Vector2VariableResolver();
			_resolvers[(int)VariableType.Vector3] = new Vector3VariableResolver();
			_resolvers[(int)VariableType.Vector4] = new Vector4VariableResolver();
			_resolvers[(int)VariableType.Quaternion] = new QuaternionVariableResolver();
			_resolvers[(int)VariableType.Rect] = new RectVariableResolver();
			_resolvers[(int)VariableType.Bounds] = new BoundsVariableResolver();
			_resolvers[(int)VariableType.Color] = new ColorVariableResolver();
			_resolvers[(int)VariableType.Object] = new ObjectVariableResolver();
			_resolvers[(int)VariableType.Store] = new StoreVariableResolver();
		}

		public static VariableValue LookupVariable(VariableValue owner, string lookup)
		{
			var resolver = _resolvers[(int)owner.Type];

			if (resolver != null)
				return resolver.Lookup(owner, lookup);
			else
				return VariableValue.Empty;
		}

		public static SetVariableResult ApplyVariable(ref VariableValue owner, string lookup, VariableValue value)
		{
			var resolver = _resolvers[(int)owner.Type];

			if (resolver != null)
				return resolver.Apply(ref owner, lookup, value);
			else
				return SetVariableResult.NotFound;
		}

		#endregion
	}
}
