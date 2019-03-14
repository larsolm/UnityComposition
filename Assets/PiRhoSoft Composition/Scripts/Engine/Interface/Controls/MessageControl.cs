using PiRhoSoft.UtilityEngine;
using System.Collections;
using TMPro;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	public enum MessageControlDisplay
	{
		None,
		Continue,
		Finished
	}

	public enum MessageInteractionType
	{
		WaitForInput,
		WaitForFinalInput,
		WaitForDisplay,
		DontWait
	}

	[HelpURL(Composition.DocumentationUrl + "message-control")]
	[AddComponentMenu("PiRho Soft/Interface/Message Control")]
	public class MessageControl : InterfaceControl
	{
		[Tooltip("The object that message text will be displayed in")]
		public TextMeshProUGUI DisplayText = null;

		[Tooltip("The object to show when the message is not the last in a sequence")]
		public GameObject ContinueIndicator = null;

		[Tooltip("The object to show when the message is the last in a sequence")]
		public GameObject FinishedIndicator = null;

		[Tooltip("The input button to use to advance the text")]
		public string AcceptButton = "Submit";

		public IEnumerator Show(IVariableStore variables, string text, MessageInteractionType interaction, bool isLast, float wait)
		{
			UpdateBindings(variables, null, null);
			SetInteraction(MessageControlDisplay.None, interaction, isLast);

			if (interaction == MessageInteractionType.DontWait)
			{
				StartCoroutine(DoRun(text, interaction, isLast, wait));
			}
			else
			{
				Activate();

				yield return null;
				yield return Run(text, interaction, isLast);
				yield return null; // Always wait one frame to reset the accept press.

				Deactivate();
			}
		}

		private IEnumerator DoRun(string text, MessageInteractionType interaction, bool isLast, float wait)
		{
			Activate();

			yield return Run(text, interaction, isLast);
			yield return new WaitForSecondsRealtime(wait);

			Deactivate();
		}

		protected virtual IEnumerator Run(string text, MessageInteractionType interaction, bool isLast)
		{
			if (DisplayText != null)
				DisplayText.text = text;

			SetInteraction(MessageControlDisplay.Finished, interaction, isLast);

			while (interaction != MessageInteractionType.DontWait && interaction != MessageInteractionType.WaitForDisplay && !InputHelper.GetWasButtonPressed(AcceptButton))
				yield return null;

			SetInteraction(MessageControlDisplay.None, interaction, isLast);
		}

		protected override void Setup()
		{
			SetInteraction(MessageControlDisplay.None, MessageInteractionType.DontWait, false);

			if (DisplayText != null)
				DisplayText.text = "";
		}

		protected override void Teardown()
		{
			SetInteraction(MessageControlDisplay.None, MessageInteractionType.DontWait, false);

			if (DisplayText != null)
				DisplayText.text = "";
		}

		protected void SetInteraction(MessageControlDisplay display, MessageInteractionType interaction, bool isLast)
		{
			var continuing = display == MessageControlDisplay.Continue && interaction == MessageInteractionType.WaitForInput;
			var finished = display == MessageControlDisplay.Finished && (interaction == MessageInteractionType.WaitForInput || interaction == MessageInteractionType.WaitForFinalInput);

			if (ContinueIndicator)
				ContinueIndicator.SetActive(continuing || (finished && !isLast));

			if (FinishedIndicator)
				FinishedIndicator.SetActive(finished && isLast);
		}
	}
}
