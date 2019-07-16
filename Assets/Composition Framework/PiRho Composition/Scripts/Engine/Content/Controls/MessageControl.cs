using System.Collections;
using TMPro;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	[HelpURL(Composition.DocumentationUrl + "message-control")]
	[AddComponentMenu("PiRho Soft/Interface/Message Control")]
	public class MessageControl : InterfaceControl
	{
		[Tooltip("The object that message text will be displayed in")]
		public TMP_Text DisplayText = null;

		public bool IsRunning { get; private set; } = false;

		protected bool IsAdvancing { get; set; } = false;

		public void Show(string text)
		{
			Activate();
			StopAllCoroutines();

			if (DisplayText)
			{
				DisplayText.text = text;
				DisplayText.ForceMeshUpdate();
			}

			StartCoroutine(Run_());
		}

		public void Advance()
		{
			IsAdvancing = true;
		}

		protected override void Setup()
		{
			if (DisplayText)
				DisplayText.enabled = true;
		}

		protected override void Teardown()
		{
			if (DisplayText)
				DisplayText.enabled = false;

			StopAllCoroutines();

			IsRunning = false;
			IsAdvancing = false;
		}

		private IEnumerator Run_()
		{
			IsRunning = true;

			yield return Run();

			IsRunning = false;
		}

		protected virtual IEnumerator Run()
		{
			yield return null; // always wait one frame to skip any previous inputs

			while (!IsAdvancing)
				yield return null;

			IsAdvancing = false;
		}
	}
}
