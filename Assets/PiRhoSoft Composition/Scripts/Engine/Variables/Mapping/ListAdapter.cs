using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	public abstract class ListAdapter : IVariableList
	{
		private bool _allowSet;
		private bool _allowAdd;
		private bool _allowRemove;

		public static IVariableList Create(IList list)
		{
			ListAdapter adapter = null;
			var itemType = list.GetType().GetGenericArguments()[0];

			if (itemType == typeof(bool)) adapter = new BoolListAdapter();
			else if (itemType == typeof(int)) adapter = new IntListAdapter();
			else if (itemType == typeof(float)) adapter = new FloatListAdapter();
			else if (itemType == typeof(Vector2Int)) adapter = new Int2ListAdapter();
			else if (itemType == typeof(Vector3Int)) adapter = new Int3ListAdapter();
			else if (itemType == typeof(RectInt)) adapter = new IntRectListAdapter();
			else if (itemType == typeof(BoundsInt)) adapter = new IntBoundsListAdapter();
			else if (itemType == typeof(Vector2)) adapter = new Vector2ListAdapter();
			else if (itemType == typeof(Vector3)) adapter = new Vector3ListAdapter();
			else if (itemType == typeof(Vector4)) adapter = new Vector4ListAdapter();
			else if (itemType == typeof(Quaternion)) adapter = new QuaternionListAdapter();
			else if (itemType == typeof(RectInt)) adapter = new RectListAdapter();
			else if (itemType == typeof(BoundsInt)) adapter = new BoundsListAdapter();
			else if (itemType == typeof(Color)) adapter = new ColorListAdapter();
			else if (itemType == typeof(string)) adapter = new StringListAdapter();
			else if (itemType.IsEnum) adapter = new EnumListAdapter();
			else adapter = new ObjectListAdapter();

			adapter.Setup(list, itemType, true, true, true);

			return adapter;
		}

		public abstract int Count { get; }

		protected abstract void Setup(IList list, Type itemType, bool allowSet, bool allowAdd, bool allowRemove);

		protected void Setup(bool allowSet, bool allowAdd, bool allowRemove)
		{
			_allowSet = allowSet;
			_allowAdd = allowAdd;
			_allowRemove = allowRemove;
		}

		public VariableValue GetVariable(int index)
		{
			if (index >= 0 && index <= Count)
				return Get(index);
			else
				return VariableValue.Empty;
		}

		public SetVariableResult SetVariable(int index, VariableValue value)
		{
			if (!_allowSet)
			{
				return SetVariableResult.ReadOnly;
			}
			else if (index >= 0 && index <= Count)
			{
				if (Set(index, value))
					return SetVariableResult.Success;
				else
					return SetVariableResult.TypeMismatch;
			}

			return SetVariableResult.NotFound;
		}

		public SetVariableResult AddVariable(VariableValue value)
		{
			if (!_allowAdd)
				return SetVariableResult.ReadOnly;
			else if (Add(value))
				return SetVariableResult.Success;

			return SetVariableResult.TypeMismatch;
		}

		public SetVariableResult RemoveVariable(int index)
		{
			if (!_allowRemove)
			{
				return SetVariableResult.ReadOnly;
			}
			else if (index >= 0 && index <= Count)
			{
				Remove(index);
				return SetVariableResult.Success;
			}

			return SetVariableResult.NotFound;
		}

		protected abstract VariableValue Get(int index);
		protected abstract bool Set(int index, VariableValue value);
		protected abstract bool Add(VariableValue value);
		protected abstract void Remove(int index);
	}

	#region Value Adapters

	internal abstract class ValueListAdapter<T> : ListAdapter
	{
		public override int Count => _list.Count;

		protected IList<T> _list;

		protected override void Setup(IList list, Type itemType, bool allowSet, bool allowAdd, bool allowRemove)
		{
			_list = list as IList<T>;
			Setup(allowSet, allowAdd, allowRemove);
		}

		protected bool Set(int index, T value)
		{
			_list[index] = value;
			return true;
		}

		protected bool Add(T value)
		{
			_list.Add(value);
			return true;
		}

		protected override void Remove(int index)
		{
			_list.RemoveAt(index);
		}
	}

	internal class BoolListAdapter : ValueListAdapter<bool>
	{
		protected override VariableValue Get(int index) => VariableValue.Create(_list[index]);
		protected override bool Set(int index, VariableValue value) => value.TryGetBool(out var v) && Set(index, v);
		protected override bool Add(VariableValue value) => value.TryGetBool(out var v) && Add(v);
	}

	internal class IntListAdapter : ValueListAdapter<int>
	{
		protected override VariableValue Get(int index) => VariableValue.Create(_list[index]);
		protected override bool Set(int index, VariableValue value) => value.TryGetInt(out var v) && Set(index, v);
		protected override bool Add(VariableValue value) => value.TryGetInt(out var v) && Add(v);
	}

	internal class FloatListAdapter : ValueListAdapter<float>
	{
		protected override VariableValue Get(int index) => VariableValue.Create(_list[index]);
		protected override bool Set(int index, VariableValue value) => value.TryGetFloat(out var v) && Set(index, v);
		protected override bool Add(VariableValue value) => value.TryGetFloat(out var v) && Add(v);
	}

	internal class Int2ListAdapter : ValueListAdapter<Vector2Int>
	{
		protected override VariableValue Get(int index) => VariableValue.Create(_list[index]);
		protected override bool Set(int index, VariableValue value) => value.TryGetInt2(out var v) && Set(index, v);
		protected override bool Add(VariableValue value) => value.TryGetInt2(out var v) && Add(v);
	}

	internal class Int3ListAdapter : ValueListAdapter<Vector3Int>
	{
		protected override VariableValue Get(int index) => VariableValue.Create(_list[index]);
		protected override bool Set(int index, VariableValue value) => value.TryGetInt3(out var v) && Set(index, v);
		protected override bool Add(VariableValue value) => value.TryGetInt3(out var v) && Add(v);
	}

	internal class IntRectListAdapter : ValueListAdapter<RectInt>
	{
		protected override VariableValue Get(int index) => VariableValue.Create(_list[index]);
		protected override bool Set(int index, VariableValue value) => value.TryGetIntRect(out var v) && Set(index, v);
		protected override bool Add(VariableValue value) => value.TryGetIntRect(out var v) && Add(v);
	}

	internal class IntBoundsListAdapter : ValueListAdapter<BoundsInt>
	{
		protected override VariableValue Get(int index) => VariableValue.Create(_list[index]);
		protected override bool Set(int index, VariableValue value) => value.TryGetIntBounds(out var v) && Set(index, v);
		protected override bool Add(VariableValue value) => value.TryGetIntBounds(out var v) && Add(v);
	}

	internal class Vector2ListAdapter : ValueListAdapter<Vector2>
	{
		protected override VariableValue Get(int index) => VariableValue.Create(_list[index]);
		protected override bool Set(int index, VariableValue value) => value.TryGetVector2(out var v) && Set(index, v);
		protected override bool Add(VariableValue value) => value.TryGetVector2(out var v) && Add(v);
	}

	internal class Vector3ListAdapter : ValueListAdapter<Vector3>
	{
		protected override VariableValue Get(int index) => VariableValue.Create(_list[index]);
		protected override bool Set(int index, VariableValue value) => value.TryGetVector3(out var v) && Set(index, v);
		protected override bool Add(VariableValue value) => value.TryGetVector3(out var v) && Add(v);
	}

	internal class Vector4ListAdapter : ValueListAdapter<Vector4>
	{
		protected override VariableValue Get(int index) => VariableValue.Create(_list[index]);
		protected override bool Set(int index, VariableValue value) => value.TryGetVector4(out var v) && Set(index, v);
		protected override bool Add(VariableValue value) => value.TryGetVector4(out var v) && Add(v);
	}

	internal class QuaternionListAdapter : ValueListAdapter<Quaternion>
	{
		protected override VariableValue Get(int index) => VariableValue.Create(_list[index]);
		protected override bool Set(int index, VariableValue value) => value.TryGetQuaternion(out var v) && Set(index, v);
		protected override bool Add(VariableValue value) => value.TryGetQuaternion(out var v) && Add(v);
	}

	internal class RectListAdapter : ValueListAdapter<Rect>
	{
		protected override VariableValue Get(int index) => VariableValue.Create(_list[index]);
		protected override bool Set(int index, VariableValue value) => value.TryGetRect(out var v) && Set(index, v);
		protected override bool Add(VariableValue value) => value.TryGetRect(out var v) && Add(v);
	}

	internal class BoundsListAdapter : ValueListAdapter<Bounds>
	{
		protected override VariableValue Get(int index) => VariableValue.Create(_list[index]);
		protected override bool Set(int index, VariableValue value) => value.TryGetBounds(out var v) && Set(index, v);
		protected override bool Add(VariableValue value) => value.TryGetBounds(out var v) && Add(v);
	}

	internal class ColorListAdapter : ValueListAdapter<Color>
	{
		protected override VariableValue Get(int index) => VariableValue.Create(_list[index]);
		protected override bool Set(int index, VariableValue value) => value.TryGetColor(out var v) && Set(index, v);
		protected override bool Add(VariableValue value) => value.TryGetColor(out var v) && Add(v);
	}

	internal class StringListAdapter : ValueListAdapter<string>
	{
		protected override VariableValue Get(int index) => VariableValue.Create(_list[index]);
		protected override bool Set(int index, VariableValue value) => value.TryGetString(out var v) && Set(index, v);
		protected override bool Add(VariableValue value) => value.TryGetString(out var v) && Add(v);
	}

	#endregion

	#region Reference Adapters

	internal abstract class ReferenceListAdapter : ListAdapter
	{
		public override int Count => _list.Count;

		protected IList _list;
		protected Type _itemType;

		protected override void Setup(IList list, Type itemType, bool allowSet, bool allowAdd, bool allowRemove)
		{
			_list = list;
			_itemType = itemType;
			Setup(allowSet, allowAdd, allowRemove);
		}

		protected override VariableValue Get(int index)
		{
			return VariableValue.CreateReference(_list[index]);
		}

		protected override void Remove(int index)
		{
			_list.RemoveAt(index);
		}
	}

	internal class EnumListAdapter : ReferenceListAdapter
	{
		protected override bool Set(int index, VariableValue value)
		{
			if (value.EnumType == _itemType)
			{
				_list[index] = value.Enum;
				return true;
			}

			return false;
		}

		protected override bool Add(VariableValue value)
		{
			if (value.EnumType == _itemType)
			{
				_list.Add(value.Enum);
				return true;
			}

			return false;
		}
	}

	internal class ObjectListAdapter : ReferenceListAdapter
	{
		protected override bool Set(int index, VariableValue value)
		{
			if (_itemType.IsAssignableFrom(value.ReferenceType))
			{
				_list[index] = value.Reference;
				return true;
			}

			return false;
		}

		protected override bool Add(VariableValue value)
		{
			if (_itemType.IsAssignableFrom(value.ReferenceType))
			{
				_list.Add(value.Reference);
				return true;
			}

			return false;
		}
	}

	#endregion
}
