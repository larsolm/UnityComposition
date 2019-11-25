using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	internal class StoreCommand : ICommand
	{
		private const string _invalidSchemaWarning = "(CECSCIS) Failed to create constrained variable store with the schema '{0}': the schema could not be found. Make sure it is in a \"Resources\" folder and the correct path was specified.";

		public Variable Evaluate(IVariableMap variables, string name, List<Operation> parameters)
		{
			if (parameters.Count == 0)
			{
				return Variable.Dictionary(new VariableDictionary());
			}
			else if (parameters.Count == 1)
			{
				var schemaName = parameters[0].Evaluate(variables);

				if (!schemaName.IsString)
					throw CommandEvaluationException.WrongParameterType(name, 0, schemaName.Type, VariableType.String);

				var schema = Resources.Load<VariableSchema>(schemaName.AsString);

				if (!schema)
					Debug.LogWarningFormat(_invalidSchemaWarning, schemaName.AsString);

				var dictionary = new VariableCollection();
				dictionary.ApplySchema(schema, null);

				return Variable.Dictionary(dictionary);
			}

			throw CommandEvaluationException.WrongParameterCount(name, parameters.Count, 0, 1);
		}
	}
}
