using TMPro;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(TextMeshProUGUI))]
	[HelpURL(Composition.DocumentationUrl + "text-binding")]
	[AddComponentMenu("Composition/Interface/Text Binding")]
	public class TextBinding : InterfaceBinding
	{
		[Tooltip("The variable holding the value to display as text in this object")]
		public VariableReference Variable = new VariableReference();

		private TextMeshProUGUI _text;

		void Awake()
		{
			_text = GetComponent<TextMeshProUGUI>();
		}

		public override void UpdateBinding(IVariableStore variables, BindingAnimationStatus status)
		{
			_text.text = Variable.GetValue(variables).ToString();
		}
	}
}
