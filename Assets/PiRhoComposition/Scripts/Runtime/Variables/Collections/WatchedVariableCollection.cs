﻿using System.Collections.Generic;

namespace PiRhoSoft.Composition
{
	public interface IVariableListener
	{
		void VariableChanged(string name, Variable value);
	}

	public class WatchedVariableCollection : IVariableCollection
	{
		private IVariableListener _listener;
		private IVariableCollection _store;

		public WatchedVariableCollection(IVariableListener listener, IVariableCollection store)
		{
			_listener = listener;
			_store = store;
		}

		public IReadOnlyList<string> VariableNames
		{
			get => _store.VariableNames;
		}

		public Variable GetVariable(string name)
		{
			return _store.GetVariable(name);
		}

		public SetVariableResult SetVariable(string name, Variable value)
		{
			var result = _store.SetVariable(name, value);

			if (result == SetVariableResult.Success)
				_listener.VariableChanged(name, value);

			return result;
		}
	}
}