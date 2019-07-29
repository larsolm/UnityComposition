﻿using PiRhoSoft.Utilities;
using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	[HelpURL(Composition.DocumentationUrl + "variable-set-asset")]
	[CreateAssetMenu(menuName = "PiRho Soft/Variable Set", fileName = nameof(VariableSetAsset), order = 113)]
	public class VariableSetAsset : ScriptableObject, IVariableCollection, IResettableVariables
	{
		[ChangeTrigger(nameof(SetupSchema))]
		[ObjectPicker]
		[SerializeField]
		private VariableSchema _schema = null;

		public ConstrainedStore Variables;
		public VariableSchema Schema => _schema;

		private MultiStore _store;

		protected virtual void OnEnable()
		{
			SetupSchema();
		}

		public void SetupSchema()
		{
			Variables.Setup(Schema, this);
			_store = new MultiStore(new ClassStore(this), Variables);
		}

		#region IVariableStore Implementation

		public IReadOnlyList<string> VariableNames => _store.VariableNames;
		public Variable GetVariable(string name) => _store.GetVariable(name);
		public SetVariableResult SetVariable(string name, Variable value) => _store.SetVariable(name, value);

		#endregion

		#region IVariableReset Implementation

		public void ResetTag(string tag) => Variables.ResetTag(tag);
		public void ResetVariables(IList<string> variables) => Variables.ResetVariables(variables);
		public void ResetAll() => Variables.ResetAll();

		#endregion
	}
}
