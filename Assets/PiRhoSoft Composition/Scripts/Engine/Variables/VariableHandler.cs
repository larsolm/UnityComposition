using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Object = UnityEngine.Object;

namespace PiRhoSoft.CompositionEngine
{
	public abstract class VariableHandler
	{
		private static VariableHandler[] _handlers;

		static VariableHandler()
		{
			_handlers = new VariableHandler[(int)(VariableType.List + 1)];
			_handlers[(int)VariableType.Empty] = new EmptyVariableHandler();
			_handlers[(int)VariableType.Bool] = new BoolVariableHandler();
			_handlers[(int)VariableType.Int] = new IntVariableHandler();
			_handlers[(int)VariableType.Float] = new FloatVariableHandler();
			_handlers[(int)VariableType.Int2] = new Int2VariableHandler();
			_handlers[(int)VariableType.Int3] = new Int3VariableHandler();
			_handlers[(int)VariableType.IntRect] = new IntRectVariableHandler();
			_handlers[(int)VariableType.IntBounds] = new IntBoundsVariableHandler();
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
		}

		private static VariableHandler Get(VariableType type)
		{
			return _handlers[(int)type];
		}

		#region General

		public static string ToString(VariableValue value)
		{
			var builder = new StringBuilder();
			ToString(value, builder);
			return builder.ToString();
		}

		public static VariableValue CreateDefault(VariableType type, VariableConstraint constraint) => Get(type).CreateDefault_(constraint);
		public static void ToString(VariableValue value, StringBuilder builder) => Get(value.Type).ToString_(value, builder);

		protected abstract VariableValue CreateDefault_(VariableConstraint constraint);
		protected abstract void ToString_(VariableValue value, StringBuilder builder);

		#endregion

		#region Serialization

		public static void Save(VariableValue value, ref string data, ref List<Object> objects)
		{
			objects = new List<Object>();

			using (var stream = new MemoryStream())
			{
				using (var writer = new BinaryWriter(stream))
					Write(value, writer, objects);

				data = Convert.ToBase64String(stream.ToArray());
			}
		}

		public static VariableValue Load(ref string data, ref List<Object> objects)
		{
			try
			{
				var bytes = Convert.FromBase64String(data);

				using (var stream = new MemoryStream(bytes))
				{
					using (var reader = new BinaryReader(stream))
						return Read(reader, objects);
				}
			}
			catch
			{
			}

			data = null;
			objects = null;

			return VariableValue.Empty;
		}

		public static void Write(VariableValue value, BinaryWriter writer, List<Object> objects)
		{
			writer.Write((int)value.Type);
			Get(value.Type).Write_(value, writer, objects);
		}

		public static VariableValue Read(BinaryReader reader, List<Object> objects)
		{
			var type = (VariableType)reader.ReadInt32();
			return Get(type).Read_(reader, objects);
		}

		protected abstract void Write_(VariableValue value, BinaryWriter writer, List<Object> objects);
		protected abstract VariableValue Read_(BinaryReader reader, List<Object> objects);

		#endregion

		#region Constraints

		public static VariableConstraint CreateConstraint(VariableType type, string data, IList<Object> objects)
		{
			if (!string.IsNullOrEmpty(data))
			{
				var handler = Get(type);
				var constraint = handler.CreateConstraint();

				if (constraint != null)
				{
					if (constraint.Read(data, objects))
						return constraint;
				}
			}

			return null;
		}

		protected virtual VariableConstraint CreateConstraint() => null;

		#endregion

		#region Math

		public static VariableValue Add(VariableValue left, VariableValue right) => Get(left.Type).Add_(left, right);
		public static VariableValue Subtract(VariableValue left, VariableValue right) => Get(left.Type).Subtract_(left, right);
		public static VariableValue Multiply(VariableValue left, VariableValue right) => Get(left.Type).Multiply_(left, right);
		public static VariableValue Divide(VariableValue left, VariableValue right) => Get(left.Type).Divide_(left, right);
		public static VariableValue Modulo(VariableValue left, VariableValue right) => Get(left.Type).Modulo_(left, right);
		public static VariableValue Exponent(VariableValue left, VariableValue right) => Get(left.Type).Exponent_(left, right);
		public static VariableValue Negate(VariableValue value) => Get(value.Type).Negate_(value);

		protected virtual VariableValue Add_(VariableValue left, VariableValue right) => VariableValue.Empty;
		protected virtual VariableValue Subtract_(VariableValue left, VariableValue right) => VariableValue.Empty;
		protected virtual VariableValue Multiply_(VariableValue left, VariableValue right) => VariableValue.Empty;
		protected virtual VariableValue Divide_(VariableValue left, VariableValue right) => VariableValue.Empty;
		protected virtual VariableValue Modulo_(VariableValue left, VariableValue right) => VariableValue.Empty;
		protected virtual VariableValue Exponent_(VariableValue left, VariableValue right) => VariableValue.Empty;
		protected virtual VariableValue Negate_(VariableValue value) => VariableValue.Empty;

		#endregion

		#region Logic

		public static VariableValue And(VariableValue left, VariableValue right) => Get(left.Type).And_(left, right);
		public static VariableValue Or(VariableValue left, VariableValue right) => Get(left.Type).Or_(left, right);
		public static VariableValue Not(VariableValue value) => Get(value.Type).Not_(value);

		protected virtual VariableValue And_(VariableValue left, VariableValue right) => VariableValue.Empty;
		protected virtual VariableValue Or_(VariableValue left, VariableValue right) => VariableValue.Empty;
		protected virtual VariableValue Not_(VariableValue value) => VariableValue.Empty;

		#endregion

		#region Comparison

		// Valid comparisons follow the same casting rules as laid out in the Casting region of the VariableValue
		// definition with the addition that VariableType Empty compares equal to null objects. Comparison results
		// follow the same rules as the .net CompareTo method.

		public static bool? IsEqual(VariableValue left, VariableValue right) => Get(left.Type).IsEqual_(left, right);
		public static int? Compare(VariableValue left, VariableValue right) => Get(left.Type).Compare_(left, right);

		protected virtual bool? IsEqual_(VariableValue left, VariableValue right) => null;
		protected virtual int? Compare_(VariableValue left, VariableValue right) => null;

		#endregion

		#region Lookup

		public static VariableValue Lookup(VariableValue owner, VariableValue lookup) => Get(owner.Type).Lookup_(owner, lookup);
		public static SetVariableResult Apply(ref VariableValue owner, VariableValue lookup, VariableValue value) => Get(owner.Type).Apply_(ref owner, lookup, value);
		public static VariableValue Cast(VariableValue owner, string type) => Get(owner.Type).Cast_(owner, type);
		public static bool Test(VariableValue owner, VariableValue test) => Get(owner.Type).Test_(owner, test);

		protected virtual VariableValue Lookup_(VariableValue owner, VariableValue lookup) => VariableValue.Empty;
		protected virtual SetVariableResult Apply_(ref VariableValue owner, VariableValue lookup, VariableValue value) => SetVariableResult.NotFound;
		protected virtual VariableValue Cast_(VariableValue owner, string type) => VariableValue.Empty;
		protected virtual bool Test_(VariableValue owner, VariableValue test) => false;

		#endregion
	}
}
