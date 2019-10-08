using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	[HelpURL(Configuration.DocumentationUrl + "variable-pool-asset")]
	[CreateAssetMenu(menuName = "PiRho Soft/Variable Pool", fileName = nameof(VariablePoolAsset), order = 114)]
	public class VariablePoolAsset : ScriptableObject, IVariableCollection
	{
		public CustomVariableCollection Variables = new CustomVariableCollection();

		#region IVariableStore Implementation

		public Variable GetVariable(string name) => Variables.GetVariable(name);
		public SetVariableResult SetVariable(string name, Variable value) => Variables.SetVariable(name, value);
		public IReadOnlyList<string> VariableNames => Variables.VariableNames;

		#endregion
	}
}
