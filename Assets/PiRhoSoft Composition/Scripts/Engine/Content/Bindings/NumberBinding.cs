using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[HelpURL(Composition.DocumentationUrl + "number-binding")]
	[AddComponentMenu("PiRho Soft/Bindings/Number Binding")]
	public class NumberBinding : StringBinding
	{
		private const string _missingVariableWarning = "(CBNBMV) Unable to bind text for number binding '{0}': the variable '{1}' could not be found";
		private const string _invalidVariableWarning = "(CBNBIV) Unable to bind text for number binding '{0}': the variable '{1}' is not an int or a float";

		public BindingFormatter Format;

		[Tooltip("The variable holding the value to display as text in this object")]
		public VariableReference Variable = new VariableReference();

		protected override void UpdateBinding(IVariableStore variables, BindingAnimationStatus status)
		{
			var value = Variable.GetValue(variables);
			var enabled = value.HasNumber;
			var text = string.Empty;

			if (value.TryGetInt(out var intValue))
				text = Format.GetFormattedString(value.Int);
			else if (value.TryGetFloat(out var floatValue))
				text = Format.GetFormattedString(value.Float);
			else if (!SuppressErrors)
				Debug.LogWarningFormat(this, value.IsEmpty ? _missingVariableWarning : _invalidVariableWarning, name, Variable);

			SetText(text, enabled);
		}
	}
}
