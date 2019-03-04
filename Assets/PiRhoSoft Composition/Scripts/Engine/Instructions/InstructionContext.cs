using System.Collections.Generic;

namespace PiRhoSoft.CompositionEngine
{
	public class InstructionContext
	{
		public Dictionary<string, IVariableStore> Stores { get; } = new Dictionary<string, IVariableStore>();

		public void SetStore(string name, IVariableStore store)
		{
			Stores[name] = store;
		}

		public void Clear()
		{
			Stores.Clear();
		}
	}
}
