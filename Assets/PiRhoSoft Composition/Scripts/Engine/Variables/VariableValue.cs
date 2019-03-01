using System;
using PiRhoSoft.UtilityEngine;
using Object = UnityEngine.Object;

namespace PiRhoSoft.CompositionEngine
{
	public enum VariableType
	{
		Empty,
		Boolean,
		Integer,
		Number,
		String,
		Object,
		Store,
		Null
	}

	public struct VariableValue : IEquatable<VariableValue>, IEquatable<bool>, IEquatable<int>, IEquatable<float>, IEquatable<string>, IComparable<VariableValue>, IComparable<bool>, IComparable<int>, IComparable<float>, IComparable<string>
	{
		public VariableType Type { get; private set; }

		public bool Boolean { get; private set; }
		public int Integer { get; private set; }
		public float Number { get; private set; }
		public string String { get; private set; }
		private object _object;

		public object RawObject => _object;
		public Object Object => _object as Object;
		public IVariableStore Store => _object as IVariableStore;

		public override string ToString()
		{
			switch (Type)
			{
				case VariableType.Empty: return "(empty)";
				case VariableType.Boolean: return Boolean.ToString();
				case VariableType.Integer: return Integer.ToString();
				case VariableType.Number: return Number.ToString();
				case VariableType.String: return String;
				case VariableType.Object: return Object != null ? Object.name : "(object)";
				case VariableType.Store: return "(store)";
				case VariableType.Null: return "(null)";
			}

			return "(unknown)";
		}

		#region Creation

		public static VariableValue Empty;

		public static VariableType GetType(Type type)
		{
			if (type == typeof(bool)) return VariableType.Boolean;
			else if (type == typeof(int)) return VariableType.Integer;
			else if (type == typeof(float)) return VariableType.Number;
			else if (type == typeof(string)) return VariableType.String;
			else if (typeof(Object).IsAssignableFrom(type)) return VariableType.Object;
			else if (typeof(IVariableStore).IsAssignableFrom(type)) return VariableType.Store;

			return VariableType.Empty;
		}

		public static VariableValue Create(VariableType type)
		{
			var variable = new VariableValue();
			variable.Type = type;
			return variable;
		}

		public static VariableValue Create<T>(T value)
		{
			var type = value != null ? GetType(value.GetType()) : VariableType.Null;
			var variable = Create(type);

			switch (value)
			{
				case bool bool_: variable.Boolean = bool_; break;
				case int int_: variable.Integer = int_; variable.Number = int_; break;
				case float float_: variable.Number = float_; variable.Integer = (int)float_; break;
				case string string_: variable.String = string_; break;
				case Object object_: variable._object = object_; break;
				case IVariableStore store_: variable._object = store_; break;
			}

			return variable;
		}

		#endregion

		#region Value Access

		public bool TryGetBoolean(out bool value)
		{
			value = Boolean;
			return Type == VariableType.Boolean;
		}

		public bool TryGetInteger(out int value)
		{
			value = Integer;
			return Type == VariableType.Integer;
		}

		public bool TryGetNumber(out float value)
		{
			value = Number;
			return Type == VariableType.Number;
		}

		public bool TryGetString(out string value)
		{
			value = String;
			return Type == VariableType.String;
		}

		public bool TryGetObject(out Object value)
		{
			value = Object;
			return Type == VariableType.Object || Type == VariableType.Store;
		}

		public bool TryGetObject<T>(out T obj) where T : Object
		{
			obj = _object as T;
			return obj != null;
		}

		public bool TryGetStore(out IVariableStore variables)
		{
			variables = _object as IVariableStore;
			return variables != null;
		}

		#endregion

		#region Comparison

