﻿using PiRhoSoft.Composition;
using System.Collections;
using UnityEngine;

namespace PiRhoSoft.MonsterRpg
{
	[CreateGraphNodeMenu("Audio/Push Audio", order: 0)]
	public class PushAudio : GraphNode
	{
		public AudioClipVariableSource AudioClip;

		[Min(0.0f)] public float FadeIn = 0.0f;
		[Min(0.0f)] public float FadeOut = 0.0f;
		[Min(0.0f)] public float Crossfade  = 0.0f;

		public GraphNode Next;

		public override IEnumerator Run(IGraphRunner graph, IVariableCollection variables)
		{
			if (variables.ResolveObject(this, AudioClip, out var clip))
				AudioManager.Instance.Push(clip, FadeIn, FadeOut, Crossfade);

			graph.GoTo(Next, nameof(Next));

			yield break;
		}
	}
}