using System;

namespace PiRhoSoft.Composition
{
	public enum VariablePermission
	{
		Never,
		Explicit,
		Always
	}

	[Serializable]
	public class VariableCollection : SerializedVariableDictionary
	{
		public VariablePermission AddPermission = VariablePermission.Explicit;
		public VariablePermission RemovePermission = VariablePermission.Explicit;
		public VariablePermission ChangePermission = VariablePermission.Explicit;

		public VariableSchema Schema;

		public IVariableMap Owner { get; }

		public VariableCollection() => Owner = null;
		public VariableCollection(IVariableMap owner) => Owner = owner;

		#region Schema Testing

		private bool IsInSchema(string variable)
		{
			return Schema != null && Schema.HasEntry(variable);
		}

		private bool IsValid(string variable)
		{
			if (Schema == null)
				return true;

			if (!Schema.TryGetEntry(variable, out var entry))
				return true;

			var value = GetVariable(variable);
			return entry.Definition.IsValid(value);
		}

		public bool ImplementsSchema(VariableSchema schema, bool allowExtra)
		{
			if (VariableCount < schema.EntryCount || (!allowExtra && VariableCount > schema.EntryCount))
				return false;

			for (var i = 0; i < schema.EntryCount; i++)
			{
				var entry = schema.GetEntry(i);
				var variable = GetVariable(entry.Definition.Name);

				if (entry.Definition.IsValid(variable)) // this will cover not exists unless the schema wants Empty (which is the same as not existing)
					return false;
			}

			return true;
		}

		#endregion

		#region Schema Updating

		public void SetSchema(VariableSchema schema)
		{
			Schema = schema;
			UpdateSchema();
		}

		public void ResetVariable(string variable)
		{
			var value = GetVariable(variable);

			if (Schema != null && Schema.TryGetEntry(variable, out var entry))
				base.SetVariable(variable, entry.GenerateVariable(Owner));
			else
				base.SetVariable(variable, Variable.Create(value.Type));
		}

		public void ResetSchema()
		{
			if (Schema != null)
			{
				for (var i = 0; i < Schema.EntryCount; i++)
				{
					var entry = Schema.GetEntry(i);
					base.RemoveVariable(entry.Definition.Name);
				}
			}

			UpdateSchema();
		}

		public void UpdateSchema()
		{
			if (Schema != null)
			{
				for (var i = 0; i < Schema.EntryCount; i++)
				{
					var entry = Schema.GetEntry(i);
					var variable = GetVariable(entry.Definition.Name);

					if (!entry.Definition.IsValid(variable))
					{
						var generated = entry.GenerateVariable(Owner);
						var result = base.SetVariable(entry.Definition.Name, generated);

						if (result == SetVariableResult.NotFound)
							base.AddVariable(entry.Definition.Name, generated);
					}
				}
			}
		}

		#endregion

		#region Variable Updates

		public SetVariableResult ChangeVariable(string name, Variable variable)
		{
			var exists = TryGetIndex(name, out var index);

			if (ChangePermission == VariablePermission.Never)
			{
				if (Schema != null && Schema.TryGetEntry(name, out var entry))
				{
					if (!entry.Definition.IsValid(variable))
						return SetVariableResult.TypeMismatch;
				}
				else if (exists)
				{
					if (GetVariable(index).Type != variable.Type)
						return SetVariableResult.TypeMismatch;
				}
			}

			if (exists)
				return base.SetVariable(index, variable);
			else if (AddPermission == VariablePermission.Always)
				return base.AddVariable(name, variable);
			else
				return SetVariableResult.NotFound;
		}

		public override SetVariableResult AddVariable(string name, Variable variable)
		{
			if (AddPermission != VariablePermission.Never)
				return SetVariableResult.ReadOnly;

			return base.AddVariable(name, variable);
		}

		public override SetVariableResult RemoveVariable(string name)
		{
			if (RemovePermission != VariablePermission.Never)
				return SetVariableResult.ReadOnly;

			return base.RemoveVariable(name);
		}

		public override SetVariableResult ClearVariables()
		{
			if (RemovePermission != VariablePermission.Never)
				return SetVariableResult.ReadOnly;

			return base.ClearVariables();
		}

		public override SetVariableResult SetVariable(string name, Variable variable)
		{
			if (TryGetIndex(name, out var index))
				return SetVariable(index, variable);
			else if (AddPermission == VariablePermission.Always)
				return base.AddVariable(name, variable);
			else
				return SetVariableResult.NotFound;
		}

		public override SetVariableResult SetVariable(int index, Variable variable)
		{
			var exists = TryGetName(index, out var name);

			if (ChangePermission != VariablePermission.Always)
			{
				if (Schema != null && Schema.TryGetEntry(name, out var entry))
				{
					if (!entry.Definition.IsValid(variable))
						return SetVariableResult.TypeMismatch;
				}
				else if (exists)
				{
					if (GetVariable(index).Type != variable.Type)
						return SetVariableResult.TypeMismatch;
				}
			}

			if (exists)
				return base.SetVariable(index, variable);
			else
				return SetVariableResult.NotFound;
		}

		#endregion
	}
}
