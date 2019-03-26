﻿using PiRhoSoft.UtilityEngine;
using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[HelpURL(Composition.DocumentationUrl + "variable-store-component")]
	[AddComponentMenu("PiRho Soft/Composition/Variable Store Component")]
	public class VariableStoreComponent : MonoBehaviour, IVariableStore, ISchemaOwner
	{
		[ChangeTrigger(nameof(SetupSchema))] [AssetPopup] public VariableSchema Schema;
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
	}
}
