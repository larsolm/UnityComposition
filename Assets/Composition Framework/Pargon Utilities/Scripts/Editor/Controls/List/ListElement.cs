using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UIElements;

namespace PiRhoSoft.PargonUtilities.Editor
{
	// USE CASES
	// as a uxml element (lookup by id, bind to property, value, or callbacks)
	// as a uxml element with binding path (bind to property)
	// as a property drawer (bind to property)
	// an an instance in another element or editor (bind to property, value, or callbacks)

	public class ListElement : VisualElement
	{
		private const string _uxmlPath = "Assets/PargonUtilities/Scripts/Editor/Controls/List/List.uxml";
		private const string _ussPath = "Assets/PargonUtilities/Scripts/Editor/Controls/List/List.uss";

		private const string _labelName = "list-label";
		private const string _addButtonName = "list-add-button";
		private const string _containerName = "list-container";

		private const string _itemClass = "pargon-list-item";
		private const string _itemOddClass = "pargon-list-item-odd";
		private const string _itemEvenClass = "pargon-list-item-even";
		private const string _itemDragClass = "pargon-list-item-drag";
		private const string _itemContainerClass = "pargon-list-item-container";
		private const string _dragHandleClass = "pargon-list-drag-handle";
		private const string _removeButtonClass = "pargon-list-remove-item";

		private const int _dragUpdateTime = 0;

		public bool AllowAdd = true;
		public bool AllowRemove = true;
		public bool AllowMove = true;

		public event Action OnItemAdded;
		public event Action<int> OnItemRemoved;
		public event Action<int, int> OnItemMoved;

		private int _dragFromIndex = -1;
		private int _dragToIndex = -1;
		private VisualElement _dragElement;

		public ListProxy Proxy { get; private set; }
		protected VisualElement ItemContainer { get; private set; }

		public void Setup(string label, ListProxy proxy)
		{
			ElementHelper.AddVisualTree(this, _uxmlPath);
			ElementHelper.AddStyleSheet(this, _ussPath);

			Proxy = proxy;
			ItemContainer = this.Q(_containerName);

			var text = this.Q<TextElement>(_labelName);
			text.text = label;

			var addButton = this.Q<Button>(_addButtonName);
			addButton.clickable.clicked += () => AddItem();

			RegisterCallback<MouseMoveEvent>(UpdateDrag);
			RegisterCallback<MouseUpEvent>(StopDrag);

			Rebuild();
		}

		public void Setup(SerializedProperty property)
		{
			Setup(property.displayName, new PropertyListProxy(property));
		}

		public void Setup<T>(IList<T> list)
		{
		}

		public void Setup<T>(T[] array)
		{
			AllowAdd = false;
			AllowRemove = false;
		}

		public void Rebuild()
		{
			ItemContainer.Clear();

			for (var i = 0; i < Proxy.Count; i++)
			{
				var index = i;
				var item = new VisualElement();
				item.AddToClassList(_itemClass);
				item.AddToClassList(i % 2 == 0 ? _itemEvenClass : _itemOddClass);

				var container = new VisualElement();
				container.AddToClassList(_itemContainerClass);

				var element = Proxy.CreateElement(i);

				var dragHandle = new TextElement();
				dragHandle.AddToClassList(_dragHandleClass);
				dragHandle.text = "=";
				dragHandle.RegisterCallback((MouseDownEvent e) => StartDrag(e, index));

				var removeButton = new Button();
				removeButton.AddToClassList(_removeButtonClass);
				removeButton.text = "-";
				removeButton.clickable.clicked += () => RemoveItem(index);

				container.Add(element);
				item.Add(dragHandle);
				item.Add(container);
				item.Add(removeButton);
				ItemContainer.Add(item);
			}
		}

		private void StartDrag(MouseDownEvent e, int index)
		{
			_dragFromIndex = index;
			_dragToIndex = index;
			_dragElement = ItemContainer.ElementAt(index);

			_dragElement.AddToClassList(_itemDragClass);
			this.CaptureMouse();
		}

		private void UpdateDrag(MouseMoveEvent e)
		{
			if (_dragElement != null)
			{
				var y = e.localMousePosition.y - ItemContainer.localBound.y;

				var nextIndex = -1;
				VisualElement nextElement = null;

				for (var i = 0; i < ItemContainer.childCount; i++)
				{
					if (y < ItemContainer.ElementAt(i).localBound.center.y)
					{
						nextIndex = i;
						nextElement = ItemContainer.ElementAt(i);
						break;
					}
				}

				if (_dragElement != nextElement)
				{
					if (nextElement != null)
					{
						_dragToIndex = nextIndex > _dragToIndex ? nextIndex - 1 : nextIndex;
						_dragElement.PlaceBehind(nextElement);
					}
					else
					{
						_dragToIndex = ItemContainer.childCount - 1;
						_dragElement.BringToFront();
					}
				}
			}
		}

		private void StopDrag(MouseUpEvent e)
		{
			this.ReleaseMouse();

			if (_dragElement != null)
				_dragElement.RemoveFromClassList(_itemDragClass);

			if (_dragFromIndex != _dragToIndex)
				MoveItem(_dragFromIndex, _dragToIndex);

			_dragElement = null;
			_dragFromIndex = -1;
			_dragToIndex = -1;
		}

		private void AddItem()
		{
			Proxy.AddItem();
			Rebuild();
			OnItemAdded?.Invoke();
		}

		private void RemoveItem(int index)
		{
			Proxy.RemoveItem(index);
			Rebuild();
			OnItemRemoved?.Invoke(index);
		}

		private void MoveItem(int from, int to)
		{
			Proxy.MoveItem(from, to);
			Rebuild();
			OnItemMoved?.Invoke(from, to);
		}

		#region UXML

		private new class UxmlFactory : UxmlFactory<ListElement, UxmlTraits>
		{
		}

		private new class UxmlTraits : VisualElement.UxmlTraits
		{
			private UxmlBoolAttributeDescription _allowAdd = new UxmlBoolAttributeDescription { name = "allow-add", defaultValue = true };
			private UxmlBoolAttributeDescription _allowRemove = new UxmlBoolAttributeDescription { name = "allow-remove", defaultValue = true };
			private UxmlBoolAttributeDescription _allowMove = new UxmlBoolAttributeDescription { name = "allow-move", defaultValue = true };

			public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
			{
				get { yield break; }
			}

			public override void Init(VisualElement element, IUxmlAttributes bag, CreationContext context)
			{
				base.Init(element, bag, context);

				var list = element as ListElement;
				list.AllowAdd = _allowAdd.GetValueFromBag(bag, context);
				list.AllowRemove = _allowRemove.GetValueFromBag(bag, context);
				list.AllowMove = _allowMove.GetValueFromBag(bag, context);
			}
		}

		#endregion
	}
}