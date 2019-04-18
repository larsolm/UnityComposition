using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[HelpURL(Composition.DocumentationUrl + "variable-pool-asset")]
	[CreateAssetMenu(menuName = "PiRho Soft/Variable Pool", fileName = nameof(VariablePoolAsset), order = 131)]
	public class VariablePoolAsset : ScriptableObject, IVariableStore
	{
		public VariablePool Variables = new VariablePool();

		#region IVariableStore Implementation

		public VariableValue GetVariable(string name) => Variables.GetVariable(name);
		public SetVariableResult SetVariable(string name, VariableValue value) => Variables.SetVariable(name, value);
		public IList<string> GetVariableNames() => Variables.GetVariableNames();

		#endregion
	}
}
