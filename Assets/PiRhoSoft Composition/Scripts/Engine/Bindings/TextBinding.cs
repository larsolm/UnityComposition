using TMPro;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(TextMeshProUGUI))]
	[HelpURL(Composition.DocumentationUrl + "text-binding")]
	[AddComponentMenu("PiRho Soft/Interface/Text Binding")]
	public class TextBinding : VariableBinding
	{
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

			Text.enabled = value.Type != VariableType.Empty;
			Text.text = value.ToString();
		}
	}
}
