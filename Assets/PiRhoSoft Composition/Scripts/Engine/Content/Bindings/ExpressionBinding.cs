using TMPro;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(TextMeshProUGUI))]
	[HelpURL(Composition.DocumentationUrl + "expression-binding")]
	[AddComponentMenu("PiRho Soft/Bindings/Expression Binding")]
	public class ExpressionBinding : VariableBinding
	{
		public BindingFormatter Formatting;

		[Tooltip("The expression to evaluate and display as text in this object")]
		public Expression Expression;

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
			try
			{
				var result = Expression.Evaluate(variables);

				Text.enabled = result.Type != VariableType.Empty;

				if (result.Type == VariableType.Int)
					Text.text = Formatting.GetFormattedString(result.Int);
				if (result.Type == VariableType.Float)
					Text.text = Formatting.GetFormattedString(result.Float);
				else
					Text.text = result.ToString();
			}
			catch
			{
				Text.enabled = false;
			}
		}
	}
}
