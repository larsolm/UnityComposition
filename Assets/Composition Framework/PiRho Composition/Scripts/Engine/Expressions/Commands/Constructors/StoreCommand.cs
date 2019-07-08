using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.Composition.Engine
{
	internal class StoreCommand : ICommand
	{
		private const string _invalidSchemaWarning = "(CECSCIS) Failed to create constrained variable store with the schema '{0}': the schema could not be found. Make sure it is in a \"Resources\" folder and the correct path was specified.";

		public VariableValue Evaluate(IVariableStore variables, string name, List<Operation> parameters)
		{
			if (parameters.Count == 0)
			{
				return VariableValue.Create(new VariableStore());
			}
			else if (parameters.Count == 1)
			{
				var schemaName = parameters[0].Evaluate(variables);

				if (!schemaName.HasString)
					throw CommandEvaluationException.WrongParameterType(name, 0, schemaName.Type, VariableType.String);

				var schema = Resources.Load<VariableSchema>(schemaName.String);

				if (!schema)
					Debug.LogWarningFormat(_invalidSchemaWarning, schemaName.String);

				return VariableValue.Create(schema ? new ConstrainedStore(schema) : new VariableStore());
			}

			throw CommandEvaluationException.WrongParameterCount(name, parameters.Count, 0, 1);
		}
	}
}
