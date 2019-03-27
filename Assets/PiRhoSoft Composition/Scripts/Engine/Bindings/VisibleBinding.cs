using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[HelpURL(Composition.DocumentationUrl + "visible-binding")]
	[AddComponentMenu("PiRho Soft/Interface/Visible Binding")]
	public class VisibleBinding : VariableBinding
	{
		[Tooltip("The renderer to show or hide based on Expression")]
		public Renderer Renderer;

		[Tooltip("The expression to run to determine if the refereneced component should be visible")]
		public Expression Condition = new Expression();

		protected override void UpdateBinding(IVariableStore variables, BindingAnimationStatus status)
		{
			if (Renderer)
			{
				var active = false;

				try
				{
					Condition.Evaluate(variables).TryGetBool(out active);
				}
				catch
				{
				}

				Renderer.enabled = active;
			}
		}
	}
}
