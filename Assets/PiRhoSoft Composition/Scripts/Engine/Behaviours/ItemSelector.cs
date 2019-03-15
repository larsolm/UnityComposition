using UnityEngine;
using UnityEngine.EventSystems;

namespace PiRhoSoft.CompositionEngine
{
	[DisallowMultipleComponent]
	[HelpURL(Composition.DocumentationUrl + "item-selector")]
	[AddComponentMenu("PiRho Soft/Interface/Item Selector")]
	public class ItemSelector : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
	{
		public SelectionControl Selection { get; internal set; }
		public int Index { get; internal set; }

		public void OnPointerEnter(PointerEventData eventData)
		{
			if (Selection)
				Selection.MoveFocus(Index);
		}

		public void OnPointerExit(PointerEventData eventData)
		{
		}

		public void OnPointerDown(PointerEventData eventData)
		{
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			if (Selection)
				Selection.SelectItem(Index);
		}
	}
}
