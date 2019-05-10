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
			Text.enabled = Resolve(variables, Variable, out Color color);
			Text.color = color;
		}
	}
}
