using System.Collections.Generic;
using System.Text;

namespace PiRhoSoft.CompositionEngine
{
	public class LookupOperation : Operation
	{
		private const string _missingVariableException = "the variable '{0}' could not be found";

		public VariableReference Reference = new VariableReference();

		public LookupOperation(string variable)
		{
			Reference.Update(variable);
		}

		public override VariableValue Evaluate(IVariableStore variables)
		{
			var value = Reference.GetValue(variables);

			if (value.Type == VariableType.Empty)
				throw new ExpressionEvaluationException(_missingVariableException, Reference);

			return value;
		}

		public override void ToString(StringBuilder builder)
		{
			builder.Append(Reference.ToString());
		}

		public override void GetInputs(List<VariableDefinition> inputs, string source)
		{
			if (source == null)
			{
				if (Reference.IsAssigned && string.IsNullOrEmpty(Reference.StoreName))
					inputs.Add(VariableDefinition.Create(Reference.RootName, VariableType.Empty));
			}
			else if (Reference.IsAssigned && Reference.StoreName.ToLowerInvariant() == source.ToLowerInvariant())
			{
				inputs.Add(VariableDefinition.Create(Reference.RootName, VariableType.Empty));
			}
		}
	}
}
