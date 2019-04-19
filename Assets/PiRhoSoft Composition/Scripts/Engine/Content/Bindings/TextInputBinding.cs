using TMPro;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(TMP_InputField))]
	[HelpURL(Composition.DocumentationUrl + "text-input-binding")]
	[AddComponentMenu("PiRho Soft/Bindings/Text Input Binding")]
	public class TextInputBinding : VariableBinding
	{
		private const string _variableNotFoundWarning = "(CBTIBVNF) Unable to bind text to variable on {0}: the variable '{1}' could not be found";
		private const string _readOnlyWarning = "(CBTIBRO) Unable to bind text to variable on {0}: the variable '{1}' is read only";

		[Tooltip("The variable holding the value to set basedo on the text in this object")]
		public VariableReference Variable = new VariableReference();

		private IVariableStore _variables;
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

		protected override void UpdateBinding(IVariableStore variables, BindingAnimationStatus status)
		{
			_variables = variables;
			Text.textComponent.text = Variable.GetValue(_variables).ToString();
		}

		private void TextChanged(string text)
		{
			if (_variables != null)
			{
				var result = Variable.SetValue(_variables, VariableValue.Create(Text.textComponent.text));

				if (!SuppressErrors)
				{
					if (result == SetVariableResult.NotFound)
						Debug.LogWarningFormat(this, _variableNotFoundWarning, name, Variable);
					else if (result == SetVariableResult.ReadOnly)
						Debug.LogWarningFormat(this, _readOnlyWarning, name, Variable);
				}
			}
		}
	}
}
