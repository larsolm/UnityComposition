using System.Collections.Generic;
using System.Text;

namespace PiRhoSoft.CompositionEngine
{
	public class LookupOperation : Operation
	{
		public VariableReference Reference { get; private set; }
		public Operation Parameter { get; private set; }
		public LookupOperation(string variable, Operation parameter)
		{
			Reference = new VariableReference(variable);
			Parameter = parameter;
		}

		public override void ToString(StringBuilder builder)
		{
			builder.Append(Reference.ToString());
		}

		public override void GetInputs(List<VariableDefinition> inputs, string source)
		{
			GetVariables(inputs, source);
			Parameter?.GetInputs(inputs, source);
		}

		public override void GetOutputs(List<VariableDefinition> outputs, string source)
		{
			GetVariables(outputs, source);
		}

		private void GetVariables(List<VariableDefinition> inputs, string source)
		{
			if (source == null)
			{
				if (Reference.IsAssigned && string.IsNullOrEmpty(Reference.StoreName))
					inputs.Add(VariableDefinition.Create(Reference.RootName, VariableType.Empty));
			}
			else if (Reference.IsAssigned && Reference.StoreName == source)
			{
				inputs.Add(VariableDefinition.Create(Reference.RootName, VariableType.Empty));
			}
		}

		public override VariableValue Evaluate(IVariableStore variables)
		{
			var value = Reference.GetValue(variables);

			if (Parameter == null)
			{
				return value;
			}
			else
			{
				var lookup = Parameter.Evaluate(variables);

				if (lookup.Type == VariableType.String)
					return VariableReference.ResolveLookup(value, lookup.String);
				else if (lookup.Type == VariableType.Int)
					return VariableReference.ResolveLookup(value, lookup.Int);
				else
					return VariableValue.Empty;
			}
		}
	}
}
