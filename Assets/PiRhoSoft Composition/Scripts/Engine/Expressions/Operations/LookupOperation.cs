using System.Collections.Generic;
using System.Text;

namespace PiRhoSoft.CompositionEngine
{
	public class LookupOperation : Operation
	{
		private const string _missingVariableException = "unable to find variable {0}";
		private const string _missingLookupException = "unable to find variable {0}[{1}]";
		private const string _invalidLookupException = "unable to lookup [{1}] on variable {0}";

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

		public override void GetInputs(IList<VariableDefinition> inputs, string source)
		{
			GetVariables(inputs, source);
			Parameter?.GetInputs(inputs, source);
		}

		public override void GetOutputs(IList<VariableDefinition> outputs, string source)
		{
			GetVariables(outputs, source);
		}

		private void GetVariables(IList<VariableDefinition> inputs, string source)
		{
			if (source == null)
			{
				if (Reference.IsAssigned && string.IsNullOrEmpty(Reference.StoreName))
					inputs.Add(new VariableDefinition { Name = Reference.RootName, Definition = ValueDefinition.Create(VariableType.Empty) });
			}
			else if (Reference.IsAssigned && Reference.StoreName == source)
			{
				inputs.Add(new VariableDefinition { Name = Reference.RootName, Definition = ValueDefinition.Create(VariableType.Empty) });
			}
		}

		public override VariableValue Evaluate(IVariableStore variables)
		{
			if (Parameter == null)
			{
				var value = Reference.GetValue(variables);

				if (value.IsEmpty)
					throw new ExpressionEvaluationException(_missingVariableException, Reference);
				else
					return value;
			}
			else
			{
				var owner = Reference.GetValue(variables);
				var lookup = Parameter.Evaluate(variables);

				if (lookup.Type == VariableType.String || lookup.Type == VariableType.Int)
				{
					var value = owner.Handler.Lookup(owner, lookup.ToString());

					if (value.IsEmpty)
						throw new ExpressionEvaluationException(_missingLookupException, Reference, Parameter);
					else
						return value;
				}
				else
				{
					throw new ExpressionEvaluationException(_invalidLookupException, Reference, Parameter);
				}
			}
		}

		public SetVariableResult SetValue(IVariableStore variables, VariableValue value)
		{
			if (Parameter == null)
			{
				return Reference.SetValue(variables, value);
			}
			else
			{
				var owner = Reference.GetValue(variables);
				var lookup = Parameter.Evaluate(variables);

				if (lookup.Type == VariableType.String || lookup.Type == VariableType.Int)
				{
					var result = owner.Handler.Apply(ref owner, lookup.ToString(), value);

					if (!owner.HasStore && result == SetVariableResult.Success)
						return Reference.SetValue(variables, owner);
					else
						return result;
				}
				else
				{
					return SetVariableResult.NotFound;
				}
			}
		}
	}
}
