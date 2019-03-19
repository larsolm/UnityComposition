using System.Collections.Generic;

namespace PiRhoSoft.CompositionEngine
{
	public abstract class AssignmentOperation : InfixOperation
	{
		protected VariableValue Assign(IVariableStore variables, ref VariableValue value)
		{
			var left = Left as LookupOperation;

			if (left == null)
				throw ExpressionEvaluationException.InvalidAssignment(Symbol);

			var result = left.SetValue(variables, value);

			switch (result)
			{
				case SetVariableResult.NotFound: throw ExpressionEvaluationException.MissingAssignment(Symbol, left.Reference);
				case SetVariableResult.ReadOnly: throw ExpressionEvaluationException.ReadOnlyAssignment(Symbol, left.Reference);
				case SetVariableResult.TypeMismatch: throw ExpressionEvaluationException.MismatchedAssignment(Symbol, left.Reference, value.Type);
			}

			return value;
		}

		public override void GetInputs(List<VariableDefinition> inputs, string source)
		{
			Left.GetInputs(inputs, source);
			Right.GetInputs(inputs, source);
		}

		public override void GetOutputs(List<VariableDefinition> outputs, string source)
		{
			Left.GetOutputs(outputs, source);
			Right.GetOutputs(outputs, source);
		}
	}
}
