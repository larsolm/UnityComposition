using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Camera))]
	[HelpURL(Composition.DocumentationUrl + "transition-renderer")]
	[AddComponentMenu("PiRho Soft/Interface/Transition Renderer")]
	public class TransitionRenderer : MonoBehaviour
	{
		private Camera _camera;

		void OnEnable()
		{
			_camera = GetComponent<Camera>();
			TransitionManager.Instance.AddRenderer(this);
		}

		void OnDisable()
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
