using UnityEngine;

namespace PiRhoSoft.Composition
{
	[HelpURL(Configuration.DocumentationUrl + "expression-binding")]
	[AddComponentMenu("PiRho Soft/Bindings/Expression Binding")]
	public class ExpressionBinding : VariableBinding
	{
		private const string _missingExpressionWarning = "(CEXBMV) Unable to bind text for binding '{0}': the expression is empty";
		private const string _failedExpressionWarning = "(CEXBFE) Unable to bind text for binding '{0}': the expression '{1}' failed with error '{2}'";
		private const string _failedCommandWarning = "(CEXBFC) Unable to bind text for binding '{0}': the Command '{1}' failed with error '{2}'";

		[Tooltip("The expression to evaluate and assign to the binding")]
		public Expression Expression;

		private enum ResultState
		{
			New,
			Error,
			Value
		}

		private ResultState _resultState = ResultState.New;
		private Variable _resultValue = Variable.Empty;

		protected override void UpdateBinding(IVariableCollection variables, BindingAnimationStatus status)
		{
			try
			{
				var result = Expression.Evaluate(variables, true);
				var equal = VariableHandler.IsEqual(result, _resultValue);

				if (_resultState != ResultState.Value || !equal.HasValue || !equal.Value)
				{
					if (result.IsEmpty)
					{
						if (ErrorType == BindingErrorType.Log)
							Debug.LogWarningFormat(this, _missingExpressionWarning, this);
					}
					else
					{
						SetBinding(result, true);
					}
				}

				_resultValue = result;
			}
			catch (ExpressionEvaluationException exception)
			{
				if (_resultState != ResultState.Error)
					Debug.LogWarningFormat(this, _failedExpressionWarning, this, Expression.LastOperation, exception.Message);

				_resultState = ResultState.Error;
			}
			catch (CommandEvaluationException exception)
			{
				if (_resultState != ResultState.Error)
					Debug.LogWarningFormat(this, _failedCommandWarning, this, exception.Command, exception.Message);

				_resultState = ResultState.Error;
			}
		}
	}
}
