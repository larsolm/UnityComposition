using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[CreateAssetMenu(menuName = "PiRho Soft/Transitions/Pixelate", fileName = nameof(PixelateTransition), order = 111)]
	[HelpURL(Composition.DocumentationUrl + "pixelate")]
	public class PixelateTransition : Transition
	{
		private static readonly int _amountId = Shader.PropertyToID("_Amount");

		[Tooltip("The maximum amount to pixelate")]
		[Min(1)]
		public int MaxAmount = 100;

		private int _amount = 1;

		void OnEnable()
		{
			SetShader("PiRhoSoft/Composition/Shaders/Pixelate");
		}

		public override void Begin(TransitionPhase phase)
		{
			if (phase == TransitionPhase.Out)
				_amount = 1;
			else if (phase == TransitionPhase.In)
				_amount = MaxAmount;
		}

		public override void Process(float time, TransitionPhase phase)
		{
			if (phase == TransitionPhase.Out)
				_amount++;
			else if (phase == TransitionPhase.In)
				_amount--;

			_amount = Mathf.Clamp(_amount, 1, MaxAmount);
		}

		protected override void Update()
		{
			if (Material)
				Material.SetInt(_amountId, _amount);
		}
	}
}
