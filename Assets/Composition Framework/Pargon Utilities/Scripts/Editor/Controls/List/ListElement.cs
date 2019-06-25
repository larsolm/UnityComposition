using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.PargonUtilities.Editor
{
	public class ListElement : VisualElement
	{
		private const string _uxmlPath = Utilities.AssetPath + "Controls/List/List.uxml";
		private const string _ussPath = Utilities.AssetPath + "Controls/List/List.uss";

		private const string _ussLabelName = "label";
		private const string _ussAddButtonName = "add-button";
		private const string _ussContainerName = "container";

		private const string _ussBaseClass = "pargon-list";
		private const string _ussEmptyClass = "empty-label";
		private const string _ussItemClass = "item";
		private const string _ussItemOddClass = "odd";
		private const string _ussItemEvenClass = "even";
		private const string _ussItemDragClass = "drag";
		private const string _ussItemContainerClass = "item-container";
		private const string _ussDragHandleClass = "drag-handle";
		private const string _ussRemoveButtonClass = "remove-button";

		private static readonly Icon _dragIcon = Icon.BuiltIn("animationnocurve");

		private readonly bool _allowAdd;
		private readonly bool _allowRemove;
		private readonly bool _allowMove;
		private readonly string _emptyLabel;

		private readonly int _addButtonIndex;

		private readonly ListProxy _proxy;
		private readonly VisualElement _itemContainer;

		public event Action OnItemAdded;
		public event Action<int> OnItemRemoved;
		public event Action<int, int> OnItemMoved;

		private int _dragFromIndex = -1;
		private int _dragToIndex = -1;
		private VisualElement _dragElement;

		public ListElement(ListProxy proxy, string label, string tooltip, bool allowAdd = true, bool allowRemove = true, bool allowMove = true, string emptyLabel = "List is empty")
		{
			ElementHelper.AddVisualTree(this, _uxmlPath);
			ElementHelper.AddStyleSheet(this, _ussPath);

			AddToClassList(_ussBaseClass);

			_allowAdd = allowAdd;
			_allowRemove = allowRemove;
			_allowMove = allowMove;
			_emptyLabel = emptyLabel;

			_proxy = proxy;
			_itemContainer = this.Q(_ussContainerName);

			var text = this.Q<TextElement>(_ussLabelName);
			text.text = label;
			text.tooltip = tooltip;

			var addButton = this.Q<Image>(_ussAddButtonName);
			addButton.tooltip = "Add an item to this list";
			addButton.image = Icon.Add.Content;
			addButton.AddManipulator(new Clickable(AddItem));

			ElementHelper.SetVisible(addButton, _allowAdd);

			_addButtonIndex = IndexOf(addButton);

			RegisterCallback<MouseMoveEvent>(UpdateDrag);
			RegisterCallback<MouseUpEvent>(StopDrag);

			Rebuild();
		}

		public VisualElement AddHeaderButton(Texture image, string tooltip, Action action)
		{
			var button = ElementHelper.CreateIconButton(image, tooltip, action);
			Insert(_addButtonIndex, button);
			return button;
		}

		public void Rebuild()
		{
			_itemContainer.Clear();

			if (_proxy.Count == 0)
			{
				var empty = new Label(_emptyLabel);
				empty.AddToClassList(_ussEmptyClass);

				_itemContainer.Add(empty);
			}
			else
			{
				for (var i = 0; i < _proxy.Count; i++)
				{
					var item = new VisualElement();
					item.AddToClassList(_ussItemClass);
					item.AddToClassList(i % 2 == 0 ? _ussItemEvenClass : _ussItemOddClass);

					var container = new VisualElement();
					container.AddToClassList(_ussItemContainerClass);

					var element = _proxy.CreateElement(i);

					var dragHandle = new Image { image = _dragIcon.Content };
					dragHandle.AddToClassList(_ussDragHandleClass);
					dragHandle.RegisterCallback((MouseDownEvent e) => StartDrag(e, i));

					ElementHelper.SetVisible(dragHandle, _allowMove);

					var removeButton = ElementHelper.CreateIconButton(Icon.Remove.Content, "Remove this item from the list", () => RemoveItem(i));
					removeButton.AddToClassList(_ussRemoveButtonClass);

					ElementHelper.SetVisible(removeButton, _allowRemove);

					container.Add(element);
					item.Add(dragHandle);
					item.Add(container);
					item.Add(removeButton);
					_itemContainer.Add(item);
				}
			}
		}

		private void AddItem()
		{
			_proxy.AddItem();
			OnItemAdded?.Invoke();
			Rebuild();
		}

		private void RemoveItem(int index)
		{
			_proxy.RemoveItem(index);
			OnItemRemoved?.Invoke(index);
			Rebuild();
		}

		private void MoveItem(int from, int to)
		{
			_proxy.MoveItem(from, to);
			OnItemMoved?.Invoke(from, to);
			Rebuild();
		}

		#region Dragging

		private void StartDrag(MouseDownEvent evt, int index)
		{
			_dragFromIndex = index;
			_dragToIndex = index;
			_dragElement = _itemContainer.ElementAt(index);

			_dragElement.AddToClassList(_ussItemDragClass);
			this.CaptureMouse();
		}

		private void UpdateDrag(MouseMoveEvent e)
		{
			if (_dragElement != null)
			{
				var y = e.localMousePosition.y - _itemContainer.localBound.y;

				var nextIndex = -1;
				VisualElement nextElement = null;

				for (var i = 0; i < _itemContainer.childCount; i++)
				{
					if (y < _itemContainer.ElementAt(i).localBound.center.y)
					{
						nextIndex = i;
						nextElement = _itemContainer.ElementAt(i);
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
						_dragToIndex = _itemContainer.childCount - 1;
						_dragElement.BringToFront();
					}
				}
			}
		}

		private void StopDrag(MouseUpEvent e)
		{
			this.ReleaseMouse();

			if (_dragElement != null)
				_dragElement.RemoveFromClassList(_ussItemDragClass);

			if (_dragFromIndex != _dragToIndex)
				MoveItem(_dragFromIndex, _dragToIndex);

			_dragElement = null;
			_dragFromIndex = -1;
			_dragToIndex = -1;
		}

		#endregion
	}
}