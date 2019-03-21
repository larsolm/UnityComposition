namespace PiRhoSoft.CompositionEngine
{
	public interface IVariableListener
	{
		void VariableChanged(int index, VariableValue value);
	}

	public class VariableListener : IMappedVariableList
	{
		private IVariableListener _listener;
		private IMappedVariableList _list;

		public VariableListener(IVariableListener listener, IMappedVariableList list)
		{
			_listener = listener;
			_list = list;
		}

		#region IVariableList Implementation

		public int VariableCount
		{
			get { return _list.VariableCount; }
		}

		public string GetVariableName(int index)
		{
			return _list.GetVariableName(index);
		}

		public VariableValue GetVariableValue(int index)
		{
			return _list.GetVariableValue(index);
		}

		public SetVariableResult SetVariableValue(int index, VariableValue value)
		{
			var result = _list.SetVariableValue(index, value);
			
			if (result == SetVariableResult.Success)
				_listener.VariableChanged(index, value);

			return result;
		}

		#endregion
	}
}
