using System.Collections;
using TMPro;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[HelpURL(Composition.DocumentationUrl + "message-control")]
	[AddComponentMenu("PiRho Soft/Interface/Message Control")]
	public class MessageControl : InterfaceControl
	{
		[Tooltip("The object that message text will be displayed in")]
		public TMP_Text DisplayText = null;

		public bool IsRunning { get; private set; } = false;

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

		protected override void Teardown()
		{
			StopAllCoroutines();
		}

		private IEnumerator Run_()
		{
			IsRunning = true;

			yield return Run();

			IsRunning = false;
		}

		protected virtual IEnumerator Run()
		{
			yield break;
		}
	}
}
