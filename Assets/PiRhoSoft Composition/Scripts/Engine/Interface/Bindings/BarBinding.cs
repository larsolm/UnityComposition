using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace PiRhoSoft.CompositionEngine
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Image))]
	[HelpURL(Composition.DocumentationUrl + "bar-binding")]
	[AddComponentMenu("PiRho Soft/Interface/Bar Binding")]
	public class BarBinding : InterfaceBinding
	{
		private const string _invalidAmountError = "(CBBIA) Failed to update bar binding: the amount variable '{0}' is not an Integer or Number";
		private const string _missingAmountError = "(CBBMA) Failed to update bar binding: the amount variable '{0}' could not be found";
		private const string _invalidTotalError = "(CBBIT) Failed to update bar binding: the total variable '{0}' is not an Integer or Number";
		private const string _missingTotalError = "(CBBMT) Failed to update bar binding: the total variable '{0}' could not be found";

		[Tooltip("The speed at which to animate the fill change (in % per second, 0 means immediate)")]
		[Range(0.0f, 1.0f)]
		public float Speed = 0.0f;

		[Tooltip("The variable holding the amount (numerator) the image should be filled")]
		public VariableReference AmountVariable = new VariableReference();

		[Tooltip("The variable holding the total (denomerator) for determining the amound the image should be filled")]
		public VariableReference TotalVariable = new VariableReference();

		[Tooltip("The blend colors to apply to the image depending on the fill amount")]
		public Gradient FillColors = new Gradient();

		protected Image _image;

		void Awake()
		{
			_image = GetComponent<Image>();
		}

		public override void UpdateBinding(IVariableStore variables, BindingAnimationStatus status)
		{
			status?.Increment();

			var fill = GetFill(variables);

			if (Speed <= 0.0f)
			{
				SetFill(fill);
				status?.Decrement();
			}
			else
			{
				StartCoroutine(AnimateFill(fill, status));
			}
		}

		private float GetFill(IVariableStore variables)
		{
			var amountValue = AmountVariable.GetValue(variables);
			var totalValue = TotalVariable.GetValue(variables);

			float amount, total;

			switch (amountValue.Type)
			{
				case VariableType.Empty: Debug.LogErrorFormat(this, _missingAmountError, AmountVariable); amount = 0.0f; break;
				case VariableType.Integer: amount = amountValue.Integer; break;
				case VariableType.Number: amount = amountValue.Number; break;
				default: Debug.LogErrorFormat(this, _invalidAmountError, AmountVariable); amount = 0.0f; break;
			}

			switch (totalValue.Type)
			{
				case VariableType.Empty: Debug.LogErrorFormat(this, _missingTotalError, TotalVariable); total = 1.0f; break;
				case VariableType.Integer: total = totalValue.Integer; break;
				case VariableType.Number: total = totalValue.Number; break;
				default: Debug.LogErrorFormat(this, _invalidTotalError, TotalVariable); total = 1.0f; break;
			}

			return amount / total;
		}

		private void SetFill(float amount)
		{
			var fill = Mathf.Clamp01(amount);

			_image.color = FillColors.Evaluate(amount);
			_image.fillAmount = amount;
		}

		private IEnumerator AnimateFill(float target, BindingAnimationStatus status)
		{
			while (_image.fillAmount != target)
			{
				var speed = Speed * Time.deltaTime;
				var fill = Mathf.MoveTowards(_image.fillAmount, target, speed);

				SetFill(fill);

				yield return null;
			}

			status?.Decrement();
		}
	}
}
