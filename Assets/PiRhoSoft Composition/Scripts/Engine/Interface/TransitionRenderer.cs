using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Camera))]
	[HelpURL(Composition.DocumentationUrl + "transition-renderer")]
	[AddComponentMenu("Composition/Transition Renderer")]
	public class TransitionRenderer : MonoBehaviour
	{
		private const string _missingManagerWarning = "(CTRMM) failed to add TransitionRenderer: a TransitionManager has not been created";

		void OnEnable()
		{
			if (TransitionManager.Exists)
			{
				TransitionManager.Instance.AddRenderer(this);
			}
			else
			{
				Debug.LogWarning(_missingManagerWarning, this);
				enabled = false;
			}
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
