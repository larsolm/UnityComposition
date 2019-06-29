using PiRhoSoft.PargonUtilities.Engine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Camera))]
	[HelpURL(Composition.DocumentationUrl + "transition-manager")]
	public sealed class TransitionManager : GlobalBehaviour<TransitionManager>
	{
		// implementation based on this post from the Unity forums: https://forum.unity.com/threads/postprocessing-issues-with-several-cameras.313903/#post-2040534

		private RenderTexture _target;
		private List<TransitionRenderer> _renderers = new List<TransitionRenderer>();

		public Transition CurrentTransition { get; private set; }

		void Awake()
		{
			CreateCamera();
			CreateTarget();
		}

		void OnDestroy()
		{
			DestroyTarget();
		}

		private void CreateCamera()
		{
			var camera = GetComponent<Camera>();
			camera.clearFlags = CameraClearFlags.Nothing;
			camera.cullingMask = 0;
			camera.depth = 1000;
		}

		private void CreateTarget()
		{
			_target = new RenderTexture(Screen.width, Screen.height, 32, RenderTextureFormat.ARGB32);
		}

		private void DestroyTarget()
		{
			_target.Release();
			_target = null;
		}

		internal void AddRenderer(TransitionRenderer renderer)
		{
			if (!_renderers.Contains(renderer))
				_renderers.Add(renderer);
		}

		internal void RemoveRenderer(TransitionRenderer renderer)
		{
			_renderers.Remove(renderer);
		}

		public IEnumerator RunTransition(Transition transition, TransitionPhase phase)
		{
			yield return StartTransition(transition, phase);
			EndTransition();
		}

		public IEnumerator StartTransition(Transition transition, TransitionPhase phase)
		{
			EndTransition();

			if (transition)
			{
				if (_target.width != Screen.width || _target.height != Screen.height)
				{
					DestroyTarget();
					CreateTarget();
				}

				foreach (var renderer in _renderers)
					renderer.SetTarget(_target);

				CurrentTransition = transition;
				CurrentTransition.Begin(phase);

				for (var elapsed = 0.0f; elapsed < CurrentTransition.Duration; elapsed += Time.unscaledDeltaTime)
				{
					CurrentTransition.Process(elapsed, phase);
					yield return null;
				}
			}
		}

		public void EndTransition()
		{
			if (CurrentTransition)
			{
				foreach (var renderer in _renderers)
					renderer.SetTarget(null);

				CurrentTransition.End();
				CurrentTransition = null;
			}
		}

		void OnPostRender()
		{
			if (CurrentTransition)
				CurrentTransition.Render(_target, null);
		}
	}
}
