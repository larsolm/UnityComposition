using PiRhoSoft.PargonUtilities.Engine;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PiRhoSoft.CompositionEngine
{
	[HelpURL(Composition.DocumentationUrl + "button-input")]
	[AddComponentMenu("PiRho Soft/Interface/Button Input")]
	public class ButtonInput : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
	{
		[Tooltip("The name of the button to set when this is pressed")]
		public string ButtonName;

		private bool _isPressed = false;

		public void OnPointerDown(PointerEventData eventData)
		{
			_isPressed = true;
			InputHelper.SetButton(ButtonName, true);
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			_isPressed = false;
			InputHelper.SetButton(ButtonName, false);
		}

		private void OnDisable()
		{
			// if gameObject is disabled from the down event then the up event won't be called
			if (_isPressed)
				InputHelper.SetButton(ButtonName, false);

			_isPressed = false;
		}
	}
}
