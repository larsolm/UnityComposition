﻿using PiRhoSoft.Utilities;
using System;
using System.Collections;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	[Serializable]
	public class AnimationClipVariableSource : VariableSource<AnimationClip> { }

	[CreateGraphNodeMenu("Animation/Play Animation", 0)]
	[HelpURL(Composition.DocumentationUrl + "play-animation-node")]
	public class PlayAnimationNode : GraphNode
	{
		[Tooltip("The node to move to when this node is finished")]
		public GraphNode Next = null;

		[Tooltip("The Animation Player to play the clip on")]
		[VariableReference(typeof(AnimationPlayer))]
		public VariableReference AnimationPlayer;

		[Tooltip("The Animation Clip to play")]
		[Inline]
		public AnimationClipVariableSource Animation = new AnimationClipVariableSource();

		[Tooltip("Whether to wait for the animation to finish")]
		public bool WaitForCompletion = false;

		public override Color NodeColor => Colors.Animation;

		public override IEnumerator Run(Graph graph, GraphStore variables, int iteration)
		{
			if (ResolveObject<AnimationPlayer>(variables, AnimationPlayer, out var player))
			{
				if (ResolveObject(variables, Animation, out var animation))
				{
					if (WaitForCompletion)
						yield return player.PlayAnimationAndWait(animation);
					else
						player.PlayAnimation(animation);
				}
			}

			graph.GoTo(Next, nameof(Next));
		}
	}
}
