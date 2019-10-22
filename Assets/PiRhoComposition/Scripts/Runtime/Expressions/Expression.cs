﻿using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PiRhoSoft.Composition
{
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
		public const string StatementField = nameof(_statement);

		private const string _expressionParseError = "(CEPE) Failed to parse Expression at location {1} ({2}): {3}\nExpression: {0}";
		private const string _expressionEvaluationError = "(CEEE) Failed to execute Expression '{0}' on '{1}': {2}";
		private const string _commandEvaluationError = "(CCEE) Failed to execute Command '{0}' on '{1}': {2}";
		private const string _invalidResultWarning = "(CEIR) '{0}' expected the Expression '{1}' to return type '{2}' but it instead returned type '{3}'";

		[SerializeField] private string _statement = string.Empty;
		[NonSerialized] private List<Operation> _operations;
		[NonSerialized] private Operation _currentOperation;

		public Operation LastOperation => _currentOperation;
		public bool IsValid => _operations != null && _operations.Count > 0;
		public bool HasError => !string.IsNullOrEmpty(_statement) && _operations == null;
		public string Statement => _statement;

		public override bool Equals(object obj)
		{
			return obj is Expression other
				&& ((string.IsNullOrEmpty(_statement) && string.IsNullOrEmpty(other.Statement)) || (_statement != null && _statement.Equals(other._statement)));
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public ExpressionCompilationResult SetStatement(string statement)
		{
			_statement = statement;
			return Compile();
		}

		public void GetInputs(VariableDefinitionList inputs, string source)
		{
			if (_operations != null)
			{
				foreach (var operation in _operations)
					operation.GetInputs(inputs, source);
			}
		}

		public void GetOutputs(VariableDefinitionList outputs, string source)
		{
			if (_operations != null)
			{
				foreach (var operation in _operations)
					operation.GetOutputs(outputs, source);
			}
		}

		public Variable Execute(Object context, IVariableMap variables)
		{
			try
			{
				return Evaluate(variables, true);
			}
			catch (ExpressionEvaluationException exception)
			{
				Debug.LogErrorFormat(context, _expressionEvaluationError, _currentOperation, context.name, exception.Message);
			}
			catch (CommandEvaluationException exception)
			{
				Debug.LogErrorFormat(context, _commandEvaluationError, exception.Command, context.name, exception.Message);
			}

			return Variable.Empty;
		}

		public Variable Execute(Object context, IVariableMap variables, VariableType expectedType)
		{
			var result = Execute(context, variables);

			if (result.Type != expectedType)
			{
				var statement = _operations != null && _operations.Count > 0 ? _operations[_operations.Count - 1].ToString() : string.Empty;
				Debug.LogWarningFormat(context, _invalidResultWarning, context, statement, expectedType, result.Type);
			}

			return result;
		}

		public Variable Evaluate(IVariableMap variables, bool isolateVariables)
		{
			var result = Variable.Empty;

			if (_operations != null)
			{
				if (isolateVariables)
					variables = new IsolatedVariableCollection(variables); // TODO: pool these

				foreach (var operation in _operations)
				{
					_currentOperation = operation;
					result = operation.Evaluate(variables);
				}
			}

			return result; // the result from the last operation is returned which is almost certainly the desired behavior
		}

		private ExpressionCompilationResult Compile()
		{
			_operations = null;
			var result = new ExpressionCompilationResult();

			if (!string.IsNullOrEmpty(_statement))
			{
				try
				{
					var tokens = ExpressionLexer.Tokenize(_statement);
					_operations = ExpressionParser.Parse(_statement, tokens);
				}
				catch (ExpressionParseException exception)
				{
					result.Success = false;
					result.Location = exception.Token.Location;
					result.Token = _statement.Substring(exception.Token.Start, exception.Token.End - exception.Token.Start);
					result.Message = exception.Message;
				}
			}

			return result;
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