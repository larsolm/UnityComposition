using UnityEngine;

namespace PiRhoSoft.Composition.Engine
{
	[CreateAssetMenu(menuName = "PiRho Soft/Transitions/Pixelate", fileName = nameof(PixelateTransition), order = 311)]
	[HelpURL(Composition.DocumentationUrl + "pixelate-transition")]
	public class PixelateTransition : Transition
	{
		private static readonly int _amountId = Shader.PropertyToID("_Amount");

		[Tooltip("The maximum amount to pixelate")]
		[Min(1)]
		public int MaxAmount = 100;

		private int _amount = 1;

		protected override void OnEnable()
		{
			if (Shader == null)
				Shader = Shader.Find("PiRhoSoft/Composition/Shaders/Pixelate");

			base.OnEnable();
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
