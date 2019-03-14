using PiRhoSoft.UtilityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[Serializable]
	public class AudioClipVariableSource : VariableSource<AudioClip> { }

	[CreateInstructionGraphNodeMenu("Animation/Play Sound", 101)]
	[HelpURL(Composition.DocumentationUrl + "play-sound")]
	public class PlaySound : InstructionGraphNode
	{
		private const string _soundNotFoundWarning = "(CAPSSNF) Unable to play sound for {0}: the audio clip could not be found";
		private const string _invalidVolumeWarning = "(CAPSIV) Unable to set volume for {0}: the volume could not be found - defaulting to 1";
		private const string _invalidPlayerWarning = "(CAPSIP) Unable to play sound for {0}: the given variables must me an AudioPlayer";

		[Tooltip("The node to move to when this node is finished")]
		public InstructionGraphNode Next = null;

		[Tooltip("The sound to play")]
		[InlineDisplay(PropagateLabel = true)]
		public AudioClipVariableSource Sound = new AudioClipVariableSource();

		[Tooltip("The volume to play the sound at")]
		[InlineDisplay(PropagateLabel = true)]
		public NumberVariableSource Volume = new NumberVariableSource(1.0f);

		[Tooltip("Whether to wait for the sound to finish before moving to Next")]
		public bool WaitForCompletion = false;

		public override Color NodeColor => Colors.Animation;

		public override void GetInputs(List<VariableDefinition> inputs)
		{
			Sound.GetInputs(inputs);
		}

		public override IEnumerator Run(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			if (variables.Root is AudioPlayer player)
			{
				if (Resolve(variables, Sound, out var sound))
				{
					if (!Resolve(variables, Volume, out var volume))
					{
						volume = 1.0f;
						Debug.LogWarningFormat(this, _invalidVolumeWarning, Name);
					}

					if (WaitForCompletion)
						yield return player.PlaySoundAndWait(sound, volume);
					else
						player.PlaySound(sound, volume);
				}
				else
				{
					Debug.LogWarningFormat(this, _soundNotFoundWarning, player);
				}
			}
			else
			{
				Debug.LogWarningFormat(this, _invalidPlayerWarning, name);
			}

			graph.GoTo(Next, nameof(Next));
		}
	}
}
