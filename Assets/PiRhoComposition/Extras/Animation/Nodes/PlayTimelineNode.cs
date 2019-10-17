﻿using PiRhoSoft.Utilities;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace PiRhoSoft.Composition.Extensions
{
	[Serializable]
	public class TimelineVariableSource : VariableSource<TimelineAsset> { }

	[CreateGraphNodeMenu("Animation/Play Timeline", 200)]
	public class PlayTimelineNode : GraphNode
	{
		[Tooltip("The node to move to when this node is finished")]
		public GraphNode Next = null;

		[Tooltip("The Playable Director to run the timeline on")]
		[VariableConstraint(typeof(PlayableDirector))]
		public VariableLookupReference Director = new VariableLookupReference();

		[Tooltip("The timeline to run")]
		public TimelineVariableSource Timeline = new TimelineVariableSource();

		[Tooltip("The mode of the director when the timeline gets to the end")]
		public DirectorWrapMode Mode;

		[Tooltip("Whether to wait for the timeline to finish before moving on to Next")]
		[Conditional(nameof(Mode), (int)DirectorWrapMode.Loop, Test = ConditionalTest.Inequal)]
		public bool WaitForCompletion = false;

		public override Color NodeColor => Colors.Animation;

		public override IEnumerator Run(IGraphRunner graph, IVariableMap variables)
		{
			if (variables.ResolveObject(this, Director, out PlayableDirector director))
			{
				if (variables.ResolveObject(this, Timeline, out var timeline))
				{
					director.Play(timeline, Mode);

					if (WaitForCompletion)
					{
						if (Mode == DirectorWrapMode.None)
						{
							while (director.state == PlayState.Playing)
								yield return null;
						}
						else if (Mode == DirectorWrapMode.Hold)
						{
							while (director.time < director.duration)
								yield return null;
						}
					}
				}
			}

			graph.GoTo(Next, nameof(Next));
		}
	}
}
