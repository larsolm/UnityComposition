using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	[HelpURL(Configuration.DocumentationUrl + "custom-variable-component")]
	[AddComponentMenu("PiRho Composition/Custom Variables")]
	public class CustomVariableComponent : MonoBehaviour, IVariableArray, IVariableDictionary
	{
		public CustomVariableCollection Variables = new CustomVariableCollection();

		#region IVariableArray Implementation

		public int VariableCount => Variables.VariableCount;
		public Variable GetVariable(int index) => Variables.GetVariable(index);
		public SetVariableResult SetVariable(int index, Variable variable) => Variables.SetVariable(index, variable);

		#endregion

		#region IVariableMap Implementation

		public IReadOnlyList<string> VariableNames => Variables.VariableNames;
		public Variable GetVariable(string name) => Variables.GetVariable(name);
		public SetVariableResult SetVariable(string name, Variable variable) => Variables.SetVariable(name, variable);

		#endregion

		#region IVariableDictionary Implementation

		public SetVariableResult AddVariable(string name, Variable variable) => Variables.AddVariable(name, variable);
		public SetVariableResult RemoveVariable(string name) => Variables.RemoveVariable(name);
		public SetVariableResult ClearVariables() => Variables.ClearVariables();

		#endregion
	}
}
