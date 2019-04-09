﻿using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PiRhoSoft.CompositionEngine
{
	public class ExpressionDisplayAttribute : PropertyAttribute
	{
		public bool Foldout = false;
		public bool FullWidth = true;
		public int MinimumLines = 2;
		public int MaximumLines = 8;
	}

	public class ExpressionCompilationResult
	{
		public bool Success = true;
		public int Location;
		public string Token;
		public string Message;
	}

	[Serializable]
	public class Expression : ISerializationCallbackReceiver
	{
		private const string _expressionTokenizeError = "(CETE) Failed to parse Expression at location {1}: {2}\nExpression: {0}";
		private const string _expressionParseError = "(CEPE) Failed to parse Expression at location {1} ({2}): {3}\nExpression: {0}";
		private const string _expressionEvaluationError = "(CEEE) Failed to execute Expression '{0}': {1}";
		private const string _commandEvaluationError = "(CCEE) Failed to execute Command '{0}': {1}";
		private const string _invalidResultWarning = "(CEIR) The Expression '{0}' was expected to return type {1} but instead returned type {2}";

		[SerializeField] private string _statement;
		[NonSerialized] private List<Operation> _operations;
		[NonSerialized] private Operation _currentOperation;

		public ExpressionCompilationResult CompilationResult { get; private set; } = new ExpressionCompilationResult();
		public bool IsValid => _operations != null && _operations.Count > 0;
		public bool HasError => !string.IsNullOrEmpty(_statement) && _operations == null;
		public string Statement => _statement;

		public ExpressionCompilationResult SetStatement(string statement)
		{
			_statement = statement;
			return Compile();
		}

		public void GetInputs(IList<VariableDefinition> inputs, string source)
		{
			if (_operations != null)
			{
				foreach (var operation in _operations)
					operation.GetInputs(inputs, source);
			}
		}

		public void GetOutputs(IList<VariableDefinition> outputs, string source)
		{
			if (_operations != null)
			{
				foreach (var operation in _operations)
					operation.GetOutputs(outputs, source);
			}
		}

		public VariableValue Execute(Object context, IVariableStore variables)
		{
			try
			{
				return Evaluate(variables);
			}
			catch (ExpressionEvaluationException exception)
			{
				Debug.LogErrorFormat(context, _expressionEvaluationError, _currentOperation.ToString(), exception.Message);
			}
			catch (CommandEvaluationException exception)
			{
				Debug.LogErrorFormat(context, _commandEvaluationError, exception.Command, exception.Message);
			}

			return VariableValue.Empty;
		}

		public VariableValue Execute(Object context, IVariableStore variables, VariableType expectedType)
		{
			var result = Execute(context, variables);

			if (result.Type != expectedType && _currentOperation == null) // _currentOperation will not be null if there was an exception in which case an error was already logged
			{
				var statement = _operations != null && _operations.Count > 0 ? _operations[_operations.Count - 1].ToString() : string.Empty;
				Debug.LogWarningFormat(context, _invalidResultWarning, statement, expectedType, result.Type);
			}

			return result;
		}

		public VariableValue Evaluate(IVariableStore variables)
		{
			var result = VariableValue.Empty;

			if (_operations != null)
			{
				foreach (var operation in _operations)
				{
					_currentOperation = operation;
					result = operation.Evaluate(variables);
					_currentOperation = null;
				}
			}

			return result; // the result from the last operation is returned which is almost certainly the desired behavior
		}

		private ExpressionCompilationResult Compile()
		{
			_operations = null;

			if (!string.IsNullOrEmpty(_statement))
			{
				try
				{
					var tokens = ExpressionLexer.Tokenize(_statement);
					_operations = ExpressionParser.Parse(_statement, tokens);

					CompilationResult.Success = true;
					CompilationResult.Location = 0;
					CompilationResult.Token = null;
					CompilationResult.Message = null;
				}
				catch (ExpressionParseException exception)
				{
					CompilationResult.Success = false;
					CompilationResult.Location = exception.Token.Location;
					CompilationResult.Token = _statement.Substring(exception.Token.Start, exception.Token.End - exception.Token.Start);
					CompilationResult.Message = exception.Message + exception.Message + exception.Message + exception.Message + exception.Message + exception.Message + exception.Message + exception.Message + exception.Message + exception.Message;
				}
			}

			return CompilationResult;
		}

		public void OnBeforeSerialize()
		{
		}

		public void OnAfterDeserialize()
		{
			var error = Compile();

			if (!error.Success)
				string.Format(_expressionParseError, _statement, error.Location, error.Token, error.Message);
		}
	}
}
