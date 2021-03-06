﻿using PiRhoSoft.UtilityEngine;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace PiRhoSoft.CompositionEngine
{
	[Serializable]
	public class TimelineVariableSource : VariableSource<TimelineAsset> { }

	[CreateInstructionGraphNodeMenu("Animation/Play Timeline", 200)]
	[HelpURL(Composition.DocumentationUrl + "play-timeline-node")]
	public class PlayTimelineNode : InstructionGraphNode
	{
		[Tooltip("The node to move to when this node is finished")]
		public InstructionGraphNode Next = null;

		[Tooltip("The Playable Director to run the timeline on")]
		[VariableConstraint(typeof(PlayableDirector))]
		public VariableReference Director;

		[Tooltip("The timeline to run")]
		[ClassDisplay(ClassDisplayType.Propogated)]
		public TimelineVariableSource Timeline = new TimelineVariableSource();

		[Tooltip("The mode of the director when the timeline gets to the end")]
		public DirectorWrapMode Mode;

		[Tooltip("Whether to wait for the timeline to finish before moving on to Next")]
		[ConditionalDisplaySelf(nameof(Mode), EnumValue = (int)DirectorWrapMode.Loop, Invert = true)]
		public bool WaitForCompletion = false;

		public override Color NodeColor => Colors.Animation;

		public override IEnumerator Run(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			if (ResolveObject(variables, Director, out PlayableDirector director))
			{
				if (ResolveObject(variables, Timeline, out var timeline))
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
