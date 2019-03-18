using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[HelpURL(Composition.DocumentationUrl + "visible-binding")]
	[AddComponentMenu("PiRho Soft/Interface/Visible Binding")]
	public class VisibleBinding : VariableBinding
	{
		[Tooltip("The expression to run to determine if this object should be visible")]
		public Expression Condition = new Expression();

		protected override void UpdateBinding(IVariableStore variables, BindingAnimationStatus status)
		{
			var active = false;

			try
			{
				Condition.Evaluate(variables).TryGetBool(out active);
			}
			catch
			{
			}

			gameObject.SetActive(active);
		}
	}
}
