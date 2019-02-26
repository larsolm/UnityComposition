﻿using System.Collections.Generic;

namespace PiRhoSoft.CompositionEngine
{
	public class ExpressionCommand : Command
	{
		public class ParameterStore : IVariableStore
		{
			public const string ParameterName = "P";
			public List<VariableValue> Parameters = new List<VariableValue>(5);

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

		public const int InitialStoreCount = 5;

		public static Stack<ParameterStore> Stores = new Stack<ParameterStore>(InitialStoreCount);

		static ExpressionCommand()
		{
			for (var i = 0; i < InitialStoreCount; i++)
				Stores.Push(new ParameterStore());
		}

		public static ParameterStore ReserveStore()
		{
			if (Stores.Count == 0)
				Stores.Push(new ParameterStore());

			return Stores.Pop();
		}

		public static void ReleaseStore(ParameterStore store)
		{
			Stores.Push(store);
			store.Parameters.Clear();
		}

		public Expression Expression { get; private set; }

		public ExpressionCommand(Expression expression) => Expression = expression;

		public override VariableValue Evaluate(IVariableStore variables, string name, List<Operation> parameters)
		{
			var store = ReserveStore();

			foreach (var parameter in parameters)
			{
				var value = parameter.Evaluate(variables);
				store.Parameters.Add(value);
			}

			var result = Expression.Evaluate(store);
			ReleaseStore(store);
			return result;
		}
	}
}
