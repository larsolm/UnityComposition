using PiRhoSoft.Utilities;
using System.Collections;
using UnityEngine;

namespace PiRhoSoft.Composition.Extensions
{
	[CreateGraphNodeMenu("Audio/Pop Audio", 2)]
	public class PopAudioNode : GraphNode
	{
		[Tooltip("The node to move to when this node is finished")]
		public GraphNode Next;

		[Tooltip("How long to fade the previous audio clip")]
		[Minimum(0.0f)]
		[CustomLabel("Fade In (seconds)")]
		public float FadeIn = 0.0f;

		[Tooltip("How long to fade out this audio clip")]
		[Minimum(0.0f)]
		[CustomLabel("Fade Out (seconds)")]
		public float FadeOut = 0.0f;

		[Tooltip("How long to crossfade this audio clip")]
		[Minimum(0.0f)]
		[Maximum(nameof(MaxCrossfade))]
		[CustomLabel("Crossfade (seconds)")]
		public float Crossfade = 0.0f;
		private float MaxCrossfade() => Mathf.Min(FadeIn, FadeOut);

		public override IEnumerator Run(IGraphRunner graph, IVariableMap variables)
		{
			AudioManager.Instance.Pop(FadeOut, FadeIn, Crossfade);

			graph.GoTo(Next, nameof(Next));

			yield break;
		}
	}
}