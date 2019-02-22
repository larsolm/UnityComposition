using System.Collections.Generic;

namespace PiRhoSoft.CompositionEngine
{
	public class ExpressionCommand : Command
	{
		public class ParameterStore : IVariableStore
		{
			public const string ParameterName = "P";
			public List<VariableValue> Parameters = new List<VariableValue>(10);

			public VariableValue GetVariable(string name)
			{
				if (name.StartsWith(ParameterName))
				{
					var index = 0;
					for (var i = ParameterName.Length; i < name.Length; i++)
						index = index * 10 + (name[i] - '0');

					if (index >= 0 && index < Parameters.Count)
						return Parameters[index];
				}

				return VariableValue.Empty;
			}

			public SetVariableResult SetVariable(string name, VariableValue value)
			{
				return SetVariableResult.ReadOnly;
			}
		}

		public static ParameterStore Store = new ParameterStore();

		public Expression Expression { get; private set; }

		public ExpressionCommand(Expression expression) => Expression = expression;

		public override VariableValue Evaluate(IVariableStore variables, string name, List<Operation> parameters)
		{
			Store.Parameters.Clear();

			foreach (var parameter in parameters)
			{
				var value = parameter.Evaluate(variables);
				Store.Parameters.Add(value);
			}

			return Expression.Evaluate(Store);
		}
	}
}
