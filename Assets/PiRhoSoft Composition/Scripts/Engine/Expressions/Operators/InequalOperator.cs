namespace PiRhoSoft.CompositionEngine
{
	public class InequalOperator : EqualOperator
	{
		public override VariableValue Evaluate(IVariableStore variables)
		{
			var equal = base.Evaluate(variables);
			return VariableValue.Create(!equal.Bool);
		}
	}
}
