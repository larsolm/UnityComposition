using System.Collections.Generic;
using System.Text;

namespace PiRhoSoft.CompositionEngine
{
	public class CommandOperation : Operation
	{
		private const string _missingCommandException = "the Command '{0}' could not be found";

		public string Name { get; private set; }
		public List<Operation> Parameters { get; private set; }

		public CommandOperation(string name, List<Operation> parameters)
		{
			Name = name;
			Parameters = parameters;
		}

		public override void ToString(StringBuilder builder)
		{
			builder.Append(Name);
			builder.Append('(');

			for (var i = 0; i < Parameters.Count; i++)
			{
				if (i != 0)
					builder.Append(", ");

				Parameters[i].ToString(builder);
			}

			builder.Append(')');
		}

		public override void GetInputs(IList<VariableDefinition> inputs, string source)
		{
			foreach (var parameter in Parameters)
				parameter.GetInputs(inputs, source);
		}

		public override VariableValue Evaluate(IVariableStore variables)
		{
			var command = ExpressionParser.GetCommand(Name);

			if (command != null)
				return command.Evaluate(variables, Name, Parameters);
			else
				throw new ExpressionEvaluationException(_missingCommandException, Name);
		}
	}
}
