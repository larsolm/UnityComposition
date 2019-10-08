using PiRhoSoft.Utilities;
using System.IO;

namespace PiRhoSoft.Composition
{
	public enum SetVariableResult
	{
		Success,
		NotFound,
		ReadOnly,
		TypeMismatch
	}

	public abstract class VariableHandler
	{
		private static VariableHandler[] _handlers;

		static VariableHandler()
		{
			_handlers = new VariableHandler[(int)(VariableType.Object + 1)];
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
			_handlers[(int)VariableType.Enum] = new EnumVariableHandler();
			_handlers[(int)VariableType.String] = new StringVariableHandler();
			_handlers[(int)VariableType.List] = new ListVariableHandler();
			_handlers[(int)VariableType.Dictionary] = new DictionaryVariableHandler();
			_handlers[(int)VariableType.Asset] = new AssetVariableHandler();
			_handlers[(int)VariableType.Object] = new ObjectVariableHandler();
		}

		private static VariableHandler Get(VariableType type)
		{
			return _handlers[(int)type];
		}

		#region General

		public static string ToString(Variable value)
		{
			return Get(value.Type).ToString_(value);
		}

		protected internal abstract string ToString_(Variable value);

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

		public static bool? IsEqual(Variable left, Variable right) => Get(left.Type).IsEqual_(left, right);
		public static int? Compare(Variable left, Variable right) => Get(left.Type).Compare_(left, right);

		protected internal virtual bool? IsEqual_(Variable left, Variable right) => null;
		protected internal virtual int? Compare_(Variable left, Variable right) => null;

		#endregion

		#region Animation

		public static float Distance(Variable from, Variable to) => Get(from.Type).Distance_(from, to);
		public static Variable Interpolate(Variable from, Variable to, float time) => Get(from.Type).Interpolate_(from, to, time);

		protected internal virtual float Distance_(Variable from, Variable to) => 0.0f;
		protected internal virtual Variable Interpolate_(Variable from, Variable to, float time) => Variable.Empty;

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

		#endregion
	}
}