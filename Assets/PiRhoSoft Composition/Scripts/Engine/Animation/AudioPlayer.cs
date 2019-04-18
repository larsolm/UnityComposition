using System.Collections;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(AudioSource))]
	[HelpURL(Composition.DocumentationUrl + "autdio-player")]
	[AddComponentMenu("PiRho Soft/Animation/Audio Player")]
	public class AudioPlayer : MonoBehaviour, ICompletionNotifier
	{
		private const string _infiniteLoopingWarning = "(CAAUPIL) Unable to wait on sound for {0}: the clip '{1}' was set to loop and would have never finished";

		private AudioSource _audio;
		private bool _started = false;

		public bool IsComplete
		{
			get
			{
				var done = !_audio.isPlaying;

				if (_started && done) // Reset started here since PlaySound cannot
					_started = false;

				return !_started || done;
			}
		}

		protected virtual void Awake()
		{
			_audio = GetComponent<AudioSource>();
		}

		public void PlaySound(AudioClip sound, float volume)
		{
			_started = true;
			_audio.PlayOneShot(sound, volume);
		}

		public IEnumerator PlaySoundAndWait(AudioClip sound, float volume)
		{
			PlaySound(sound, volume);

			if (!_audio.loop)
			{
				while (!IsComplete)
					yield return null;
			}
			else
			{
				Debug.LogFormat(this, _infiniteLoopingWarning, name, sound.name);
			}
		}
	}
}
