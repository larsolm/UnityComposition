using PiRhoSoft.CompositionEngine;
using PiRhoSoft.UtilityEngine;
using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.CompositionExample
{
	[AddComponentMenu("PiRho Soft/Examples/World Manager")]
	public class WorldManager : MonoBehaviour, IVariableStore
	{
		[ChangeTrigger(nameof(SetupSchema))] [AssetPopup] public VariableSchema Schema;

		public VariableList Variables = new VariableList();
		public MappedVariableStore Store = new MappedVariableStore();

		[MappedVariable] public bool BoolField = true;
		[MappedVariable] public bool BoolProperty { get; set; }

		[MappedVariable] public Character ObjectField;
		[MappedVariable] public Character ObjectProperty { get; set; }

		[MappedVariable] public IVariableStore StoreField;
		[MappedVariable] public IVariableStore StoreProperty { get; set; }

		void OnEnable()
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
