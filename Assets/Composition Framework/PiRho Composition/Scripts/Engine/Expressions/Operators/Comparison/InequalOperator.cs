namespace PiRhoSoft.Composition
{
	internal class InequalOperator : EqualOperator
	{
		public override OperatorPrecedence Precedence => OperatorPrecedence.Equality;

		public override VariableValue Evaluate(IVariableStore variables)
		{
			var equal = base.Evaluate(variables);
			return VariableValue.Create(!equal.Bool);
		}
	}
}
