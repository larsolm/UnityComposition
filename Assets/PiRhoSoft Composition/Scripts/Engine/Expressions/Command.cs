using PiRhoSoft.UtilityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	public class CommandEvaluationException : Exception
	{
		public string Command;

		public CommandEvaluationException(string command, string error) : base(error) => Command = command;
		public CommandEvaluationException(string command, string errorFormat, params object[] arguments) : this(command, string.Format(errorFormat, arguments)) { }

		#region Errors

		private const string _wrongParameterCount1Exception = "the Command was passed {0} parameter but expected {1}";
		private const string _wrongParameterCount2Exception = "the Command was passed {0} parameter but expected either {1} or {2}";
		private const string _wrongParameterRangeException = "the Command was passed {0} parameter but expected between {1} and {2}";
		private const string _tooFewParametersException = "the Command was passed {0} parameter but expected at least {1}";
		private const string _tooManyParametersException = "the Command was passed {0} parameter but expected at most {1}";
		private const string _wrongParameterCount1PluralException = "the Command was passed {0} parameters but expected {1}";
		private const string _wrongParameterCount2PluralException = "the Command was passed {0} parameters but expected either {1} or {2}";
		private const string _wrongParameterRangePluralException = "the Command was passed {0} parameters but expected between {1} and {2}";
		private const string _tooFewParametersPluralException = "the Command was passed {0} parameters but expected at least {1}";
		private const string _tooManyParametersPluralException = "the Command was passed {0} parameters but expected at most {1}";
		private const string _wrongParameterType1Exception = "the Command was passed a value of type {0} for parameter {1} but expected a value of type {2}";
		private const string _wrongParameterType2Exception = "the Command was passed a value of type {0} for parameter {1} but expected a value of type {2} or {3}";
		private const string _wrongParameterTypeXException = "the Command was passed a value of type {0} for parameter {1} but expected a value of type {2}, or {3}";

		public static CommandEvaluationException WrongParameterCount(string commandName, int got, int expected)
		{
			if (got == 1)
				return new CommandEvaluationException(commandName, _wrongParameterCount1Exception, got, expected);
			else
				return new CommandEvaluationException(commandName, _wrongParameterCount1PluralException, got, expected);
		}

		public static CommandEvaluationException WrongParameterCount(string commandName, int got, int expected1, int expected2)
		{
			if (got == 1)
				return new CommandEvaluationException(commandName, _wrongParameterCount2Exception, got, expected1, expected2);
			else
				return new CommandEvaluationException(commandName, _wrongParameterCount2PluralException, got, expected1, expected2);
		}

		public static CommandEvaluationException WrongParameterRange(string commandName, int got, int expectedMinimum, int expectedMaximum)
		{
			if (got == 1)
				return new CommandEvaluationException(commandName, _wrongParameterRangeException, got, expectedMinimum, expectedMaximum);
			else
				return new CommandEvaluationException(commandName, _wrongParameterRangePluralException, got, expectedMinimum, expectedMaximum);
		}

		public static CommandEvaluationException TooFewParameters(string commandName, int got, int expected)
		{
			if (got == 1)
				return new CommandEvaluationException(commandName, _tooFewParametersException, got, expected);
			else
				return new CommandEvaluationException(commandName, _tooFewParametersPluralException, got, expected);
		}

		public static CommandEvaluationException TooManyParameters(string commandName, int got, int expected)
		{
			if (got == 1)
				return new CommandEvaluationException(commandName, _tooManyParametersException, got, expected);
			else
				return new CommandEvaluationException(commandName, _tooManyParametersPluralException, got, expected);
		}

		public static CommandEvaluationException WrongParameterType(string commandName, int index, VariableType got, VariableType expected)
		{
			return new CommandEvaluationException(commandName, _wrongParameterType1Exception, got, index + 1, expected);
		}

		public static CommandEvaluationException WrongParameterType(string commandName, int index, VariableType got, VariableType expected1, VariableType expected2)
		{
			return new CommandEvaluationException(commandName, _wrongParameterType2Exception, got, index + 1, expected1, expected2);
		}

		public static CommandEvaluationException WrongParameterType(string commandName, int index, VariableType got, params VariableType[] expected)
		{
			var first = string.Join(",", expected.Take(expected.Length - 1));
			var last = expected[expected.Length - 1];

			return new CommandEvaluationException(commandName, _wrongParameterTypeXException, got, index + 1, first, last);
		}

		#endregion
	}

	public interface ICommand
	{
		VariableValue Evaluate(IVariableStore variables, string name, List<Operation> parameters);
	}

	[HelpURL(Composition.DocumentationUrl + "command")]
	[CreateAssetMenu(menuName = "PiRho Soft/Command", fileName = nameof(Command), order = 119)]
	public class Command : ScriptableObject, ICommand
	{
		[Serializable]
		public class Parameter
		{
			public string Name;
			public VariableType Type;
		}

		[Serializable] public class ParameterList : SerializedList<Parameter> { }

		[ChangeTrigger(nameof(OnNameChanged))][Delayed] public string Name;
		[ListDisplay] [ClassDisplay(ClassDisplayType.Inline)] public ParameterList Parameters = new ParameterList();
		[ExpressionDisplay(MinimumLines = 5, MaximumLines = 20)] public Expression Expression = new Expression();

		private string _registeredName;

		void OnEnable()
		{
			Register();
		}

		void OnDisable()
		{
			Unregister();
		}

		private void OnNameChanged()
		{
			Unregister();
			Register();
		}

		private void Register()
		{
			if (!string.IsNullOrEmpty(Name))
				ExpressionParser.AddCommand(Name, this);

			_registeredName = Name;
		}

		private void Unregister()
		{
			if (!string.IsNullOrEmpty(_registeredName))
				ExpressionParser.RemoveCommand(_registeredName);
		}

		public VariableValue Evaluate(IVariableStore variables, string name, List<Operation> parameters)
		{
			var store = ReserveStore();

			if (Parameters.Count != parameters.Count)
				throw CommandEvaluationException.WrongParameterCount(name, parameters.Count, Parameters.Count);

			for (var i = 0; i < parameters.Count; i++)
			{
				var parameter = Parameters[i];
				var value = parameters[i].Evaluate(variables);

				if (parameter.Type != VariableType.Empty && parameter.Type != value.Type)
					throw CommandEvaluationException.WrongParameterType(name, i, value.Type, parameter.Type);

				store.AddVariable(Parameters[i].Name, value);
			}

			var result = Expression.Evaluate(store);
			ReleaseStore(store);
			return result;
		}

		protected CommandEvaluationException WrongParameterType(int index, VariableType got, VariableType expected) => CommandEvaluationException.WrongParameterType(Name, index, got, expected);
		protected CommandEvaluationException WrongParameterType(int index, VariableType got, VariableType expected1, VariableType expected2) => CommandEvaluationException.WrongParameterType(Name, index, got, expected1, expected2);
		protected CommandEvaluationException WrongParameterType(int index, VariableType got, params VariableType[] expected) => CommandEvaluationException.WrongParameterType(Name, index, got, expected);

		#region Parameters

		private const int _initialStoreCount = 5; // multiple because commands can call other commands
		private static Stack<VariableStore> _stores = new Stack<VariableStore>(_initialStoreCount);

		static Command()
		{
			for (var i = 0; i < _initialStoreCount; i++)
				_stores.Push(new VariableStore());
		}

		private static VariableStore ReserveStore()
		{
			if (_stores.Count == 0)
				_stores.Push(new VariableStore());

			return _stores.Pop();
		}

		private static void ReleaseStore(VariableStore store)
		{
			_stores.Push(store);
			store.Clear();
		}

		#endregion
	}
}