		public static bool operator ==(VariableValue left, VariableValue right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(VariableValue left, VariableValue right)
		{
			return !left.Equals(right);
		}

		public static bool operator >(VariableValue left, VariableValue right)
		{
			return left.CompareTo(right) == 1;
		}

		public static bool operator <(VariableValue left, VariableValue right)
		{
			return left.CompareTo(right) == -1;
		}

		public static bool operator >=(VariableValue left, VariableValue right)
		{
			return left.CompareTo(right) >= 0;
		}

		public static bool operator <=(VariableValue left, VariableValue right)
		{
			return left.CompareTo(right) <= 0;
		}

		public bool Equals(VariableValue other)
		{
			switch (other.Type)
			{
				case VariableType.Empty: return Type == VariableType.Empty || Type == VariableType.Null;
				case VariableType.Boolean: return Equals(other.Boolean);
				case VariableType.Integer: return Equals(other.Integer);
				case VariableType.Number: return Equals(other.Number);
				case VariableType.String: return Equals(other.String);
				case VariableType.Object: return Equals(other.Object);
				case VariableType.Store: return Equals(other.Store);
				case VariableType.Null: return Type == VariableType.Empty || Type == VariableType.Null;
			}

			return false;
		}

		public bool Equals(bool value)
		{
			return Type == VariableType.Boolean && Boolean == value;
		}

		public bool Equals(int value)
		{
			return Type == VariableType.Integer && Integer == value;
		}

		public bool Equals(float value)
		{
			return Type == VariableType.Number && Number == value;
		}

		public bool Equals(string value)
		{
			return Type == VariableType.String && String == value;
		}

		public bool Equals(Object value)
		{
			return Type == VariableType.Object && (Object == value || ComponentHelper.GetAsBaseObject(Object) == ComponentHelper.GetAsBaseObject(value));
		}

		public bool Equals(IVariableStore value)
		{
			return Type == VariableType.Store && Store == value;
		}

		public override bool Equals(object other)
		{
			return (other is VariableValue) && Equals((VariableValue)other);
		}

		public int CompareTo(VariableValue other)
		{
			switch (other.Type)
			{
				case VariableType.Empty: return Type == VariableType.Empty || Type == VariableType.Null ? 0 : 1;
				case VariableType.Boolean: return CompareTo(other.Boolean);
				case VariableType.Integer: return CompareTo(other.Integer);
				case VariableType.Number: return CompareTo(other.Number);
				case VariableType.String: return CompareTo(other.String);
				case VariableType.Object: return CompareTo(other.Object);
				case VariableType.Store: return CompareTo(other.Store);
				case VariableType.Null: return Type == VariableType.Empty || Type == VariableType.Null ? 0 : 1;
			}

			return -1;
		}

		public int CompareTo(bool value)
		{
			switch (Type)
			{
				case VariableType.Empty: return -1;
				case VariableType.Null: return -1;
				case VariableType.Boolean: return Boolean == value ? 0 : (value ? 1 : -1);
				case VariableType.Integer: return Integer.CompareTo(value ? 1 : 0);
				case VariableType.Number: return Number.CompareTo(value ? 1.0f : 0.0f);
				default: return 1;
			}
		}

		public int CompareTo(int value)
		{
			switch (Type)
			{
				case VariableType.Empty: return -1;
				case VariableType.Null: return -1;
				case VariableType.Boolean: return (Boolean ? 1 : 0).CompareTo(value);
				case VariableType.Integer: return Integer.CompareTo(value);
				case VariableType.Number: return Number.CompareTo(value);
				default: return 1;
			}
		}

		public int CompareTo(float value)
		{
			switch (Type)
			{
				case VariableType.Empty: return -1;
				case VariableType.Null: return -1;
				case VariableType.Boolean: return (Boolean ? 1.0f : 0.0f).CompareTo(value);
				case VariableType.Integer: return ((float)Integer).CompareTo(value);
				case VariableType.Number: return Number.CompareTo(value);
				default: return 1;
			}
		}

		public int CompareTo(string value)
		{
			if (Type == VariableType.String)
				return String.CompareTo(value);

			return ((int)Type).CompareTo(VariableType.String);
		}

		public int CompareTo(Object unityObject)
		{
			if (Type == VariableType.Object)
				return Object == unityObject ? 0 : (Object == null ? -1 : 1);

			return ((int)Type).CompareTo(VariableType.Object);
		}

		public int CompareTo(IVariableStore store)
		{
			if (Type == VariableType.Store)
				return Store == store ? 0 : (Store == null ? -1 : 1);

			return ((int)Type).CompareTo(VariableType.Store);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hash = 13;
				hash = (hash * 397) ^ Type.GetHashCode();

				switch (Type)
				{
					case VariableType.Boolean: hash = (hash * 397) ^ Boolean.GetHashCode(); break;
					case VariableType.Integer: hash = (hash * 397) ^ Integer.GetHashCode(); break;
					case VariableType.Number: hash = (hash * 397) ^ Number.GetHashCode(); break;
					case VariableType.String: hash = (hash * 397) ^ String.GetHashCode(); break;
					case VariableType.Object: hash = (hash * 397) ^ ComponentHelper.GetAsBaseObject(Object).GetHashCode(); break;
					case VariableType.Store: hash = (hash * 397) ^ Store.GetHashCode(); break;
				}

				return hash;
			}
		}

		#endregion

		#region Persistence

		public string Write()
		{
			// Object types are persisted via SerializedVariable and any _object type that is not an Object cannot be
			// saved at all.

			switch (Type)
			{
				case VariableType.Boolean: return Boolean.ToString();
				case VariableType.Integer: return Integer.ToString();
				case VariableType.Number: return Number.ToString();
				case VariableType.String: return String;
				default: return "";
			}
		}

		public void Read(string value)
		{
			switch (Type)
			{
				case VariableType.Boolean: bool.TryParse(value, out bool boolean); Boolean = boolean; break;
				case VariableType.Integer: int.TryParse(value, out int integer); Integer = integer; Number = integer; break;
				case VariableType.Number: float.TryParse(value, out float number); Number = number; Integer = (int)number; break;
				case VariableType.String: String = value; break;
				default: break;
			}
		}

		#endregion
	}
}
