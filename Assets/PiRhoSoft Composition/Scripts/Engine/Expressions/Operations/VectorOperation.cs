using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	public class VectorOperation : Operation
	{
		private const string _invalidParameterCountException = "invalid parameters count for Vector - must be 2, 3, or 4";
		private const string _invalidParametersException = "invalid parameters for Vector - must be floats or ints";

		public List<Operation> Parameters { get; private set; }

		public VectorOperation(List<Operation> parameters)
		{
			Parameters = parameters;
		}

		public override void ToString(StringBuilder builder)
		{
			builder.Append('<');

			for (var i = 0; i < Parameters.Count; i++)
			{
				if (i != 0)
					builder.Append(", ");

				Parameters[i].ToString(builder);
			}

			builder.Append('>');
		}

		public override void GetInputs(List<VariableDefinition> inputs, string source)
		{
			foreach (var parameter in Parameters)
				parameter.GetInputs(inputs, source);
		}

		public override VariableValue Evaluate(IVariableStore variables)
		{
			if (Parameters.Count == 2)
			{
				var xValue = Parameters[0].Evaluate(variables);
				var yValue = Parameters[1].Evaluate(variables);

				if (xValue.Type == VariableType.Int && yValue.Type == VariableType.Int)
					return VariableValue.Create(new Vector2Int(xValue.Int, yValue.Int));
				else if (xValue.HasNumber && yValue.HasNumber)
					return VariableValue.Create(new Vector2(xValue.Number, yValue.Number));
				else
					throw new ExpressionEvaluationException(_invalidParametersException);
			}
			else if (Parameters.Count == 3)
			{
				var xValue = Parameters[0].Evaluate(variables);
				var yValue = Parameters[1].Evaluate(variables);
				var zValue = Parameters[2].Evaluate(variables);

				if (xValue.Type == VariableType.Int && yValue.Type == VariableType.Int && zValue.Type == VariableType.Int)
					return VariableValue.Create(new Vector3Int(xValue.Int, yValue.Int, zValue.Int));
				else if (xValue.HasNumber && yValue.HasNumber && zValue.HasNumber)
					return VariableValue.Create(new Vector3(xValue.Number, yValue.Number, zValue.Number));
				else
					throw new ExpressionEvaluationException(_invalidParametersException);
			}
			else if (Parameters.Count == 2)
			{
				var xValue = Parameters[0].Evaluate(variables);
				var yValue = Parameters[1].Evaluate(variables);
				var zValue = Parameters[2].Evaluate(variables);
				var wValue = Parameters[3].Evaluate(variables);

				if (xValue.HasNumber && yValue.HasNumber && zValue.HasNumber && wValue.HasNumber)
					return VariableValue.Create(new Vector4(xValue.Number, yValue.Number, zValue.Number, wValue.Number));
				else
					throw new ExpressionEvaluationException(_invalidParametersException);
			}

			throw new ExpressionEvaluationException(_invalidParameterCountException);
		}
	}
}
