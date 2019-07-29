using PiRhoSoft.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	[HelpURL(Composition.DocumentationUrl + "variable-set-component")]
	[AddComponentMenu("PiRho Soft/Composition/Variable Set Component")]
	public class VariableSetComponent : MonoBehaviour, IVariableCollection, IResettableVariables
	{
		[ChangeTrigger(nameof(SetupSchema))]
		[ObjectPicker]
		[SerializeField]
		private VariableSchema _schema = null;

		public ConstrainedStore SchemaVariables;
		public ClassStore ClassVariables;
		public MultiStore Variables;

		public VariableSchema Schema => _schema;

		public VariableSetComponent()
		{
			SchemaVariables = new ConstrainedStore();
			ClassVariables = new ClassStore(this);
			Variables = new MultiStore(SchemaVariables, ClassVariables);
		}

		protected virtual void OnEnable()
		{
			SetupSchema();
		}

		public void SetupSchema()
		{
			SchemaVariables.Setup(_schema, this);
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

	[Serializable]
	public class VariableSetComponentSource : VariableSource<VariableSetComponent> { }
}
