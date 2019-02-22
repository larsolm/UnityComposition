using System;
using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	public struct ExpressionCompilationResult
	{
		public bool Success;
		public string Message;
	}

	[Serializable]
	public class Expression : ISerializationCallbackReceiver
	{
		private const string _expressionTokenizeError = "(CETE) Failed to parse Expression '{0} at location {1}': {2}";
		private const string _expressionParseError = "(CEPE) Failed to parse Expression '{0}' at location {1}: {2}";
		private const string _expressionEvaluationError = "(CEEE) Failed to execute Expression '{0}': {1}";
		private const string _commandEvaluationError = "(CCEE) Failed to execute Command '{0}': {1}";
		private const string _invalidResultWarning = "(CEIR) The Expression '{0}' was expected to return type {1} but instead returned type {2}";

		[SerializeField] private string _statement;
		private List<Operation> _operations;

		public bool IsValid => _operations != null && _operations.Count > 0;
		public bool HasError => !string.IsNullOrEmpty(_statement) && _operations == null;
		public string Statement => _statement;

		public ExpressionCompilationResult SetStatement(string statement)
		{
			_statement = statement;
			return Compile();
		}

		public void GetInputs(List<VariableDefinition> inputs, string source)
		{
			if (_operations != null)
			{
				foreach (var operation in _operations)
					operation.GetInputs(inputs, source);
			}
		}

		public void GetOutputs(List<VariableDefinition> outputs, string source)
		{
			if (_operations != null)
			{
				foreach (var operation in _operations)
					operation.GetOutputs(outputs, source);
			}
		}

		public VariableValue Execute(IVariableStore variables)
		{
			try
			{
				return Evaluate(variables);
			}
			catch (ExpressionEvaluationException exception)
			{
				Debug.LogErrorFormat(_expressionEvaluationError, _statement, exception.Message);
			}
			catch (CommandEvaluationException exception)
			{
				Debug.LogErrorFormat(_commandEvaluationError, exception.Command, exception.Message);
			}

			return VariableValue.Empty;
		}

		public VariableValue Execute(IVariableStore variables, VariableType expectedType)
		{
			var result = Execute(variables);

			if (result.Type != expectedType) // Empty might mean there was an exception so logging would be duplicated, but that kind of makes sense in this case
				Debug.LogWarningFormat(_invalidResultWarning, _statement, expectedType, result.Type);

			return result;
		}

		public VariableValue Evaluate(IVariableStore variables)
		{
			var result = VariableValue.Empty;

			if (_operations != null)
			{
				foreach (var operation in _operations)
					result = operation.Evaluate(variables);
			}

			return result; // the result from the last operation is returned which is almost certainly the desired behavior
		}

		private ExpressionCompilationResult Compile()
		{
			_operations = null;
			var result = new ExpressionCompilationResult { Success = true };

			if (!string.IsNullOrEmpty(_statement))
			{
				try
				{
					var tokens = ExpressionLexer.Tokenize(_statement);
					_operations = ExpressionParser.Parse(tokens);
				}
				catch (ExpressionTokenizeException exception)
				{
					result.Success = false;
					result.Message = string.Format(_expressionTokenizeError, _statement, exception.Location, exception.Message);
				}
				catch (ExpressionParseException exception)
				{
					result.Success = false;
					result.Message = string.Format(_expressionParseError, _statement, exception.Location, exception.Message);
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
				Debug.LogError(error.Message);
		}

		#region Editor Support
#if UNITY_EDITOR
		[NonSerialized] public bool IsExpanded = false;
#endif
		#endregion
	}
}
