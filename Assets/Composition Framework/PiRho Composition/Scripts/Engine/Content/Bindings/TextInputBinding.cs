using TMPro;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(TMP_InputField))]
	[HelpURL(Composition.DocumentationUrl + "text-input-binding")]
	[AddComponentMenu("PiRho Soft/Bindings/Text Input Binding")]
	public class TextInputBinding : VariableBinding
	{
		[Tooltip("The variable holding the value to set basedo on the text in this object")]
		public VariableReference Variable = new VariableReference();

		private IVariableCollection _variables;
		private TMP_InputField _text;

		public TMP_InputField Text
		{
			get
			{
				// can't look up in awake because it's possible to update bindings before the component is enabled

				if (!_text)
					_text = GetComponent<TMP_InputField>();

				return _text;
			}
		}

		protected override void Awake()
		{
			base.Awake();

			Text.onValueChanged.AddListener(TextChanged);
		}

		protected override void UpdateBinding(IVariableCollection variables, BindingAnimationStatus status)
		{
			_variables = variables;
			Text.textComponent.text = Variable.GetValue(_variables).ToString();
		}

		private void TextChanged(string text)
		{
			if (_variables != null)
				Assign(_variables, Variable, PiRhoSoft.Composition.Variable.String(Text.textComponent.text));
		}
	}
}
