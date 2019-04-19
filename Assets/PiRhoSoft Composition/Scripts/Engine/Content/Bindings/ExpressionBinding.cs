using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[HelpURL(Composition.DocumentationUrl + "expression-binding")]
	[AddComponentMenu("PiRho Soft/Bindings/Expression Binding")]
	public class ExpressionBinding : StringBinding
	{
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

				enabled = result.Type != VariableType.Empty;

				if (result.TryGetInt(out var intValue))
					text = Formatting.GetFormattedString(intValue);
				if (result.TryGetFloat(out var floatValue))
					text = Formatting.GetFormattedString(floatValue);
				else
					text = result.ToString();
			}
			catch
			{
			}

			SetText(text, enabled);
		}
	}
}
