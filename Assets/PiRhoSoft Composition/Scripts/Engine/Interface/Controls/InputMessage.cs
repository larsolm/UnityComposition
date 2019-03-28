using PiRhoSoft.UtilityEngine;
using System.Collections;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[HelpURL(Composition.DocumentationUrl + "input-message")]
	[AddComponentMenu("PiRho Soft/Interface/Input Message")]
	public class InputMessage : MessageControl
	{
		[Tooltip("The input button to use to complete the text")]
		public string AcceptButton = "Submit";

		protected override IEnumerator Run()
		{
			while (!InputHelper.GetWasButtonPressed(AcceptButton))
				yield return null;
		}
	}
}
