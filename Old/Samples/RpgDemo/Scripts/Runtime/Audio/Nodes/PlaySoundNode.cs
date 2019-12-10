﻿using System;
using System.Collections;
using UnityEngine;

namespace PiRhoSoft.Composition.Extensions
{
	[Serializable]
	public class SoundVariableSource : VariableSource<AudioClip> { }

	[CreateGraphNodeMenu("Audio/Play Sound", 101)]
	public class PlaySoundNode : GraphNode
	{
		[Tooltip("The node to move to when this node is finished")]
		public GraphNode Next = null;

		[Tooltip("The Audio Player to play the sound on")]
		[VariableConstraint(typeof(AudioPlayer))]
		public VariableLookupReference AudioPlayer = new VariableLookupReference();

		[Tooltip("The sound to play")]
		public SoundVariableSource Sound = new SoundVariableSource();

		[Tooltip("The volume to play the sound at")]
		[VariableConstraint(0.0f, 0.1f)]
		public FloatVariableSource Volume = new FloatVariableSource(1.0f);

		[Tooltip("Whether to wait for the sound to finish before moving to Next")]
		public bool WaitForCompletion = false;

		public override Color NodeColor => Colors.Animation;

		public override IEnumerator Run(IGraphRunner graph, IVariableMap variables)
		{
			if (variables.ResolveObject<AudioPlayer>(this, AudioPlayer, out var player))
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