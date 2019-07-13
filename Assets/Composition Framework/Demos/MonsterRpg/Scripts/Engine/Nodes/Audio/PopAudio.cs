using PiRhoSoft.Composition.Engine;
using System.Collections;
using UnityEngine;

namespace PiRhoSoft.MonsterRpg.Engine
{
	[CreateGraphNodeMenu("Audio/Pop Audio", order: 1)]
	public class PopAudio : GraphNode
	{
		[Min(0.0f)] public float FadeOut = 0.0f;
		[Min(0.0f)] public float FadeIn = 0.0f;
		[Min(0.0f)] public float Crossfade = 0.0f;

		public GraphNode Next;

		public override IEnumerator Run(Graph graph, GraphStore variables, int iteration)
		{
			AudioManager.Instance.Pop(FadeOut, FadeIn, Crossfade);

			graph.GoTo(Next, nameof(Next));

			yield break;
		}
	}
}