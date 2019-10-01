using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	[HelpURL(Configuration.DocumentationUrl + "variable-set-asset")]
	[CreateAssetMenu(menuName = "PiRho Soft/Variable Set", fileName = nameof(VariableSetAsset), order = 113)]
	public class VariableSetAsset : ScriptableObject, IVariableCollection, IResettableVariables
	{
		public SchemaVariableCollection SchemaVariables;
		public MappedVariableCollection ClassVariables;
		public CombinedVariableCollection Variables;

		public VariableSetAsset()
		{
			SchemaVariables = new SchemaVariableCollection(this);
			ClassVariables = new MappedVariableCollection(this);
			Variables = new CombinedVariableCollection(SchemaVariables, ClassVariables);
		}

		#region IVariableStore Implementation

		public IReadOnlyList<string> VariableNames => Variables.VariableNames;
		public Variable GetVariable(string name) => Variables.GetVariable(name);
		public SetVariableResult SetVariable(string name, Variable value) => Variables.SetVariable(name, value);

		#endregion

		#region IVariableReset Implementation

		public void ResetTag(string tag) => SchemaVariables.ResetTag(tag);
		public void ResetVariables(IList<string> variables) => SchemaVariables.ResetVariables(variables);
		public void ResetAll() => SchemaVariables.ResetAll();

		#endregion
	}
}
