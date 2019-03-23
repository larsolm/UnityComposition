using System;
using System.Collections.Generic;
using System.IO;
using Object = UnityEngine.Object;

namespace PiRhoSoft.CompositionEngine
{
	public abstract class VariableHandler
	{
		private static VariableHandler[] _handlers;
		private static Dictionary<Type, VariableHandler> _types;

		static VariableHandler()
		{
			_handlers = new VariableHandler[(int)(VariableType.List + 1)];
			_handlers[(int)VariableType.Empty] = new EmptyVariableResolver();
			_handlers[(int)VariableType.Bool] = new BoolVariableResolver();
			_handlers[(int)VariableType.Int] = new IntVariableResolver();
			_handlers[(int)VariableType.Float] = new FloatVariableResolver();
			_handlers[(int)VariableType.Int2] = new Int2VariableResolver();
			_handlers[(int)VariableType.Int3] = new Int3VariableResolver();
			_handlers[(int)VariableType.IntRect] = new IntRectVariableResolver();
			_handlers[(int)VariableType.IntBounds] = new IntBoundsVariableResolver();
			_handlers[(int)VariableType.Vector2] = new Vector2VariableResolver();
			_handlers[(int)VariableType.Vector3] = new Vector3VariableResolver();
			_handlers[(int)VariableType.Vector4] = new Vector4VariableResolver();
			_handlers[(int)VariableType.Quaternion] = new QuaternionVariableResolver();
			_handlers[(int)VariableType.Rect] = new RectVariableResolver();
			_handlers[(int)VariableType.Bounds] = new BoundsVariableResolver();
			_handlers[(int)VariableType.Color] = new ColorVariableResolver();
			_handlers[(int)VariableType.String] = new StringVariableResolver();
			_handlers[(int)VariableType.Enum] = new EnumVariableResolver();
			_handlers[(int)VariableType.Object] = new ObjectVariableResolver();
			_handlers[(int)VariableType.Store] = new StoreVariableResolver();
			_handlers[(int)VariableType.List] = new ListVariableResolver();

			_types = new Dictionary<Type, VariableHandler>();
			_types.Add(typeof(bool), _handlers[(int)VariableType.Bool]);
		}

		public static VariableHandler Get(VariableType type)
		{
			return _handlers[(int)type];
		}

		public static VariableHandler Get(Type type)
		{
		}

		public abstract void Write(BinaryWriter writer, List<Object> objects);
		public abstract void Read(BinaryReader reader, List<Object> objects);
		public abstract VariableValue Lookup(VariableValue owner, string lookup);
		public abstract SetVariableResult Apply(ref VariableValue owner, string lookup, VariableValue value);
	}
}
