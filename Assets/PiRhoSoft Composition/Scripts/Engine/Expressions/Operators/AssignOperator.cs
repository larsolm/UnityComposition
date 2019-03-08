using System.Collections.Generic;

namespace PiRhoSoft.CompositionEngine
{
	public class AssignOperator : InfixOperation
	{
		private const string _invalidAssignmentException = "only identifiers can be assigned";
		private const string _missingAssignmentException = "the variable '{0}' could not be found";
		private const string _readOnlyAssignmentException = "the variable '{0}' is read only and cannot be assigned";
		private const string _mismatchedAssignmentException = "the variable '{0}' cannot be assigned a value of type {1}";

		public override VariableValue Evaluate(IVariableStore variables)
		{
			var result = Right.Evaluate(variables);
			return Assign(variables, ref result);
		}

		protected VariableValue Assign(IVariableStore variables, ref VariableValue value)
		{
			var left = Left as LookupOperation;

			if (left == null)
				throw new ExpressionEvaluationException(_invalidAssignmentException);

			var result = left.Reference.SetValue(variables, value);

			switch (result)
			{
				case SetVariableResult.Success: break;
				case SetVariableResult.NotFound: throw new ExpressionEvaluationException(_missingAssignmentException, left.Reference);
				case SetVariableResult.ReadOnly: throw new ExpressionEvaluationException(_readOnlyAssignmentException, left.Reference);
				case SetVariableResult.TypeMismatch: throw new ExpressionEvaluationException(_mismatchedAssignmentException, left.Reference, value.Type);
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
		}
	}
}
