using PiRhoSoft.UtilityEngine;
using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[HelpURL(Composition.DocumentationUrl + "variable-store-component")]
	[AddComponentMenu("PiRho Soft/Composition/Variable Store Component")]
	public class VariableStoreComponent : MonoBehaviour, IVariableStore, IVariableReset, ISchemaOwner
	{
		[ChangeTrigger(nameof(SetupSchema))]
		[AssetDisplay(SaveLocation = AssetLocation.Selectable)]
		public VariableSchema Schema;

		public VariableSet Variables;

		public MappedVariableStore Store { get; private set; } = new MappedVariableStore();

		protected virtual void OnEnable()
		{
			SetupSchema();
		}

		public void SetupSchema()
		{
			Store.Setup(this, Schema, Variables);
		}

		#region IVariableStore Implementation

		public VariableValue GetVariable(string name) => Store.GetVariable(name);
		public SetVariableResult SetVariable(string name, VariableValue value) => Store.SetVariable(name, value);
		public IEnumerable<string> GetVariableNames() => Store.GetVariableNames();

		#endregion

		#region IVariableReset Implementation

		public void ResetTag(string tag) => Variables.ResetTag(tag);
		public void ResetVariables(IList<string> variables) => Variables.ResetVariables(variables);

		#endregion
	}
}
