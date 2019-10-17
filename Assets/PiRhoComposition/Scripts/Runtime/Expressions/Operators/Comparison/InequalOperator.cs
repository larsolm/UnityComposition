namespace PiRhoSoft.Composition
{
	internal class InequalOperator : EqualOperator
	{
		public override OperatorPrecedence Precedence => OperatorPrecedence.Equality;

		public override Variable Evaluate(IVariableMap variables)
		{
			var equal = base.Evaluate(variables);
			return Variable.Bool(!equal.AsBool);
		}
	}
}
