using PiRhoSoft.Utilities;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PiRhoSoft.Composition
{
	[HelpURL(Configuration.DocumentationUrl + "click-graph-trigger")]
	[AddComponentMenu("PiRho Composition/Graph Triggers/Click Graph Trigger")]
	public class ClickGraphTrigger : GraphTrigger, IPointerDownHandler, IPointerUpHandler
	{
		public enum ClickState
		{
			OnUp,
			OnDown
		}

		[Tooltip("Whether to execute the graph on pointer down or on pointer up")]
		[EnumButtons]
		public ClickState State = ClickState.OnUp;

		public void OnPointerDown(PointerEventData eventData)
		{
			if (State == ClickState.OnDown)
				Run();
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			if (State == ClickState.OnDown)
				Run();
		}
	}
}
