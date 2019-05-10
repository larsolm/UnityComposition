using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[HelpURL(Composition.DocumentationUrl + "number-binding")]
	[AddComponentMenu("PiRho Soft/Bindings/Number Binding")]
	public class NumberBinding : StringBinding
	{
		public BindingFormatter Format;

		[Tooltip("The variable holding the value to display as text in this object")]
		public VariableReference Variable = new VariableReference();

		protected override void UpdateBinding(IVariableStore variables, BindingAnimationStatus status)
		{
			var enabled = false;
			var text = string.Empty;

			if (Resolve(variables, Variable, out int i))
			{
				enabled = true;
				text = Format.GetFormattedString(i);
			}
			else if (Resolve(variables, Variable, out float f))
			{
				enabled = true;
				text = Format.GetFormattedString(f);
			}

			SetText(text, enabled);
		}
	}
}
