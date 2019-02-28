using System.Collections;
using System.Collections.Generic;
using PiRhoSoft.UtilityEngine;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[HelpURL(Composition.DocumentationUrl + "transition-manager")]
	public class TransitionManager : GlobalBehaviour<TransitionManager>
	{
		private const string _invalidAddWarning = "(CTMIA) this TransitionRenderer has already been added";
		private const string _invalidRemoveWarning = "(CTMIR) this TransitionRenderer has not been added";

		private List<TransitionRenderer> _renderers = new List<TransitionRenderer>();

		public Transition CurrentTransition { get; private set; }
		public TransitionRenderer CurrentRenderer => _renderers.Count > 0 ? _renderers[_renderers.Count - 1] : null;

		public void AddRenderer(TransitionRenderer renderer)
		{
			if (!_renderers.Contains(renderer))
				_renderers.Add(renderer);
			else
				Debug.LogWarning(_invalidAddWarning, renderer);
		}

		public void RemoveRenderer(TransitionRenderer renderer)
		{
			if (!_renderers.Remove(renderer))
				Debug.LogWarning(_invalidRemoveWarning, renderer);
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
				CurrentTransition.End();
				CurrentTransition = null;
			}
		}
	}
}
