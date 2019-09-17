namespace PiRhoSoft.Composition
{
	internal abstract class MemberOperator : InfixOperation
	{
		public override void GetInputs(VariableDefinitionList inputs, string source)
		{
			if (Left is MemberOperator)
				Left.GetInputs(inputs, source);
		}
	}
}
