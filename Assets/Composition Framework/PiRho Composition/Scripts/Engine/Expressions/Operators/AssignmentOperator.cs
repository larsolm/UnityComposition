namespace PiRhoSoft.Composition
{
	public interface IAssignableOperation
	{
		SetVariableResult SetValue(IVariableCollection variables, Variable value);
	}

	public abstract class AssignmentOperator : InfixOperation
	{
		private const string _invalidAssignmentException = "unable to assign '{0}' to '{1}'";

		public override OperatorPrecedence Precedence => OperatorPrecedence.Assignment;

		public override void GetOutputs(VariableDefinitionList outputs, string source)
		{
			Left.GetOutputs(outputs, source);
		}

		protected Variable Assign(IVariableCollection variables, Variable value)
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
