using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[HelpURL(Composition.DocumentationUrl + "expression-binding")]
	[AddComponentMenu("PiRho Soft/Bindings/Expression Binding")]
	public class ExpressionBinding : StringBinding
	{
		private const string _missingExpressionWarning = "(CEXBMV) Unable to bind text for binding '{0}': the expression is empty";
		private const string _failedExpressionWarning = "(CEXBFE) Unable to bind text for binding '{0}': the expression '{1}' failed with error '{2}'";
		private const string _failedCommandWarning = "(CEXBFC) Unable to bind text for binding '{0}': the Command '{1}' failed with error '{2}'";

		public BindingFormatter Formatting;

		[Tooltip("The expression to evaluate and display as text in this object")]
		public Expression Expression;

		protected override void UpdateBinding(IVariableStore variables, BindingAnimationStatus status)
		{
			var enabled = false;
			var text = string.Empty;

			try
			{
				var result = Expression.Evaluate(variables);

				if (result.IsEmpty)
				{
					if (!SuppressErrors)
						Debug.LogWarningFormat(this, _missingExpressionWarning, this);
				}
				else
				{
					enabled = true;

					if (result.TryGetInt(out var intValue))
						text = Formatting.GetFormattedString(intValue);
					if (result.TryGetFloat(out var floatValue))
						text = Formatting.GetFormattedString(floatValue);
					else
						text = result.ToString();
				}
			}
			catch (ExpressionEvaluationException exception)
			{
				if (!SuppressErrors)
					Debug.LogWarningFormat(this, _failedExpressionWarning, this, Expression.LastOperation, exception.Message);
			}
			catch (CommandEvaluationException exception)
			{
				if (!SuppressErrors)
					Debug.LogWarningFormat(this, _failedCommandWarning, this, exception.Command, exception.Message);
			}

			SetText(text, enabled);
		}
	}
}
