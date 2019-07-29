using System.Collections;

namespace PiRhoSoft.Composition
{
	public class ListAdapter : IVariableList
	{
		private IList _list;

		private bool _allowSet;
		private bool _allowAdd;
		private bool _allowRemove;

		public ListAdapter(IList list)
		{
			_list = list;

			_allowSet = !list.IsReadOnly;
			_allowAdd = !list.IsReadOnly && !list.IsFixedSize;
			_allowRemove = !list.IsReadOnly && !list.IsFixedSize;
		}

		public int VariableCount => _list.Count;
		
		public Variable GetVariable(int index)
		{
			if (index >= 0 && index <= VariableCount)
				return Get(index);
			else
				return Variable.Empty;
		}

		public SetVariableResult SetVariable(int index, Variable value)
		{
			if (!_allowSet)
			{
				return SetVariableResult.ReadOnly;
			}
			else if (index >= 0 && index <= VariableCount)
			{
				try
				{
					Set(index, value);
					return SetVariableResult.Success;
				}
				catch
				{
					return SetVariableResult.TypeMismatch;
				}
			}

			return SetVariableResult.NotFound;
		}

		public SetVariableResult AddVariable(Variable value)
		{
			if (!_allowAdd)
				return SetVariableResult.ReadOnly;

			try
			{
				if (Add(value))
					return SetVariableResult.Success;
			}
			catch
			{
			}

			return SetVariableResult.TypeMismatch;
		}

		public SetVariableResult RemoveVariable(int index)
		{
			if (!_allowRemove)
				return SetVariableResult.ReadOnly;

			if (index >= 0 && index <= VariableCount)
			{
				Remove(index);
				return SetVariableResult.Success;
			}

			return SetVariableResult.NotFound;
		}

		protected Variable Get(int index) => Variable.Unbox(_list[index]);
		protected void Set(int index, Variable value) => _list[index] = value.Box();
		protected bool Add(Variable value) => _list.Add(value.Box()) >= 0;
		protected void Remove(int index) => _list.RemoveAt(index);
	}
}