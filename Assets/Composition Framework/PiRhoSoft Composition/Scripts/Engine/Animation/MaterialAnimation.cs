using PiRhoSoft.UtilityEngine;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[ExecuteInEditMode]
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Renderer))]
	[HelpURL(Composition.DocumentationUrl + "material-animation")]
	[AddComponentMenu("PiRho Soft/Animation/Material Animation")]
	public class MaterialAnimation : MonoBehaviour, ICompletionNotifier
	{
		private static readonly int _progressId = Shader.PropertyToID("_Progress");

		[Tooltip("Whether to advance the progress of the animation automatically or manually")]
		public bool AutoAdvance = true;

		[Tooltip("The progress of the animation (from 0 to 1)")]
		[ConditionalDisplaySelf(nameof(AutoAdvance), Invert = true)]
		public float Progress = 0.0f;

		[Tooltip("Progress is affected by Time.timeScale")]
		[ConditionalDisplaySelf(nameof(AutoAdvance))]
		public bool UseScaledTime = true;

		[Tooltip("The duration of the animation (in seconds)")]
		[ConditionalDisplaySelf(nameof(AutoAdvance))]
		public float Duration = 2.0f;

		public bool IsComplete => AutoAdvance ? Progress >= Duration : Progress >= 1.0f; 

		private Renderer _renderer;
		private MaterialPropertyBlock _propertyBlock;

		protected virtual void OnEnable()
		{
			if (AutoAdvance)
				Progress = 0.0f;

			_renderer = GetComponent<Renderer>();
			_propertyBlock = new MaterialPropertyBlock();
		}

		protected virtual void OnDisable()
		{
			_renderer = null;
			_propertyBlock = null;
		}

		protected virtual void LateUpdate()
		{
			if (AutoAdvance)
				Progress += UseScaledTime ? Time.deltaTime : Time.unscaledDeltaTime;

			var progress = AutoAdvance ? Mathf.Clamp01(Progress / Duration) : Mathf.Clamp01(Progress);

			_renderer.GetPropertyBlock(_propertyBlock);
			_propertyBlock.SetFloat(_progressId, progress);
			_renderer.SetPropertyBlock(_propertyBlock);
		}
	}
}
