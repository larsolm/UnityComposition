using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[HelpURL(Composition.DocumentationUrl + "text-binding")]
	[AddComponentMenu("PiRho Soft/Bindings/Text Binding")]
	public class TextBinding : StringBinding
	{
		[Tooltip("The variable holding the value to display as text in this object")]
		public VariableReference Variable = new VariableReference();

		protected override void UpdateBinding(IVariableStore variables, BindingAnimationStatus status)
		{
			var value = Variable.GetValue(variables);

			SetText(value.ToString(), value.Type != VariableType.Empty);
		}
	}
}
