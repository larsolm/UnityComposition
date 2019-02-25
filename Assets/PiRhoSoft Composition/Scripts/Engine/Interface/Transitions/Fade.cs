using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[CreateAssetMenu(menuName = "Composition/Transitions/Fade", fileName = nameof(Fade))]
	[HelpURL(Composition.DocumentationUrl + "fade")]
	public class Fade : Cutoff
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
