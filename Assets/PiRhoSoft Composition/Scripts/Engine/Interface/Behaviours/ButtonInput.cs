using PiRhoSoft.UtilityEngine;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PiRhoSoft.CompositionEngine
{
	[HelpURL(Composition.DocumentationUrl + "button-input")]
	[AddComponentMenu("Composition/Interface/Button Input")]
	public class ButtonInput : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
	{
		[Tooltip("The name of the button to set when this is pressed")]
		public string ButtonName;

		public void OnPointerDown(PointerEventData eventData)
		{
			InputHelper.SetButton(ButtonName, true);
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			InputHelper.SetButton(ButtonName, false);
		}
	}
}
