using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[HelpURL(Composition.DocumentationUrl + "cutoff-transition")]
	public abstract class CutoffTransition : Transition
	{
		private static readonly int _textureID = Shader.PropertyToID("_TransitionTexture");
		private static readonly int _distortId = Shader.PropertyToID("_Distort");
		private static readonly int _cutoffId = Shader.PropertyToID("_Cutoff");
		private static readonly int _fadeId = Shader.PropertyToID("_Fade");
		private static readonly int _colorId = Shader.PropertyToID("_Color");

		private Texture2D _texture;
		private bool _updateTexture = true;

		private Color _color;
		private bool _updateColor = true; 

		private float _cutoff;
		private bool _updateCutoff = true;

		private float _fade;
		private bool _updateFade = true;

		private bool _distort = true;
		private bool _updateDistort = true;

		protected override void OnEnable()
		{
			if (Shader == null)
				Shader = Shader.Find("PiRhoSoft/Composition/Shaders/Cutoff");

			base.OnEnable();
			Setup();
		}

		protected void SetTexture(Texture2D texture)
		{
			_updateTexture = true;
			_texture = texture;
		}

		protected void SetColor(Color color)
		{
			_updateColor = true;
			_color = color;
		}

		protected void SetCutoff(float cutoff)
		{
			_updateCutoff = true;
			_cutoff = cutoff;
		}

		protected void SetFade(float fade)
		{
			_updateFade = true;
			_fade = fade;
		}

		protected void SetDistort(bool distort)
		{
			_updateDistort = true;
			_distort = distort;
		}

		public override void Process(float time, TransitionPhase phase)
		{
			var progress = 1.0f;

			if (phase == TransitionPhase.Out)
				progress = Mathf.Lerp(0.0f, 1.0f, time / Duration);
			else if (phase == TransitionPhase.In)
				progress = Mathf.Lerp(1.0f, 0.0f, time / Duration);

			SetCutoff(progress);
			SetFade(progress);
		}

		public override sealed void Render(RenderTexture source, RenderTexture destination)
		{
			if (Material != null)
			{
				Update();
				Graphics.Blit(source, destination, Material);
			}
		}

		protected virtual void Setup()
		{
			SetTexture(Texture2D.blackTexture);
			SetColor(Color.black);
			SetDistort(false);
		}

		protected override sealed void Update()
		{
			if (Material != null)
			{
				if (_updateTexture) Material.SetTexture(_textureID, _texture);
				if (_updateDistort) Material.SetInt(_distortId, _distort ? 1 : 0);
				if (_updateColor) Material.SetColor(_colorId, _color);
				if (_updateCutoff) Material.SetFloat(_cutoffId, _cutoff);
				if (_updateFade) Material.SetFloat(_fadeId, _fade);
			}

			_updateTexture = false;
			_updateDistort = false;
			_updateColor = false;
			_updateCutoff = false;
			_updateFade = false;
		}
	}
}
