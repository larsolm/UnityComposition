using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	[HelpURL(Composition.DocumentationUrl + "variable-pool-component")]
	[AddComponentMenu("PiRho Soft/Composition/Variable Pool Component")]
	public class VariablePoolComponent : MonoBehaviour, IVariableCollection
	{
		public CustomVariableCollection Variables = new CustomVariableCollection();

		#region IVariableStore Implementation

		public IReadOnlyList<string> VariableNames => Variables.VariableNames;
		public Variable GetVariable(string name) => Variables.GetVariable(name);
		public SetVariableResult SetVariable(string name, Variable value) => Variables.SetVariable(name, value);

		#endregion
	}
}
