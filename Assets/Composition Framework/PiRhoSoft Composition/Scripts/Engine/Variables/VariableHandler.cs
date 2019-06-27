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
		private static short _dataVersion = 1;

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

		protected internal abstract VariableValue CreateDefault_(VariableConstraint constraint);
		protected internal abstract void ToString_(VariableValue value, StringBuilder builder);

		#endregion

		#region Serialization

		protected internal abstract void Write_(VariableValue value, BinaryWriter writer, List<Object> objects);
		protected internal abstract VariableValue Read_(BinaryReader reader, List<Object> objects, short version);

		// error reporting

		private static short ReadVersion(BinaryReader reader)
		{
			var version = reader.ReadInt16();

			// temporary enforcement that version is 1
			if (version != 1)
			{
				version = 0;
				reader.BaseStream.Seek(0, SeekOrigin.Begin);
			}

			return version;
		}

		private static void WriteVersion(BinaryWriter writer)
		{
			writer.Write(_dataVersion);
		}

		private static byte[] GetBytes(string data)
		{
			try { return Convert.FromBase64String(data); }
			catch (FormatException) { return null; }
		}

		private static string GetString(byte[] bytes)
		{
			return Convert.ToBase64String(bytes);
		}

		#region Variables

		public static List<string> SaveVariables(IList<Variable> variables, ref List<Object> objects)
		{
			var data = new List<string>();
			objects = new List<Object>();

			foreach (var variable in variables)
			{
				var variableData = WriteVariable(variable, objects);
				data.Add(variableData);
			}

			return data;
		}

		public static List<Variable> LoadVariables(ref List<string> data, ref List<Object> objects)
		{
			var variables = new List<Variable>();

			foreach (var variableData in data)
			{
				var variable = ReadVariable(variableData, objects);
				variables.Add(variable);
			}

			data = null;
			objects = null;

			return variables;
		}

		public static string SaveVariable(Variable variable, ref List<Object> objects)
		{
			objects = new List<Object>();
			return WriteVariable(variable, objects);
		}

		public static Variable LoadVariable(ref string data, ref List<Object> objects)
		{
			var variable = ReadVariable(data, objects);

			data = null;
			objects = null;

			return variable;
		}

		private static string WriteVariable(Variable variable, List<Object> objects)
		{
			using (var stream = new MemoryStream())
			{
				using (var writer = new BinaryWriter(stream))
				{
					WriteVersion(writer);
					WriteVariable(variable, writer, objects);
				}

				return GetString(stream.ToArray());
			}
		}

		private static Variable ReadVariable(string data, List<Object> objects)
		{
			var bytes = GetBytes(data);

			if (bytes == null)
				return Variable.Empty;

			using (var stream = new MemoryStream(bytes))
			{
				using (var reader = new BinaryReader(stream))
				{
					var version = ReadVersion(reader);
					return ReadVariable(reader, objects, version);
				}
			}
		}

		private static void WriteVariable(Variable variable, BinaryWriter writer, List<Object> objects)
		{
			writer.Write(variable.Name);
			WriteValue(variable.Value, writer, objects);
		}

		private static Variable ReadVariable(BinaryReader reader, List<Object> objects, short version)
		{
			var name = reader.ReadString();
			var value = ReadValue(reader, objects, version);

			return Variable.Create(name, value);
		}

		#endregion

		#region Values

		public static string SaveValue(VariableValue value, ref List<Object> objects)
		{
			objects = new List<Object>();
			return WriteValue(value, objects);
		}

		public static VariableValue LoadValue(ref string data, ref List<Object> objects)
		{
			var value = ReadValue(data, objects);

			data = null;
			objects = null;

			return value;
		}

		private static string WriteValue(VariableValue value, List<Object> objects)
		{
			using (var stream = new MemoryStream())
			{
				using (var writer = new BinaryWriter(stream))
				{
					WriteVersion(writer);
					WriteValue(value, writer, objects);
				}

				return GetString(stream.ToArray());
			}
		}

		private static VariableValue ReadValue(string data, List<Object> objects)
		{
			var bytes = GetBytes(data);

			if (bytes == null)
				return VariableValue.Empty;

			using (var stream = new MemoryStream(bytes))
			{
				using (var reader = new BinaryReader(stream))
				{
					var version = ReadVersion(reader);
					return ReadValue(reader, objects, version);
				}
			}
		}

		protected static void WriteValue(VariableValue value, BinaryWriter writer, List<Object> objects)
		{
			writer.Write((int)value.Type);
			Get(value.Type).Write_(value, writer, objects);
		}

		protected static VariableValue ReadValue(BinaryReader reader, List<Object> objects, short version)
		{
			var type = (VariableType)reader.ReadInt32();
			return Get(type).Read_(reader, objects, version);
		}

		#endregion

		#region Constraints

		public static string SaveConstraint(VariableType type, VariableConstraint constraint, out List<Object> objects)
		{
			if (constraint == null)
			{
				objects = null;
				return string.Empty;
			}

			objects = new List<Object>();
			return WriteConstraint(type, constraint, objects);
		}

		public static VariableConstraint LoadConstraint(ref string data, ref List<Object> objects)
		{
			var constraint = ReadConstraint(data, objects);

			data = null;
			objects = null;

			return constraint;
		}

		private static string WriteConstraint(VariableType type, VariableConstraint constraint, List<Object> objects)
		{
			using (var stream = new MemoryStream())
			{
				using (var writer = new BinaryWriter(stream))
				{
					WriteVersion(writer);
					WriteConstraint(type, constraint, writer, objects);
				}

				return GetString(stream.ToArray());
			}
		}

		private static VariableConstraint ReadConstraint(string data, List<Object> objects)
		{
			var bytes = string.IsNullOrEmpty(data) ? null : GetBytes(data);

			if (bytes == null)
				return null;

			using (var stream = new MemoryStream(bytes))
			{
				using (var reader = new BinaryReader(stream))
				{
					var version = ReadVersion(reader);
					return ReadConstraint(reader, objects, version);
				}
			}
		}

		internal static void WriteConstraint(VariableType type, VariableConstraint constraint, BinaryWriter writer, IList<Object> objects)
		{
			writer.Write((int)type);
			constraint.Write(writer, objects);
		}

		internal static VariableConstraint ReadConstraint(BinaryReader reader, IList<Object> objects, short version)
		{
			var type = (VariableType)reader.ReadInt32();
			var constraint = VariableConstraint.Create(type);
			constraint?.Read(reader, objects, version);
			return constraint;
		}

		#endregion

		#endregion

		#region Math

		public static VariableValue Add(VariableValue left, VariableValue right) => Get(left.Type).Add_(left, right);
		public static VariableValue Subtract(VariableValue left, VariableValue right) => Get(left.Type).Subtract_(left, right);
		public static VariableValue Multiply(VariableValue left, VariableValue right) => Get(left.Type).Multiply_(left, right);
		public static VariableValue Divide(VariableValue left, VariableValue right) => Get(left.Type).Divide_(left, right);
		public static VariableValue Modulo(VariableValue left, VariableValue right) => Get(left.Type).Modulo_(left, right);
		public static VariableValue Exponent(VariableValue left, VariableValue right) => Get(left.Type).Exponent_(left, right);
		public static VariableValue Negate(VariableValue value) => Get(value.Type).Negate_(value);

		protected internal virtual VariableValue Add_(VariableValue left, VariableValue right) => VariableValue.Empty;
		protected internal virtual VariableValue Subtract_(VariableValue left, VariableValue right) => VariableValue.Empty;
		protected internal virtual VariableValue Multiply_(VariableValue left, VariableValue right) => VariableValue.Empty;
		protected internal virtual VariableValue Divide_(VariableValue left, VariableValue right) => VariableValue.Empty;
		protected internal virtual VariableValue Modulo_(VariableValue left, VariableValue right) => VariableValue.Empty;
		protected internal virtual VariableValue Exponent_(VariableValue left, VariableValue right) => VariableValue.Empty;
		protected internal virtual VariableValue Negate_(VariableValue value) => VariableValue.Empty;

		#endregion

		#region Comparison

		// Valid comparisons follow the same casting rules as laid out in the Casting region of the VariableValue
		// definition with the addition that VariableType Empty compares equal to null objects. Comparison results
		// follow the same rules as the .net CompareTo method.

		public static bool? IsEqual(VariableValue left, VariableValue right) => Get(left.Type).IsEqual_(left, right);
		public static int? Compare(VariableValue left, VariableValue right) => Get(left.Type).Compare_(left, right);

		protected internal virtual bool? IsEqual_(VariableValue left, VariableValue right) => null;
		protected internal virtual int? Compare_(VariableValue left, VariableValue right) => null;

		#endregion

		#region Lookup

		public static VariableValue Lookup(VariableValue owner, VariableValue lookup) => Get(owner.Type).Lookup_(owner, lookup);
		public static SetVariableResult Apply(ref VariableValue owner, VariableValue lookup, VariableValue value) => Get(owner.Type).Apply_(ref owner, lookup, value);
		public static VariableValue Cast(VariableValue owner, string type) => Get(owner.Type).Cast_(owner, type);
		public static bool Test(VariableValue owner, string type) => Get(owner.Type).Test_(owner, type);

		protected internal virtual VariableValue Lookup_(VariableValue owner, VariableValue lookup) => VariableValue.Empty;
		protected internal virtual SetVariableResult Apply_(ref VariableValue owner, VariableValue lookup, VariableValue value) => SetVariableResult.NotFound;
		protected internal virtual VariableValue Cast_(VariableValue owner, string type) => VariableValue.Empty;
		protected internal virtual bool Test_(VariableValue owner, string type) => false;

		#endregion
	}
}
