using TMPro;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(TMP_Text))]
	[HelpURL(Composition.DocumentationUrl + "text-color-binding")]
	[AddComponentMenu("PiRho Soft/Bindings/Text Color Binding")]
	public class TextColorBinding : VariableBinding
	{
		private const string _missingVariableWarning = "(CBTCBMV) Unable to bind color for text color binding '{0}': the variable '{1}' could not be found";
		private const string _invalidVariableWarning = "(CBTCBIV) Unable to bind color for text color binding '{0}': the variable '{1}' is not a color";

		[Tooltip("The variable holding the color value to use for the text")]
		public VariableReference Variable = new VariableReference();

		private TMP_Text _text;

		public TMP_Text Text
		{
			get
			{
				// can't look up in awake because it's possible to update bindings before the component is enabled

				if (!_text)
					_text = GetComponent<TMP_Text>();

				return _text;
			}
		}

		protected override void UpdateBinding(IVariableStore variables, BindingAnimationStatus status)
		{
			var value = Variable.GetValue(variables);

			Text.enabled = value.Type == VariableType.Color;
			Text.color = value.Color;

			if (!SuppressErrors && value.Type != VariableType.Color)
				Debug.LogWarningFormat(this, value.IsEmpty ? _missingVariableWarning : _invalidVariableWarning, name, Variable);
		}
	}
}
