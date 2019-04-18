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
		private const string _variableNotFoundWarning = "(CIBTIBVNF) Unable to bind text to variable on {0}: the variable '{1}' could not be found";
		private const string _readOnlyWarning = "(CIBTIBRO) Unable to bind text to variable on {0}: the variable '{1}' is read only";

		[Tooltip("The variable holding the value to set basedo on the text in this object")]
		public VariableReference Variable = new VariableReference();

		private TMP_InputField _text;
		private IVariableStore _variables;

		void Awake()
		{
			_text = GetComponent<TMP_InputField>();
			_text.onValueChanged.AddListener(TextChanged);
		}

		protected override void UpdateBinding(IVariableStore variables, BindingAnimationStatus status)
		{
			_variables = variables;
			_text.textComponent.text = Variable.GetValue(_variables).ToString();
		}

		private void TextChanged(string text)
		{
			if (_variables != null)
			{
				var result = Variable.SetValue(_variables, VariableValue.Create(_text.textComponent.text));

				if (result == SetVariableResult.NotFound)
					Debug.LogWarningFormat(this, _variableNotFoundWarning, name, Variable);
				else if (result == SetVariableResult.ReadOnly)
					Debug.LogWarningFormat(this, _readOnlyWarning, name, Variable);
			}
		}
	}
}
