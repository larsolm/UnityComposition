namespace PiRhoSoft.CompositionEngine
{
	public class LocalVariableStore<T> : VariableStore where T : IVariableStore
	{
		protected T _store;

		protected LocalVariableStore(T store)
		{
			_store = store;
		}

		public override VariableValue GetVariable(string name)
		{
			var value = _store.GetVariable(name);
			return value.Type != VariableType.Empty ? value : base.GetVariable(name);
		}

		public override SetVariableResult SetVariable(string name, VariableValue value)
		{
			var result = _store.SetVariable(name, value);
			return result != SetVariableResult.NotFound ? result : base.SetVariable(name, value);
		}
	}
}
