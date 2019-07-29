using UnityEngine;

namespace PiRhoSoft.Composition
{
	[CreateAssetMenu(menuName = "PiRho Soft/Transitions/Fade", fileName = nameof(FadeTransition), order = 309)]
	[HelpURL(Configuration.DocumentationUrl + "fade-transition")]
	public class FadeTransition : CutoffTransition
	{
		[Tooltip("The color to fade to")]
		public Color Color;

		protected override void Setup()
		{
			SetColor(Color);
			SetDistort(false);
			SetTexture(Texture2D.blackTexture);
		}
	}
}
