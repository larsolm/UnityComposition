using PiRhoSoft.Utilities.Engine;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PiRhoSoft.Composition.Engine
{
	[HelpURL(Composition.DocumentationUrl + "axis-input")]
	[AddComponentMenu("PiRho Soft/Interface/Axis Input")]
	public class AxisInput : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
	{
		[Tooltip("The name of the axis to set when this is pressed")]
		public string AxisName;

		[Tooltip("The value to set the axis at when this is pressed")]
		public float AxisValue;

		public void OnPointerDown(PointerEventData eventData)
		{
			InputHelper.SetAxis(AxisName, AxisValue);
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			InputHelper.SetAxis(AxisName, 0.0f);
		}
	}
}
