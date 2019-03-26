using UnityEngine;
using UnityEngine.EventSystems;

namespace PiRhoSoft.CompositionEngine
{
	[HelpURL(Composition.DocumentationUrl + "click-trigger")]
	[AddComponentMenu("PiRho Soft/Composition/Click Trigger")]
	public class ClickTrigger : GraphRunner, IPointerDownHandler, IPointerUpHandler
	{
		public void OnPointerDown(PointerEventData eventData)
		{
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			Run();
		}
	}
}
