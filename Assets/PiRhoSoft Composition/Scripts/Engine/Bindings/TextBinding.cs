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

		void Awake()
		{
			_text = GetComponent<TextMeshProUGUI>();
		}

		protected override void UpdateBinding(IVariableStore variables, BindingAnimationStatus status)
		{
			var value = Variable.GetValue(variables);

			_text.enabled = value.Type != VariableType.Empty;
			_text.text = value.ToString();
		}
	}
}
