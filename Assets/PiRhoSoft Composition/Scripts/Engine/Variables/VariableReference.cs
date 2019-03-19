using System;
using System.Text;
using PiRhoSoft.UtilityEngine;
using UnityEngine;
using Object = UnityEngine.Object;

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
		private const string _gameObjectName = "GameObject";

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
				return store.SetVariable(_variable[_variable.Length - 1], value);
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
				value = ResolveLookup(value, lookup);

			return value;
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

		#region Lookups

		public static VariableValue ResolveLookup(VariableValue value, string lookup)
		{
			switch (value.Type)
			{
				case VariableType.Int2: return ResolveInt2Lookup(value.Int2, lookup);
				case VariableType.Int3: return ResolveInt3Lookup(value.Int3, lookup);
				case VariableType.IntRect: return ResolveIntRectLookup(value.IntRect, lookup);
				case VariableType.IntBounds: return ResolveIntBoundsLookup(value.IntBounds, lookup);
				case VariableType.Vector2: return ResolveVector2Lookup(value.Vector2, lookup);
				case VariableType.Vector3: return ResolveVector3Lookup(value.Vector3, lookup);
				case VariableType.Vector4: return ResolveVector4Lookup(value.Vector4, lookup);
				case VariableType.Quaternion: return ResolveQuaternionLookup(value.Quaternion, lookup);
				case VariableType.Rect: return ResolveRectLookup(value.Rect, lookup);
				case VariableType.Bounds: return ResolveBoundsLookup(value.Bounds, lookup);
				case VariableType.Color: return ResolveColorLookup(value.Color, lookup);
				case VariableType.Object: return ResolveObjectLookup(value.Object, lookup);
				case VariableType.Store: return ResolveStoreLookup(value.Store, lookup);
				default: return VariableValue.Empty;
			}
		}

		public static VariableValue ResolveLookup(VariableValue value, int lookup)
		{
			if (value.Type == VariableType.Store && value.Store is IIndexedVariableStore indexed)
				return VariableValue.Create(indexed.GetItem(lookup));
			else
				return VariableValue.Empty;
		}

		private static VariableValue ResolveInt2Lookup(Vector2Int value, string lookup)
		{
			if (lookup == "x") return VariableValue.Create(value.x);
			else if (lookup == "y") return VariableValue.Create(value.y);
			else return VariableValue.Empty;
		}
		private static VariableValue ResolveInt3Lookup(Vector3Int value, string lookup)
		{
			if (lookup == "x") return VariableValue.Create(value.x);
			else if (lookup == "y") return VariableValue.Create(value.y);
			else if (lookup == "z") return VariableValue.Create(value.z);
			else return VariableValue.Empty;
		}

		private static VariableValue ResolveIntRectLookup(RectInt value, string lookup)
		{
			if (lookup == "x") return VariableValue.Create(value.x);
			else if (lookup == "y") return VariableValue.Create(value.y);
			else if (lookup == "w") return VariableValue.Create(value.width);
			else if (lookup == "h") return VariableValue.Create(value.height);
			else return VariableValue.Empty;
		}

		private static VariableValue ResolveIntBoundsLookup(BoundsInt value, string lookup)
		{
			if (lookup == "x") return VariableValue.Create(value.x);
			else if (lookup == "y") return VariableValue.Create(value.y);
			else if (lookup == "z") return VariableValue.Create(value.z);
			else if (lookup == "w") return VariableValue.Create(value.size.x);
			else if (lookup == "h") return VariableValue.Create(value.size.y);
			else if (lookup == "d") return VariableValue.Create(value.size.z);
			else return VariableValue.Empty;
		}

		private static VariableValue ResolveVector2Lookup(Vector2 value, string lookup)
		{
			if (lookup == "x") return VariableValue.Create(value.x);
			else if (lookup == "y") return VariableValue.Create(value.y);
			else return VariableValue.Empty;
		}

		private static VariableValue ResolveVector3Lookup(Vector3 value, string lookup)
		{
			if (lookup == "x") return VariableValue.Create(value.x);
			else if (lookup == "y") return VariableValue.Create(value.y);
			else if (lookup == "z") return VariableValue.Create(value.z);
			else return VariableValue.Empty;
		}

		private static VariableValue ResolveVector4Lookup(Vector4 value, string lookup)
		{
			if (lookup == "x") return VariableValue.Create(value.x);
			else if (lookup == "y") return VariableValue.Create(value.y);
			else if (lookup == "z") return VariableValue.Create(value.z);
			else if (lookup == "w") return VariableValue.Create(value.w);
			else return VariableValue.Empty;
		}

		private static VariableValue ResolveQuaternionLookup(Quaternion value, string lookup)
		{
			var angles = value.eulerAngles;

			if (lookup == "x") return VariableValue.Create(angles.x);
			else if (lookup == "y") return VariableValue.Create(angles.y);
			else if (lookup == "z") return VariableValue.Create(angles.z);
			else return VariableValue.Empty;
		}

		private static VariableValue ResolveRectLookup(Rect value, string lookup)
		{
			if (lookup == "x") return VariableValue.Create(value.x);
			else if (lookup == "y") return VariableValue.Create(value.y);
			else if (lookup == "w") return VariableValue.Create(value.width);
			else if (lookup == "h") return VariableValue.Create(value.height);
			else return VariableValue.Empty;
		}

		private static VariableValue ResolveBoundsLookup(Bounds value, string lookup)
		{
			if (lookup == "x") return VariableValue.Create(value.min.x);
			else if (lookup == "y") return VariableValue.Create(value.min.y);
			else if (lookup == "z") return VariableValue.Create(value.min.z);
			else if (lookup == "w") return VariableValue.Create(value.size.x);
			else if (lookup == "h") return VariableValue.Create(value.size.y);
			else if (lookup == "d") return VariableValue.Create(value.size.z);
			else return VariableValue.Empty;
		}

		private static VariableValue ResolveColorLookup(Color value, string lookup)
		{
			if (lookup == "r") return VariableValue.Create(value.r);
			else if (lookup == "g") return VariableValue.Create(value.g);
			else if (lookup == "b") return VariableValue.Create(value.b);
			else if (lookup == "a") return VariableValue.Create(value.a);
			else return VariableValue.Empty;
		}

		private static VariableValue ResolveObjectLookup(Object obj, string lookup)
		{
			if (char.IsDigit(lookup[0]))
			{
				if (obj is IIndexedVariableStore indexed)
					return ResolveStoreLookup(indexed, lookup);
				else
					return VariableValue.Empty;
			}
			else
			{
				if (lookup == _gameObjectName)
				{
					var gameObject = ComponentHelper.GetAsGameObject(obj);
					return VariableValue.Create(gameObject);
				}
				else
				{
					var component = ComponentHelper.GetAsComponent(obj, lookup);
					return VariableValue.Create(component);
				}
			}
		}

		private static VariableValue ResolveStoreLookup(IVariableStore store, string lookup)
		{
			if (store is IIndexedVariableStore indexed)
				return ResolveStoreLookup(indexed, lookup);
			else
				return VariableValue.Empty;
		}

		private static VariableValue ResolveStoreLookup(IIndexedVariableStore store, string lookup)
		{
			if (int.TryParse(lookup, out var index))
				return VariableValue.Create(store.GetItem(index));
			else
				return VariableValue.Empty;
		}

		#endregion
	}
}
