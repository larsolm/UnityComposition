using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PiRhoSoft.CompositionEngine
{
	[Serializable]
	public class VariablePool : VariableStore, ISerializationCallbackReceiver
	{
		[SerializeField] private List<string> _variablesData;
		[SerializeField] private List<Object> _variablesObjects;

		public List<ValueDefinition> Definitions = new List<ValueDefinition>();

		public override void AddVariable(string name, VariableValue value)
		{
			Definitions.Add(ValueDefinition.Create(value.Type, null, null, null, false, false));
			base.AddVariable(name, value);
		}

		protected override void RemoveVariable(string name, int index)
		{
			Definitions.RemoveAt(index);
			base.RemoveVariable(name, index);
		}

		public override void Clear()
		{
			Definitions.Clear();
			base.Clear();
		}

		public override void VariableMoved(int from, int to)
		{
			var definition = Definitions[from];

			Definitions.RemoveAt(from);
			Definitions.Insert(to, definition);

			base.VariableMoved(from, to);
		}

		public void ChangeName(int index, string name)
		{
			var variable = Variables[index];

			if (variable.Name != name)
			{
				RemoveVariable(variable.Name);
				AddVariable(name, variable.Value);
			}
		}

		public void ChangeDefinition(int index, ValueDefinition definition)
		{
			var variable = Variables[index];

			if (!definition.IsValid(variable.Value))
				Variables[index] = Variable.Create(variable.Name, definition.Generate(null));

			Definitions[index] = definition;
		}

		public SetVariableResult SetVariable(int index, VariableValue value)
		{
			var variable = Variables[index];
			var result = SetVariable(variable.Name, value);

			if (result == SetVariableResult.Success && value.Type != Definitions[index].Type)
				Definitions[index] = ValueDefinition.Create(value.Type, null, null, null, false, false);

			return result;
		}

		#region ISerializationCallbackReceiver Implementation

		public void OnBeforeSerialize()
		{
			Variable.Save(Variables, ref _variablesData, ref _variablesObjects);
		}

		public void OnAfterDeserialize()
		{
			var variables = new List<Variable>();

			Variable.Load(variables, ref _variablesData, ref _variablesObjects);

			base.Clear();

			foreach (var variable in variables)
				base.AddVariable(variable.Name, variable.Value); // bypass the override so the definition isn't added
		}

		#endregion
	}
}
