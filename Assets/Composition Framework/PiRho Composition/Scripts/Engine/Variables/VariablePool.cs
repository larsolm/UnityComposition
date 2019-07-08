using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PiRhoSoft.Composition.Engine
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
			var variableName = Names[index];

			if (variableName != name)
			{
				Map.Remove(variableName);
				Map.Add(name, index);
				
				Names[index] = name;
			}
		}

		public void ChangeDefinition(int index, ValueDefinition definition)
		{
			var variable = Variables[index];

			if (!definition.IsValid(variable))
				Variables[index] = definition.Generate(null);

			Definitions[index] = definition;
		}

		public SetVariableResult SetVariable(int index, VariableValue value)
		{
			var name = Names[index];
			var result = SetVariable(name, value);

			if (result == SetVariableResult.Success && value.Type != Definitions[index].Type)
				Definitions[index] = ValueDefinition.Create(value.Type, null, null, null, false, false);

			return result;
		}

		#region ISerializationCallbackReceiver Implementation

		public void OnBeforeSerialize()
		{
			var variables = new List<Variable>();

			for (var i = 0; i < Names.Count && i < Variables.Count; i++)
				variables.Add(Variable.Create(Names[i], Variables[i]));

			_variablesData = VariableHandler.SaveVariables(variables, ref _variablesObjects);
		}

		public void OnAfterDeserialize()
		{
			var variables = VariableHandler.LoadVariables(ref _variablesData, ref _variablesObjects);

			base.Clear();

			foreach (var variable in variables)
				base.AddVariable(variable.Name, variable.Value); // bypass the override so the definition isn't added
		}

		#endregion
	}
}
