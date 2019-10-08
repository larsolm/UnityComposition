using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	[HelpURL(Configuration.DocumentationUrl + "variable-set-component")]
	[AddComponentMenu("PiRho Soft/Composition/Variable Set Component")]
	public class VariableSetComponent : MonoBehaviour, IVariableCollection, IResettableVariables
	{
		public SchemaVariableCollection SchemaVariables;
		public MappedVariableCollection ClassVariables;
		public CombinedVariableCollection Variables;

		public VariableSetComponent()
		{
			SchemaVariables = new SchemaVariableCollection(this);
			ClassVariables = new MappedVariableCollection(this);
			Variables = new CombinedVariableCollection(SchemaVariables, ClassVariables);
		}

		#region IVariableStore Implementation

		public Variable GetVariable(string name) => Variables.GetVariable(name);
		public SetVariableResult SetVariable(string name, Variable value) => Variables.SetVariable(name, value);
		public IReadOnlyList<string> VariableNames => Variables.VariableNames;

		#endregion

		#region IVariableReset Implementation

		public void ResetTag(string tag) => SchemaVariables.ResetTag(tag);
		public void ResetVariables(IList<string> variables) => SchemaVariables.ResetVariables(variables);
		public void ResetAll() => SchemaVariables.ResetAll();

		#endregion
	}
}
