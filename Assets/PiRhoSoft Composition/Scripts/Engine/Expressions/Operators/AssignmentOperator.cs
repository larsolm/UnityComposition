using System.Collections.Generic;

namespace PiRhoSoft.CompositionEngine
{
	public interface IAssignableOperation
	{
		SetVariableResult SetValue(IVariableStore variables, VariableValue value);
	}

	public abstract class AssignmentOperator : InfixOperation
	{
		private const string _invalidAssignmentException = "unable to assign '{0}' to '{1}'";

		public override OperatorPrecedence Precedence => OperatorPrecedence.Assignment;

		public override void GetOutputs(IList<VariableDefinition> outputs, string source)
		{
			Left.GetOutputs(outputs, source);
			Right.GetOutputs(outputs, source); // for chained assignment (output.value1 = output.value2 = value) which is supported but not advertised
		}

		protected VariableValue Assign(IVariableStore variables, VariableValue value)
		{
			if (Left is IAssignableOperation assignable)
			{
				assignable.SetValue(variables, value);
				return value;
			}
			else
			{
				throw new ExpressionEvaluationException(_invalidAssignmentException, value, Left);
			}
		}
	}
}
