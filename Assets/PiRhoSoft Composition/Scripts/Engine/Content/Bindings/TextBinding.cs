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
			var resolved = Resolve(variables, Variable, out VariableValue value);
			SetText(value.ToString(), resolved && value.Type != VariableType.Empty);
		}
	}
}
