using System.Collections.Generic;
using System.IO;
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

		public static VariableHandler Get(VariableType type)
		{
			return _handlers[(int)type];
		}

		public VariableConstraint CreateConstraint(string data)
		{
			if (!string.IsNullOrEmpty(data))
			{
				var constraint = CreateConstraint();

				if (constraint != null)
				{
					if (constraint.Read(data))
						return constraint;
				}
			}

			return null;
		}

		protected virtual VariableConstraint CreateConstraint() => null;

		public virtual bool IsAssignable(VariableValue from, VariableValue to) => from.Type == to.Type;
		public virtual bool? IsEqual(VariableValue left, VariableValue right) => null;
		public virtual int? Compare(VariableValue left, VariableValue right) => null;

		public abstract VariableValue CreateDefault(VariableConstraint constraint);
		public abstract void Write(VariableValue value, BinaryWriter writer, List<Object> objects);
		public abstract void Read(ref VariableValue value, BinaryReader reader, List<Object> objects);
		public abstract VariableValue Lookup(VariableValue owner, string lookup);
		public abstract SetVariableResult Apply(ref VariableValue owner, string lookup, VariableValue value);
	}
}
