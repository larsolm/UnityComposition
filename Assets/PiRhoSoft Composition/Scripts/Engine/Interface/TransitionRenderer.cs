using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Camera))]
	[HelpURL(Composition.DocumentationUrl + "transition-renderer")]
	[AddComponentMenu("PiRho Soft/Interface/Transition Renderer")]
	public class TransitionRenderer : MonoBehaviour
	{
		void OnEnable()
		{
			TransitionManager.Instance.AddRenderer(this);
		}

		void OnDisable()
		{
			if (TransitionManager.Exists)
				TransitionManager.Instance.RemoveRenderer(this);
		}

		void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			// all loaded and enabled renderers will get called, but the transition should only be drawn once

			if (TransitionManager.Instance.CurrentTransition != null && TransitionManager.Instance.CurrentRenderer == this)
				TransitionManager.Instance.CurrentTransition.Render(this, source, destination);
			else
				Graphics.Blit(source, destination);
		}
	}
}
