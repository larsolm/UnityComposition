using PiRhoSoft.Utilities;
using System.Collections;
using UnityEngine;

namespace PiRhoSoft.Composition.Extensions
{
	[CreateGraphNodeMenu("Audio/Push Audio", 1)]
	public class PushAudioNode : GraphNode
	{
		[Tooltip("The node to move to when this node is finished")]
		public GraphNode Next;

		[Tooltip("The Audio Clip to push onto the Audio Manager's stack")]
		public AudioClipVariableSource AudioClip;

		[Tooltip("How long to fade in this audio clip")]
		[Min(0.0f)]
		[CustomLabel("Fade In (seconds)")]
		public float FadeIn = 0.0f;

		[Tooltip("How long to fade out the previous audio clip")]
		[Min(0.0f)]
		[CustomLabel("Fade Out (seconds)")]
		public float FadeOut = 0.0f;

		[Tooltip("How long to crossfade this audio clip")]
		[Min(0.0f)]
		[CustomLabel("Crossfade (seconds)")]
		public float Crossfade  = 0.0f;

		public override IEnumerator Run(IGraphRunner graph, IVariableCollection variables)
		{
			if (variables.ResolveObject(this, AudioClip, out var clip))
				AudioManager.Instance.Push(clip, FadeIn, FadeOut, Crossfade);

			graph.GoTo(Next, nameof(Next));

			yield break;
		}
	}
}