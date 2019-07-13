using PiRhoSoft.Composition.Engine;
using System.Collections;
using UnityEngine;

namespace PiRhoSoft.MonsterRpg.Engine
{
	[CreateGraphNodeMenu("Audio/Push Audio", order: 0)]
	public class PushAudio : GraphNode
	{
		public AudioClipVariableSource AudioClip;

		[Min(0.0f)] public float FadeIn = 0.0f;
		[Min(0.0f)] public float FadeOut = 0.0f;
		[Min(0.0f)] public float Crossfade  = 0.0f;

		public GraphNode Next;

		public override IEnumerator Run(Graph graph, GraphStore variables, int iteration)
		{
			if (ResolveObject(variables, AudioClip, out var clip))
				AudioManager.Instance.Push(clip, FadeIn, FadeOut, Crossfade);

			graph.GoTo(Next, nameof(Next));

			yield break;
		}
	}
}