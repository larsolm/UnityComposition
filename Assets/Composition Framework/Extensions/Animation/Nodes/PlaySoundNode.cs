using PiRhoSoft.Utilities;
using System;
using System.Collections;
using UnityEngine;

namespace PiRhoSoft.Composition.Extensions
{
	[Serializable]
	public class SoundVariableSource : VariableSource<AudioClip> { }

	[CreateGraphNodeMenu("Animation/Play Sound", 101)]
	public class PlaySoundNode : GraphNode
	{
		[Tooltip("The node to move to when this node is finished")]
		public GraphNode Next = null;

		[Tooltip("The Audio Player to play the sound on")]
		[VariableReference(typeof(AudioPlayer))]
		public VariableReference AudioPlayer;

		[Tooltip("The sound to play")]
		[Inline]
		public SoundVariableSource Sound = new SoundVariableSource();

		[Tooltip("The volume to play the sound at")]
		[Inline]
		public FloatVariableSource Volume = new FloatVariableSource(1.0f);

		[Tooltip("Whether to wait for the sound to finish before moving to Next")]
		public bool WaitForCompletion = false;

		public override Color NodeColor => Colors.Animation;

		public override IEnumerator Run(IGraphRunner graph, IVariableCollection variables)
		{
			if (variables.ResolveObject(this, AudioPlayer, out AudioPlayer player))
			{
				if (variables.ResolveObject(this, Sound, out var sound))
				{
					if (!variables.Resolve(this, Volume, out var volume))
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
