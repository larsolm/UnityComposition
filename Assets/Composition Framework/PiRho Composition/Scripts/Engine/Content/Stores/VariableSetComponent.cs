using PiRhoSoft.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	[HelpURL(Composition.DocumentationUrl + "variable-set-component")]
	[AddComponentMenu("PiRho Soft/Composition/Variable Set Component")]
	public class VariableSetComponent : MonoBehaviour, IVariableStore, IVariableReset, ISchemaOwner
	{
		[ChangeTrigger(nameof(SetupSchema))]
		[ObjectPicker]
		[SerializeField]
		private VariableSchema _schema = null;

		public VariableSet Variables;

		public VariableSchema Schema => _schema;
		public MappedVariableStore Store { get; private set; } = new MappedVariableStore();

		protected virtual void OnEnable()
		{
			SetupSchema();
		}

		public void SetupSchema()
		{
			Store.Setup(this, _schema, Variables);
		}

		#region IVariableStore Implementation

		public VariableValue GetVariable(string name) => Store.GetVariable(name);
		public SetVariableResult SetVariable(string name, VariableValue value) => Store.SetVariable(name, value);
		public IList<string> GetVariableNames() => Store.GetVariableNames();

		#endregion

		#region IVariableReset Implementation

		public void ResetTag(string tag) => Variables.ResetTag(tag);
		public void ResetVariables(IList<string> variables) => Variables.ResetVariables(variables);

		#endregion
	}

	[Serializable]
	public class VariableSetComponentSource : VariableSource<VariableSetComponent> { }
}
