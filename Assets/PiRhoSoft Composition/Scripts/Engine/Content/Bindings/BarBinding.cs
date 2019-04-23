﻿using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace PiRhoSoft.CompositionEngine
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Image))]
	[HelpURL(Composition.DocumentationUrl + "bar-binding")]
	[AddComponentMenu("PiRho Soft/Bindings/Bar Binding")]
	public class BarBinding : VariableBinding
	{
		private const string _missingVariableWarning = "(CBBMV) unable to bind {0} for binding {1}: variable '{2}' could not be found";
		private const string _invalidVariableWarning = "(CBBIV) unable to bind {0} for binding {1}: variable '{2}' is not an int or float";
		private const string _wrongTypeWarning = "(CBBBWT) Bar Binding '{0}' has an Image component with a type that is not 'Filled'";

		[Tooltip("The variable holding the amount (numerator) the image should be filled")]
		public VariableReference AmountVariable = new VariableReference();

		[Tooltip("The variable holding the total (denomerator) for determining the amound the image should be filled")]
		public VariableReference TotalVariable = new VariableReference();

		[Tooltip("The blend colors to apply to the image depending on the fill amount")]
		public Gradient FillColors = new Gradient();

		[Tooltip("The speed at which to animate the fill change (in % per second, 0 means immediate)")]
		[Range(0.0f, 1.0f)]
		public float Speed = 0.0f;

		[Tooltip("Speed is affected by Time.timeScale")]
		public bool UseScaledTime = true;

		private Image _image;

		protected override void Awake()
		{
			base.Awake();
			_image = GetComponent<Image>();

			if (_image.type != Image.Type.Filled)
				Debug.LogFormat(this, _wrongTypeWarning, name);
		}

		protected override void UpdateBinding(IVariableStore variables, BindingAnimationStatus status)
		{
			var amountValue = AmountVariable.GetValue(variables);
			var totalValue = TotalVariable.GetValue(variables);

			_image.enabled = amountValue.HasNumber && totalValue.HasNumber;

			if (_image.enabled)
			{
				var fill = amountValue.Number / totalValue.Number;

				if (Speed <= 0.0f)
				{
					SetFill(fill);
				}
				else
				{
					StopAllCoroutines();
					StartCoroutine(AnimateFill(fill, status));
				}
			}
			else if (!SuppressErrors)
			{
				if (amountValue.IsEmpty)
					Debug.LogWarningFormat(this, _missingVariableWarning, nameof(AmountVariable), this, AmountVariable);
				else if (!amountValue.HasNumber)
					Debug.LogWarningFormat(this, _invalidVariableWarning, nameof(AmountVariable), this, AmountVariable);

				if (totalValue.IsEmpty)
					Debug.LogWarningFormat(this, _missingVariableWarning, nameof(TotalVariable), this, TotalVariable);
				else if (!totalValue.HasNumber)
					Debug.LogWarningFormat(this, _invalidVariableWarning, nameof(TotalVariable), this, TotalVariable);
			}
		}

		private void SetFill(float amount)
		{
			var fill = Mathf.Clamp01(amount);

			_image.color = FillColors.Evaluate(amount);
			_image.fillAmount = amount;
		}

		private IEnumerator AnimateFill(float target, BindingAnimationStatus status)
		{
			status.Increment();

			while (_image.fillAmount != target)
			{
				var delta = UseScaledTime ? Time.deltaTime : Time.unscaledDeltaTime;
				var speed = Speed * delta;
				var fill = Mathf.MoveTowards(_image.fillAmount, target, speed);

				SetFill(fill);

				yield return null;
			}

			status.Decrement();
		}
	}
}