using PiRhoSoft.UtilityEngine;
using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[HelpURL(Composition.DocumentationUrl + "variable-store-asset")]
	[CreateAssetMenu(menuName = "PiRho Soft/Variable Store", fileName = nameof(VariableStoreAsset), order = 130)]
	public class VariableStoreAsset : ScriptableObject, IVariableStore
	{
		[ChangeTrigger(nameof(SetupSchema))] [AssetPopup] public VariableSchema Schema;
		public VariableSet Variables;

		public MappedVariableStore Store { get; private set; } = new MappedVariableStore();

		protected virtual void OnEnable()
		{
			SetupSchema();
		}

		protected virtual void SetupSchema()
		{
			Store.Setup(this, Schema, Variables);
		}

		#region IVariableStore Implementation

		public VariableValue GetVariable(string name) => Store.GetVariable(name);
		public SetVariableResult SetVariable(string name, VariableValue value) => Store.SetVariable(name, value);
		public IEnumerable<string> GetVariableNames() => Store.GetVariableNames();

		#endregion
	}
}
