using System;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	[Serializable]
	public class SchemaVariableCollection : SerializedVariableDictionary
	{
		public const string SchemaProperty = nameof(_schema);

		[SerializeField] private VariableSchema _schema = null;
		[SerializeField] private int _schemaVersion = 1;
		private bool _locked = false;

		public VariableSchema Schema => _schema;
		public IVariableMap Owner { get; }

		public SchemaVariableCollection() => Owner = null;
		public SchemaVariableCollection(IVariableMap owner) => Owner = owner;

		public void SetSchema(VariableSchema schema)
		{
			_locked = false;

			if (schema == null)
				ClearSchema();
			else if (schema != _schema || schema.Version != _schemaVersion)
				ApplySchema(schema, Owner);

			_locked = true;
		}

		public void UpdateSchema()
		{
			SetSchema(Schema);
		}

		private void ClearSchema()
		{
			_schema = null;
			_schemaVersion = 1;
			ClearVariables();
		}

		protected override void Load()
		{
			_locked = false;
			base.Load();
			SetSchema(_schema);
		}

		#region IVariableArray Implementation

		public override SetVariableResult SetVariable(int index, Variable variable)
		{
			var entry = _schema.GetEntry(index);

			if (entry == null)
				return SetVariableResult.NotFound;
			
			if (!entry.Definition.IsValid(variable))
				return SetVariableResult.TypeMismatch;

			return base.SetVariable(index, variable);
		}

		#endregion

		#region IVariableMap Implementation

		public override SetVariableResult SetVariable(string name, Variable variable)
		{
			var entry = _schema.GetEntry(name);

			if (entry == null)
				return SetVariableResult.NotFound;

			if (!entry.Definition.IsValid(variable))
				return SetVariableResult.TypeMismatch;

			return base.SetVariable(name, variable);
		}

		#endregion

		#region IVariableDictionary Implementation

		public override SetVariableResult AddVariable(string name, Variable variable)
		{
			if (_locked)
				return SetVariableResult.ReadOnly;

			return base.AddVariable(name, variable);
		}

		public override SetVariableResult RemoveVariable(string name)
		{
			if (_locked)
				return SetVariableResult.ReadOnly;

			return base.RemoveVariable(name);
		}

		public override SetVariableResult ClearVariables()
		{
			if (_locked)
				return SetVariableResult.ReadOnly;

			return base.ClearVariables();
		}

		#endregion
	}
}
