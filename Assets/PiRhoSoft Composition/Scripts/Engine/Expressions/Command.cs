using PiRhoSoft.UtilityEngine;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	public class CommandEvaluationException : Exception
	{
		public string Command;

		public CommandEvaluationException(string command, string error) : base(error) => Command = command;
		public CommandEvaluationException(string command, string errorFormat, params object[] arguments) : this(command, string.Format(errorFormat, arguments)) { }
	}

	public interface ICommand
	{
		VariableValue Evaluate(IVariableStore variables, string name, List<Operation> parameters);
	}

	[HelpURL(Composition.DocumentationUrl + "variable-command")]
	[CreateAssetMenu(menuName = "PiRho Soft/Command", fileName = "Command", order = 119)]
	public class Command : ScriptableObject, ICommand, IReloadable
	{
		[Serializable]
		public class Parameter
		{
			public string Name;
			public VariableType Type;
		}

		[Serializable] public class ParameterList : SerializedList<Parameter> { }

		public const string WrongParameterCountException = "the Command was passed {0} parameter{1} but expected {2}";
		public const string WrongParameterRangeException = "the Command was passed {0} parameter{1} but expected between {2} and {3}";
		public const string TooFewParametersException = "the Command was passed {0} parameter{1} but expected at least {2}";
		public const string TooManyParametersException = "the Command was passed {0} parameter{1} but expected at most {2}";
		public const string WrongParameterType1Exception = "the Command was passed a {0} for parameter {1} but expected a {2}";
		public const string WrongParameterType2Exception = "the Command was passed a {0} for parameter {1} but expected a {2} or {3}";
		public const string InvalidRangeException = "the min value ({0}) should be less than the max value ({1})";

		[ReloadOnChange] public string Name;
		[ListDisplay(ItemDisplay = ListItemDisplayType.Inline)] public ParameterList Parameters = new ParameterList();
		public Expression Expression = new Expression();

		private string _registeredName;

		public void OnEnable()
		{
			if (!string.IsNullOrEmpty(Name))
			{
				_registeredName = Name;
				ExpressionParser.AddCommand(_registeredName, this);
			}
		}

		public void OnDisable()
		{
			if (!string.IsNullOrEmpty(_registeredName))
			{
				ExpressionParser.RemoveCommand(_registeredName);
				_registeredName = null;
			}
		}

		public VariableValue Evaluate(IVariableStore variables, string name, List<Operation> parameters)
		{
			var store = ReserveStore();

			if (Parameters.Count != parameters.Count)
				throw new CommandEvaluationException(Name, WrongParameterCountException, parameters.Count, parameters.Count == 1 ? "" : "s", Parameters.Count);

			for (var i = 0; i < parameters.Count; i++)
			{
				var parameter = Parameters[i];
				var value = parameters[i].Evaluate(variables);

				if (parameter.Type != VariableType.Empty && parameter.Type != value.Type)
					throw new CommandEvaluationException(Name, WrongParameterType1Exception, value.Type, i + 1, parameter.Type);

				store.AddVariable(Parameters[i].Name, value);
			}

			var result = Expression.Evaluate(store);
			ReleaseStore(store);
			return result;
		}

		#region Parameters

		public const int InitialStoreCount = 5; // multiple because commands can call other commands
		public static Stack<VariableStore> Stores = new Stack<VariableStore>(InitialStoreCount);

		static Command()
		{
			for (var i = 0; i < InitialStoreCount; i++)
				Stores.Push(new VariableStore());
		}

		public static VariableStore ReserveStore()
		{
			if (Stores.Count == 0)
				Stores.Push(new VariableStore());

			return Stores.Pop();
		}

		public static void ReleaseStore(VariableStore store)
		{
			Stores.Push(store);
			store.Clear();
		}

		#endregion
	}
}
