using PiRhoSoft.UtilityEngine;
using System.Collections;
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

		[Tooltip("The speed of change of the value (0 means instant)")]
		[Min(0)]
		public int Speed = 0;

		[Tooltip("Speed is affected by Time.timeScale")]
		public bool UseScaledTime = true;

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
			status?.Increment();

			var value = Variable.GetValue(variables);
			Text.enabled = value.Type == VariableType.Int || value.Type == VariableType.Float;

			if (Text.enabled)
			{
				if (Speed <= 0)
				{
					SetValue(value.Int);
					status?.Decrement();
				}
				else
				{
					StartCoroutine(AnimateValue(value.Int, status));
				}
			}
		}

		private void SetValue(int value)
		{
			Text.text = Format.GetFormattedString(value);
		}

		private IEnumerator AnimateValue(int target, BindingAnimationStatus status)
		{
			if (int.TryParse(Text.text, out var current))
			{
				var value = (float)current;
				while (current != target)
				{
					var delta = UseScaledTime ? Time.deltaTime : Time.unscaledDeltaTime;
					var speed = Speed * delta;
					value = Mathf.MoveTowards(value, target, speed);
					current = Mathf.RoundToInt(value);

					SetValue(current);

					yield return null;
				}
			}
			else
			{
				Debug.LogWarningFormat(this, _invalidTextWarning, name);
			}	

			status?.Decrement();
		}
	}
}
