using UnityEngine;
using UnityEngine.UI;

namespace PiRhoSoft.CompositionEngine
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Image))]
	[HelpURL(Composition.DocumentationUrl + "bar-binding")]
	[AddComponentMenu("Composition/Interface/Bar Binding")]
	public class BarBinding : InterfaceBinding
	{
		private const string _invalidAmountError = "(IBBIA) Failed to update bar binding: the amount variable '{0}' is not an Integer or Number";
		private const string _missingAmountError = "(IBBMA) Failed to update bar binding: the amount variable '{0}' could not be found";
		private const string _invalidTotalError = "(IBBIT) Failed to update bar binding: the total variable '{0}' is not an Integer or Number";
		private const string _missingTotalError = "(IBBMT) Failed to update bar binding: the total variable '{0}' could not be found";

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

		public override void UpdateBinding(IVariableStore variables)
		{
			var fill = GetFill(variables);

			_image.color = FillColors.Evaluate(fill);
			_image.fillAmount = fill;
		}

		protected float GetFill(IVariableStore variables)
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
	}
}
