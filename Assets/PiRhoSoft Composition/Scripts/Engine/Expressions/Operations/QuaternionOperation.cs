using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	public class QuaternionOperation : Operation
	{
		private const string _invalidParameterCountException = "invalid parameters count for Quaternion - must be 3";
		private const string _invalidParametersException = "invalid parameters for Quaternion - must be floats or ints";

		public List<Operation> Parameters { get; private set; }

		public QuaternionOperation(List<Operation> parameters)
		{
			Parameters = parameters;
		}

		public override void ToString(StringBuilder builder)
		{
			builder.Append('{');

			for (var i = 0; i < Parameters.Count; i++)
			{
				if (i != 0)
					builder.Append(", ");

				Parameters[i].ToString(builder);
			}

			builder.Append('}');
		}

		public override void GetInputs(List<VariableDefinition> inputs, string source)
		{
			foreach (var parameter in Parameters)
				parameter.GetInputs(inputs, source);
		}

		public override VariableValue Evaluate(IVariableStore variables)
		{
			if (Parameters.Count == 3)
			{
				var xValue = Parameters[0].Evaluate(variables);
				var yValue = Parameters[1].Evaluate(variables);
				var zValue = Parameters[2].Evaluate(variables);

				if (xValue.HasNumber && yValue.HasNumber && zValue.HasNumber)
					return VariableValue.Create(Quaternion.Euler(xValue.Number, yValue.Number, zValue.Number));
				else
					throw new ExpressionEvaluationException(_invalidParametersException);
			}

			throw new ExpressionEvaluationException(_invalidParameterCountException);
		}
	}
}
