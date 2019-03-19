using TMPro;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(TextMeshProUGUI))]
	[HelpURL(Composition.DocumentationUrl + "number-binding")]
	[AddComponentMenu("PiRho Soft/Interface/Number Binding")]
	public class NumberBinding : VariableBinding
	{
		private const string _invalidTextWarning = "(CIBNBIT) Unable to animate text for number binding {0}: the displayed text is not an int";

		public BindingFormatter Format;

		[Tooltip("The variable holding the value to display as text in this object")]
		public VariableReference Variable = new VariableReference();

		private TextMeshProUGUI _text;

		public TextMeshProUGUI Text
		{
			get
			{
				// can't look up in awake because it's possible to update bindings before the component is enabled

				if (!_text)
					_text = GetComponent<TextMeshProUGUI>();

				return _text;
			}
		}

		protected override void UpdateBinding(IVariableStore variables, BindingAnimationStatus status)
		{
			var value = Variable.GetValue(variables);

			if (value.Type == VariableType.Int)
			{
				Text.enabled = true;
				Text.text = Format.GetFormattedString(value.Int);
			}
			else if (value.Type == VariableType.Float)
			{
				Text.enabled = true;
				Text.text = Format.GetFormattedString(value.Float);
			}
			else
			{
				Text.enabled = false;
			}
		}
	}
}
