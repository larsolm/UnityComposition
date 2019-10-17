using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	[HelpURL(Configuration.DocumentationUrl + "schema-variable-asset")]
	[CreateAssetMenu(menuName = "PiRho Composition/Schema Variables", fileName = nameof(SchemaVariableAsset), order = 113)]
	public class SchemaVariableAsset : ScriptableObject, IVariableArray, IVariableDictionary
	{
		public SchemaVariableCollection Variables = new SchemaVariableCollection();

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
