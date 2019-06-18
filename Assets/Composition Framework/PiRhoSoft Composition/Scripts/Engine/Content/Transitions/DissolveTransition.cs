using PiRhoSoft.PargonUtilities.Engine;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[CreateAssetMenu(menuName = "PiRho Soft/Transitions/Dissolve", fileName = nameof(DissolveTransition), order = 110)]
	[HelpURL(Composition.DocumentationUrl + "dissolve-transition")]
	public class DissolveTransition : CutoffTransition
	{
		[Tooltip("The color to dissolve the screen to")]
		public Color Color = Color.black;

		[Tooltip("The texture to use to for the dissolve (if none a random one will be generated)")]
		public Texture2D Texture;

		[Tooltip("The size of the texture to generate")]
		[Conditional(nameof(Texture), false)]
		public Vector2Int TextureSize = new Vector2Int(256, 256);

		[Tooltip("The scale to use for generating the perlin noise based texture")]
		[Conditional(nameof(Texture), false)]
		public float NoiseScale = 2.0f;
		
		private bool UseRandom => Texture == null;
		private Texture2D _randomTexture;

		protected override void Setup()
		{
			SetFade(1.0f);
			SetColor(Color);
			SetDistort(false);
		}

		public override void Begin(TransitionPhase phase)
		{
			if (UseRandom)
			{
				UpdateTexture();
				SetTexture(_randomTexture);
			}
			else
			{
				SetTexture(Texture);
			}
		}

		public override void End()
		{
			if (_randomTexture != null)
				Destroy(_randomTexture);

			_randomTexture = null;
		}

		public override void Process(float time, TransitionPhase phase)
		{
			var cutoff = 1.0f;

			if (phase == TransitionPhase.Out)
				cutoff = Mathf.Lerp(0.0f, 1.0f, time / Duration);
			else if (phase == TransitionPhase.In)
				cutoff = Mathf.Lerp(1.0f, 0.0f, time / Duration);

			SetCutoff(cutoff);
		}

		private void UpdateTexture()
		{
			if (_randomTexture == null || _randomTexture.width != TextureSize.x || _randomTexture.height != TextureSize.y)
			{
				if (_randomTexture != null)
					Destroy(_randomTexture);

				_randomTexture = new Texture2D(TextureSize.x, TextureSize.y, TextureFormat.RGB24, false);
			}

			var xOffset = Random.Range(0.0f, _randomTexture.width);
			var yOffset = Random.Range(0.0f, _randomTexture.height);

			for (var y = 0; y < _randomTexture.height; y++)
			{
				for (var x = 0; x < _randomTexture.width; x++)
				{
					var xCoord = xOffset + x / (float)_randomTexture.width * NoiseScale;
					var yCoord = yOffset + y / (float)_randomTexture.height * NoiseScale;
					var noise = Mathf.PerlinNoise(xCoord, yCoord);

					_randomTexture.SetPixel(x, y, new Color(noise, noise, noise));
				}
			}

			_randomTexture.Apply();
		}
	}
}
