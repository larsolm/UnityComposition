using UnityEngine;
using UnityEngine.EventSystems;

namespace PiRhoSoft.CompositionEngine
{
	[DisallowMultipleComponent]
	[HelpURL(Composition.DocumentationUrl + "item-selector")]
	[AddComponentMenu("PiRho Soft/Interface/Item Selector")]
	public class ItemSelector : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
	{
		private int _index;
		private SelectionControl _selection;

		void Start()
		{
			_selection = GetComponentInParent<SelectionControl>();

			if (_selection)
				_index = _selection.GetIndex(this);
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			if (_selection)
				_selection.MoveFocus(_index);
		}

		public void OnPointerExit(PointerEventData eventData)
		{
		}

		public void OnPointerDown(PointerEventData eventData)
		{
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			if (_selection)
				_selection.SelectItem(_index);
		}
	}
}