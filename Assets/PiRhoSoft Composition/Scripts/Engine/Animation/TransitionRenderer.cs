using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Camera))]
	[HelpURL(Composition.DocumentationUrl + "transition-renderer")]
	[AddComponentMenu("PiRho Soft/Animation/Transition Renderer")]
	public class TransitionRenderer : MonoBehaviour
	{
		private Camera _camera;

		protected virtual void OnEnable()
		{
			_camera = GetComponent<Camera>();
			TransitionManager.Instance.AddRenderer(this);
		}

		protected virtual void OnDisable()
		{
			if (TransitionManager.Exists)
				TransitionManager.Instance.RemoveRenderer(this);
		}

		public virtual void SetTarget(RenderTexture target)
		{
			_camera.targetTexture = target;
		}
	}
}
