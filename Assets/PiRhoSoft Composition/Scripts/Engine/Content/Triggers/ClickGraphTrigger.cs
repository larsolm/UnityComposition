using UnityEngine;
using UnityEngine.EventSystems;

namespace PiRhoSoft.CompositionEngine
{
	[HelpURL(Composition.DocumentationUrl + "click-graph-trigger")]
	[AddComponentMenu("PiRho Soft/Composition/Click Graph Trigger")]
	public class ClickGraphTrigger : InstructionTrigger, IPointerDownHandler, IPointerUpHandler
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
