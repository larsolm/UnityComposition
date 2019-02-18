using UnityEngine;
using UnityEngine.EventSystems;

namespace PiRhoSoft.CompositionEngine
{
	[DisallowMultipleComponent]
	[HelpURL(Composition.DocumentationUrl + "item-selector")]
	[AddComponentMenu("Composition/Interface/Item Selector")]
	public class ItemSelector : MonoBehaviour, IPointerUpHandler
	{
		public SelectionControl Selection { get; internal set; }
		public int Index { get; internal set; }

		public void OnPointerUp(PointerEventData eventData)
		{
			if (Selection)
				Selection.SelectItem(Index);
		}
	}
}
