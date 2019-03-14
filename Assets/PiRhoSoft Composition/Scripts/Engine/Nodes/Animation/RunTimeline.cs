using PiRhoSoft.UtilityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace PiRhoSoft.CompositionEngine
{
	[Serializable]
	public class TimelineVariableSource : ObjectVariableSource<TimelineAsset> { }

	[CreateInstructionGraphNodeMenu("Animation/Run Timeline", 200)]
	[HelpURL(Composition.DocumentationUrl + "play-animation")]
	public class RunTimeline : InstructionGraphNode
	{
		private const string _timelineNotFoundWarning = "(CARTTNF) Unable to run timeline for {0}: the timeline could not be found";
		private const string _invalidDirectorWarning = "(CARTID) Unable to run timeline for {0}: the given variables must me a PlayableDirector";

		[Tooltip("The node to move to when this node is finished")]
		public InstructionGraphNode Next = null;

		[Tooltip("The timeline to run")]
		[InlineDisplay(PropagateLabel = true)]
		public TimelineVariableSource Timeline = new TimelineVariableSource();

		[Tooltip("The mode of the director when the timeline gets to the end")]
		public DirectorWrapMode Mode;

		[Tooltip("Whether to wait for the timeline to finish before moving on to Next")]
		[ConditionalDisplaySelf(nameof(Mode), EnumValue = (int)DirectorWrapMode.Loop, Invert = true)]
		public bool WaitForCompletion = false;

		public override Color NodeColor => Colors.Animation;

		public override void GetInputs(List<VariableDefinition> inputs)
		{
			Timeline.GetInputs(inputs);
		}

		public override IEnumerator Run(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			if (variables.Root is PlayableDirector playable)
			{
				if (Timeline.TryGetValue(variables, this, out var timeline))
				{
					playable.Play(timeline, Mode);

					if (WaitForCompletion)
					{
						if (Mode == DirectorWrapMode.None)
						{
							while (playable.state == PlayState.Playing)
								yield return null;
						}
						else if (Mode == DirectorWrapMode.Hold)
						{
							while (playable.time < playable.duration)
								yield return null;
						}
					}
				}
				else
				{
					Debug.LogWarningFormat(this, _timelineNotFoundWarning, timeline);
				}
			}
			else
			{
				Debug.LogWarningFormat(this, _invalidDirectorWarning, Name);
			}

			graph.GoTo(Next, nameof(Next));
		}
	}
}
