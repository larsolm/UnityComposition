using UnityEngine;
using UnityEngine.EventSystems;

namespace PiRhoSoft.Composition
{
	[HelpURL(Configuration.DocumentationUrl + "click-graph-trigger")]
	[AddComponentMenu("PiRho Soft/Composition/Click Graph Trigger")]
	public class ClickGraphTrigger : GraphTrigger, IPointerDownHandler, IPointerUpHandler
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
