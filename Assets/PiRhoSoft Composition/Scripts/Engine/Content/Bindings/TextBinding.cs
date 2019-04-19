using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[HelpURL(Composition.DocumentationUrl + "text-binding")]
	[AddComponentMenu("PiRho Soft/Bindings/Text Binding")]
	public class TextBinding : StringBinding
	{
		private const string _missingVariableWarning = "(CBTBMV) Unable to bind text for text binding '{0}': the variable '{1}' could not be found";

		[Tooltip("The variable holding the value to display as text in this object")]
		public VariableReference Variable = new VariableReference();

		protected override void UpdateBinding(IVariableStore variables, BindingAnimationStatus status)
		{
			var value = Variable.GetValue(variables);

			SetText(value.ToString(), value.Type != VariableType.Empty);

			if (!SuppressErrors && value.IsEmpty)
				Debug.LogWarningFormat(this, _missingVariableWarning, name, Variable);
		}
	}
}
