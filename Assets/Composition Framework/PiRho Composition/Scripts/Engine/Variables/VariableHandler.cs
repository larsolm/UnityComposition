using PiRhoSoft.Utilities;
using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace PiRhoSoft.Composition
{
	public abstract class VariableHandler
	{
		public const string ListCountName = "Count";

		private static VariableHandler[] _handlers;

		static VariableHandler()
		{
			_handlers = new VariableHandler[(int)(VariableType.Other + 1)];
			_handlers[(int)VariableType.Empty] = new EmptyVariableHandler();
			_handlers[(int)VariableType.Bool] = new BoolVariableHandler();
			_handlers[(int)VariableType.Int] = new IntVariableHandler();
			_handlers[(int)VariableType.Float] = new FloatVariableHandler();
			_handlers[(int)VariableType.Vector2Int] = new Vector2IntVariableHandler();
			_handlers[(int)VariableType.Vector3Int] = new Vector3IntVariableHandler();
			_handlers[(int)VariableType.RectInt] = new RectIntVariableHandler();
			_handlers[(int)VariableType.BoundsInt] = new BoundsIntVariableHandler();
			_handlers[(int)VariableType.Vector2] = new Vector2VariableHandler();
			_handlers[(int)VariableType.Vector3] = new Vector3VariableHandler();
			_handlers[(int)VariableType.Vector4] = new Vector4VariableHandler();
			_handlers[(int)VariableType.Quaternion] = new QuaternionVariableHandler();
			_handlers[(int)VariableType.Rect] = new RectVariableHandler();
			_handlers[(int)VariableType.Bounds] = new BoundsVariableHandler();
			_handlers[(int)VariableType.Color] = new ColorVariableHandler();
			_handlers[(int)VariableType.String] = new StringVariableHandler();
			_handlers[(int)VariableType.Enum] = new EnumVariableHandler();
			_handlers[(int)VariableType.Object] = new ObjectVariableHandler();
			_handlers[(int)VariableType.Store] = new StoreVariableHandler();
			_handlers[(int)VariableType.List] = new ListVariableHandler();
			_handlers[(int)VariableType.Other] = new OtherVariableHandler();
		}

		private static VariableHandler Get(VariableType type)
		{
			return _handlers[(int)type];
		}

		#region General

		public static string ToString(Variable value)
		{
			var builder = new StringBuilder();
			ToString(value, builder);
			return builder.ToString();
		}

		public static void ToString(Variable value, StringBuilder builder) => Get(value.Type).ToString_(value, builder);
		protected internal abstract void ToString_(Variable value, StringBuilder builder);

		#endregion

		#region Serialization

		public static void Save(Variable variable, BinaryWriter writer, SerializedData data)
		{
			writer.Write((int)variable.Type);
			Get(variable.Type).Save_(variable, writer, data);
		}

		public static Variable Load(BinaryReader reader, SerializedData data)
		{
			var type = (VariableType)reader.ReadInt32();
			return Get(type).Load_(reader, data);
		}

		protected internal abstract void Save_(Variable value, BinaryWriter writer, SerializedData data);
		protected internal abstract Variable Load_(BinaryReader reader, SerializedData data);

		#endregion

		#region Math

		public static Variable Add(Variable left, Variable right) => Get(left.Type).Add_(left, right);
		public static Variable Subtract(Variable left, Variable right) => Get(left.Type).Subtract_(left, right);
		public static Variable Multiply(Variable left, Variable right) => Get(left.Type).Multiply_(left, right);
		public static Variable Divide(Variable left, Variable right) => Get(left.Type).Divide_(left, right);
		public static Variable Modulo(Variable left, Variable right) => Get(left.Type).Modulo_(left, right);
		public static Variable Exponent(Variable left, Variable right) => Get(left.Type).Exponent_(left, right);
		public static Variable Negate(Variable value) => Get(value.Type).Negate_(value);

		protected internal virtual Variable Add_(Variable left, Variable right) => Variable.Empty;
		protected internal virtual Variable Subtract_(Variable left, Variable right) => Variable.Empty;
		protected internal virtual Variable Multiply_(Variable left, Variable right) => Variable.Empty;
		protected internal virtual Variable Divide_(Variable left, Variable right) => Variable.Empty;
		protected internal virtual Variable Modulo_(Variable left, Variable right) => Variable.Empty;
		protected internal virtual Variable Exponent_(Variable left, Variable right) => Variable.Empty;
		protected internal virtual Variable Negate_(Variable value) => Variable.Empty;

		#endregion

		#region Comparison

		// Valid comparisons follow the same casting rules as laid out in the Casting region of the VariableValue
		// definition with the addition that VariableType Empty compares equal to null objects. Comparison results
		// follow the same rules as the .net CompareTo method.

		public static bool? IsEqual(Variable left, Variable right) => Get(left.Type).IsEqual_(left, right);
		public static int? Compare(Variable left, Variable right) => Get(left.Type).Compare_(left, right);

		protected internal virtual bool? IsEqual_(Variable left, Variable right) => null;
		protected internal virtual int? Compare_(Variable left, Variable right) => null;

		#endregion

		#region Lookup

		public static Variable Lookup(Variable owner, Variable lookup) => Get(owner.Type).Lookup_(owner, lookup);
		public static SetVariableResult Apply(ref Variable owner, Variable lookup, Variable value) => Get(owner.Type).Apply_(ref owner, lookup, value);
		public static Variable Cast(Variable owner, string type) => Get(owner.Type).Cast_(owner, type);
		public static bool Test(Variable owner, string type) => Get(owner.Type).Test_(owner, type);

		protected internal virtual Variable Lookup_(Variable owner, Variable lookup) => Variable.Empty;
		protected internal virtual SetVariableResult Apply_(ref Variable owner, Variable lookup, Variable value) => SetVariableResult.NotFound;
		protected internal virtual Variable Cast_(Variable owner, string type) => Variable.Empty;
		protected internal virtual bool Test_(Variable owner, string type) => false;

		protected static Variable Lookup(object obj, Variable lookup)
		{
			if (obj is IVariableStore store)
				return LookupInStore(store, lookup);

			if (obj is IVariableList list)
				return LookupInList(list, lookup);

			if (ClassMap.Get(obj.GetType(), out var map))
				return LookupWithMap(obj, map, lookup);

			return LookupWithReflection(obj, lookup);
		}

		protected static SetVariableResult Apply(object obj, Variable lookup, Variable value)
		{
			if (obj is IVariableStore store)
				return ApplyToStore(store, lookup, value);

			if (obj is IVariableList list)
				return ApplyToList(list, lookup, value);

			if (ClassMap.Get(obj.GetType(), out var map))
				return ApplyWithMap(obj, map, lookup, value);

			return ApplyWithReflection(obj, lookup, value);
		}

		private static Variable LookupInStore(IVariableStore store, Variable lookup)
		{
			if (lookup.TryGetString(out var s))
				return store.GetVariable(s);

			return Variable.Empty;
		}

		private static SetVariableResult ApplyToStore(IVariableStore store, Variable lookup, Variable value)
		{
			if (lookup.TryGetString(out var s))
				return store.SetVariable(s, value);
			else
				return SetVariableResult.TypeMismatch;
		}

		private static Variable LookupInList(IVariableList list, Variable lookup)
		{
			if (lookup.TryGetString(out var s))
			{
				if (s == ListCountName)
					return Variable.Int(list.Count);
			}
			else if (lookup.TryGetInt(out var i))
			{
				if (i >= 0 && i < list.Count)
					return list.GetVariable(i);
			}

			return Variable.Empty;
		}

		private static SetVariableResult ApplyToList(IVariableList list, Variable lookup, Variable value)
		{
			if (lookup.TryGetString(out var s))
			{
				if (s == ListCountName)
					return SetVariableResult.ReadOnly;
				else
					return SetVariableResult.NotFound;
			}
			else if (lookup.TryGetInt(out var i))
			{
				if (i >= 0 && i < list.Count)
					return list.SetVariable(i, value);
				else
					return SetVariableResult.NotFound;
			}
			else
			{
				return SetVariableResult.TypeMismatch;
			}
		}

		private static Variable LookupWithMap(object obj, IClassMap map, Variable lookup)
		{
			if (lookup.TryGetString(out var s))
				return map.GetVariable(obj, s);

			return Variable.Empty;
		}

		private static SetVariableResult ApplyWithMap(object obj, IClassMap map, Variable lookup, Variable value)
		{
			if (lookup.TryGetString(out var s))
				return map.SetVariable(obj, s, value);

			return SetVariableResult.NotFound;
		}

		private static Variable LookupWithReflection(object obj, Variable lookup)
		{
			if (lookup.TryGetString(out var s))
			{
				var field = obj.GetType().GetField(s, BindingFlags.Instance | BindingFlags.Public);
				if (field != null)
				{
					try { return Variable.Unbox(field.GetValue(obj)); }
					catch { return Variable.Empty; }
				}

				var property = obj.GetType().GetProperty(s, BindingFlags.Instance | BindingFlags.Public);
				if (property != null)
				{
					try { return Variable.Unbox(property.GetValue(obj)); }
					catch { return Variable.Empty; }
				}
			}

			return Variable.Empty;
		}

		private static SetVariableResult ApplyWithReflection(object obj, Variable lookup, Variable value)
		{
			if (lookup.TryGetString(out var s))
			{
				var field = obj.GetType().GetField(s, BindingFlags.Instance | BindingFlags.Public);
				if (field != null)
				{
					try
					{
						field.SetValue(obj, value.Box());
						return SetVariableResult.Success;
					}
					catch (FieldAccessException)
					{
						return SetVariableResult.ReadOnly;
					}
					catch (ArgumentException)
					{
						return SetVariableResult.TypeMismatch;
					}
				}

				var property = obj.GetType().GetProperty(s, BindingFlags.Instance | BindingFlags.Public);
				if (property != null)
				{
					try
					{
						property.SetValue(obj, value.Box());
						return SetVariableResult.Success;
					}
					catch (FieldAccessException)
					{
						return SetVariableResult.ReadOnly;
					}
					catch (ArgumentException)
					{
						return SetVariableResult.TypeMismatch;
					}
				}
			}

			return SetVariableResult.NotFound;
		}

		#endregion
	}
}