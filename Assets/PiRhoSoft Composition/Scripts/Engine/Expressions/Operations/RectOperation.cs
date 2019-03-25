using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	public class RectOperation : Operation
	{
		private const string _invalidParameterCountException = "invalid parameters count for Rect - must be 2, 4, or 6";
		private const string _invalidParametersException = "invalid parameters for Rect - must be floats or ints";

		public List<Operation> Parameters { get; private set; }

		public RectOperation(List<Operation> parameters)
		{
			Parameters = parameters;
		}

		public override void ToString(StringBuilder builder)
		{
			builder.Append('[');

			for (var i = 0; i < Parameters.Count; i++)
			{
				if (i != 0)
					builder.Append(", ");

				Parameters[i].ToString(builder);
			}

			builder.Append(']');
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
				var positionValue = Parameters[0].Evaluate(variables);
				var sizeValue = Parameters[1].Evaluate(variables);

				if (positionValue.Type == VariableType.Int2 && sizeValue.Type == VariableType.Int2)
					return VariableValue.Create(new RectInt(positionValue.Int2, sizeValue.Int2));
				else if (positionValue.Type == VariableType.Int3 && sizeValue.Type == VariableType.Int3)
					return VariableValue.Create(new BoundsInt(positionValue.Int3, sizeValue.Int3));
				else if (positionValue.HasNumber2 && sizeValue.HasNumber2)
					return VariableValue.Create(new Rect(positionValue.Number2, sizeValue.Number2));
				else if (positionValue.HasNumber3 && sizeValue.HasNumber3)
					return VariableValue.Create(new Bounds(positionValue.Number3, sizeValue.Number3));
				else
					throw new ExpressionEvaluationException(_invalidParametersException);
			}
			else if (Parameters.Count == 4)
			{
				var xValue = Parameters[0].Evaluate(variables);
				var yValue = Parameters[1].Evaluate(variables);
				var widthValue = Parameters[2].Evaluate(variables);
				var heightValue = Parameters[3].Evaluate(variables);

				if (xValue.Type == VariableType.Int && yValue.Type == VariableType.Int && widthValue.Type == VariableType.Int && heightValue.Type == VariableType.Int)
					return VariableValue.Create(new RectInt(xValue.Int, yValue.Int, widthValue.Int, heightValue.Int));
				else if (xValue.HasNumber && yValue.HasNumber && widthValue.HasNumber && heightValue.HasNumber)
					return VariableValue.Create(new Rect(xValue.Number, yValue.Number, widthValue.Number, heightValue.Number));
				else
					throw new ExpressionEvaluationException(_invalidParametersException);
			}
			else if (Parameters.Count == 6)
			{
				var xValue = Parameters[0].Evaluate(variables);
				var yValue = Parameters[1].Evaluate(variables);
				var zValue = Parameters[2].Evaluate(variables);
				var widthValue = Parameters[3].Evaluate(variables);
				var heightValue = Parameters[4].Evaluate(variables);
				var depthValue = Parameters[5].Evaluate(variables);

				if (xValue.Type == VariableType.Int && yValue.Type == VariableType.Int && zValue.Type == VariableType.Int && widthValue.Type == VariableType.Int && heightValue.Type == VariableType.Int && depthValue.Type == VariableType.Int)
					return VariableValue.Create(new BoundsInt(xValue.Int, yValue.Int, zValue.Int, widthValue.Int, heightValue.Int, depthValue.Int));
				else
					throw new ExpressionEvaluationException(_invalidParametersException);
			}

			throw new ExpressionEvaluationException(_invalidParameterCountException);
		}
	}
}
