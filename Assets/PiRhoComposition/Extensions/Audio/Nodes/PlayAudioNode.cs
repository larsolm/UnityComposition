using PiRhoSoft.Utilities;
using System.Collections;
using UnityEngine;

namespace PiRhoSoft.Composition.Extensions
{
	[CreateGraphNodeMenu("Audio/Play Audio", 0)]
	public class PlayAudioNode : GraphNode
	{
		[Tooltip("The node to move to when this node is finished")]
		public GraphNode Next = null;

		[Tooltip("The Audio Clip to play")]
		public AudioClipVariableSource AudioClip = new AudioClipVariableSource();

		[Tooltip("The volume to play the sound at")]
		public FloatVariableSource Volume = new FloatVariableSource(1.0f);

		public override Color NodeColor => Colors.Animation;

		public override IEnumerator Run(IGraphRunner graph, IVariableCollection variables)
		{
			if (variables.ResolveObject(this, AudioClip, out var clip))
			{
				if (!variables.Resolve(this, Volume, out var volume))
					volume = 1.0f;

				AudioManager.Instance.Play(clip, volume);
			}

			graph.GoTo(Next, nameof(Next));
			yield break;
		}
	}
}
