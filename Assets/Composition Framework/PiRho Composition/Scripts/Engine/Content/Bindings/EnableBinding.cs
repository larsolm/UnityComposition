using UnityEngine;

namespace PiRhoSoft.Composition
{
	[HelpURL(Composition.DocumentationUrl + "enable-binding")]
	[AddComponentMenu("PiRho Soft/Bindings/Enable Binding")]
	public class EnableBinding : VariableBinding
	{
		private const string _missingExpressionWarning = "(CEBMV) Unable to bind enabled state for binding '{0}': the expression is empty";
		private const string _invalidExpressionWarning = "(CEBIV) Unable to bind enabled state for binding '{0}': the expression '{1}' did not evaluate to a bool";
		private const string _failedExpressionWarning = "(CEBFE) Unable to bind enabled state for binding '{0}': the expression '{1}' failed with error '{2}'";
		private const string _failedCommandWarning = "(CEBFC) Unable to bind enabled state for binding '{0}': the Command '{1}' failed with error '{2}'";
		private const string _invalidObjectWarning = "(CEBIO) Unable to bind enabled state for binding '{0)': the object '{1}' is not a GameObject, Behaviour, or Renderer";

		[Tooltip("The GameObject, Behaviour, or Renderer to enable or disable based on Condition")]
		public Object Object;

		[Tooltip("The expression to run to determine if the refereneced component should be enabled")]
		public Expression Condition = new Expression();

		protected override void UpdateBinding(IVariableStore variables, BindingAnimationStatus status)
		{
			if (Object)
			{
				var enabled = false;

				try
				{
					var value = Condition.Evaluate(variables, true);

					if (value.Type == VariableType.Bool)
					{
						enabled = value.AsBool;
					}
					else if (!SuppressErrors)
					{
						if (value.IsEmpty)
							Debug.LogWarningFormat(this, _missingExpressionWarning, this);
						else
							Debug.LogWarningFormat(this, _invalidExpressionWarning, this, Condition.LastOperation);
					}
				}
				catch (ExpressionEvaluationException exception)
				{
					Debug.LogWarningFormat(this, _failedExpressionWarning, this, Condition.LastOperation, exception.Message);
				}
				catch (CommandEvaluationException exception)
				{
					Debug.LogWarningFormat(this, _failedCommandWarning, this, exception.Command, exception.Message);
				}

				if (Object is GameObject gameObject)
					gameObject.SetActive(enabled);
				else if (Object is Behaviour behaviour)
					behaviour.enabled = enabled;
				else if (Object is Renderer renderer)
					renderer.enabled = enabled;
				else
					Debug.LogWarningFormat(this, _invalidObjectWarning, this, Object);
			}
		}
	}
}
