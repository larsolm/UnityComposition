using PiRhoSoft.Utilities.Engine;
using System;
using System.Collections;
using UnityEngine;

namespace PiRhoSoft.Composition.Engine
{
	[Serializable]
	public class AudioClipVariableSource : VariableSource<AudioClip> { }

	[CreateGraphNodeMenu("Animation/Play Sound", 101)]
	[HelpURL(Composition.DocumentationUrl + "play-sound-node")]
	public class PlaySoundNode : GraphNode
	{
		[Tooltip("The node to move to when this node is finished")]
		public GraphNode Next = null;

		[Tooltip("The Audio Player to play the sound on")]
		[VariableConstraint(typeof(AudioPlayer))]
		public VariableReference AudioPlayer;

		[Tooltip("The Audio Clip to play")]
		[Inline]
		public AudioClipVariableSource Sound = new AudioClipVariableSource();

		[Tooltip("The volume to play the sound at")]
		[Inline]
		public FloatVariableSource Volume = new FloatVariableSource(1.0f);

		[Tooltip("Whether to wait for the sound to finish before moving to Next")]
		public bool WaitForCompletion = false;

		public override Color NodeColor => Colors.Animation;

		public override IEnumerator Run(Graph graph, GraphStore variables, int iteration)
		{
			if (ResolveObject(variables, AudioPlayer, out AudioPlayer player))
			{
				if (ResolveObject(variables, Sound, out var sound))
				{
					if (!Resolve(variables, Volume, out var volume))
						volume = 1.0f;

					if (WaitForCompletion)
						yield return player.PlaySoundAndWait(sound, volume);
					else
						player.PlaySound(sound, volume);
				}
			}

			graph.GoTo(Next, nameof(Next));
		}
	}
}
