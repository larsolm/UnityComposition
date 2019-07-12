using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.PargonUtilities.Editor
{
	public class ListControl : VisualElement
	{
		private const string _stylesheet = Utilities.AssetPath + "Controls/List/ListControl.uss";

		#region Class Names

		public static readonly string UssClassName = "pirho-list";
		public static readonly string EmptyUssClassName = UssClassName + "--empty";
		public static readonly string AddDisabledUssClassName = UssClassName + "--add-disabled";
		public static readonly string RemoveDisabledUssClassName = UssClassName + "--remove-disabled";
		public static readonly string MoveDisabledUssClassName = UssClassName + "--move-disabled";
		public static readonly string EmptyLabelUssClassName = UssClassName + "__empty-label";
		public static readonly string HeaderUssClassName = UssClassName + "__header";
		public static readonly string LabelUssClassName = UssClassName + "__label";
		public static readonly string ContentUssClassName = UssClassName + "__content";
		public static readonly string FooterUssClassName = UssClassName + "__footer";
		public static readonly string ItemsUssClassName = UssClassName + "__items";
		public static readonly string HeaderButtonsUssClassName = HeaderUssClassName + "__buttons";
		public static readonly string FooterButtonsUssClassName = FooterUssClassName + "__buttons";
		public static readonly string HeaderButtonUssClassName = HeaderUssClassName + "__button";
		public static readonly string FooterButtonUssClassName = FooterUssClassName + "__button";
		public static readonly string AddButtonUssClassName = UssClassName + "__add-button";
		public static readonly string RemoveButtonUssClassName = UssClassName + "__remove-button";
		public static readonly string DragHandleUssClassName = UssClassName + "__drag-handle";
		public static readonly string DragPlaceholderUssClassName = UssClassName + "__drag-placeholder";
		public static readonly string ItemUssClassName = UssClassName + "__item";
		public static readonly string ItemDraggingUssClassName = ItemUssClassName + "--dragging";
		public static readonly string ItemEvenUssClassName = ItemUssClassName + "--even";
		public static readonly string ItemOddUssClassName = ItemUssClassName + "--odd";
		public static readonly string ItemButtonsUssClassName = ItemUssClassName + "__buttons";
		public static readonly string ItemButtonUssClassName = ItemUssClassName + "__button";
		public static readonly string ItemContentUssClassName = ItemUssClassName + "__content";

		#endregion

		#region Icons

		private static readonly Icon _addIcon = Icon.Add;
		private static readonly Icon _removeIcon = Icon.Remove;
		private static readonly Icon _dragIcon = Icon.BuiltIn("animationnocurve");

		#endregion

		public IListProxy Proxy { get; private set; }

		private VisualElement _headerButtonsContainer;
		private VisualElement _footerButtonsContainer;
		private VisualElement _itemsContainer;
		private UQueryState<VisualElement> _itemContentQuery;
		private UQueryState<VisualElement> _itemButtonsQuery;

		private List<ItemButton> _itemButtons = new List<ItemButton>();

		private int _dragFromIndex = -1;
		private int _dragToIndex = -1;
		private VisualElement _dragElement;
		private VisualElement _dragPlaceholder;

		public ListControl(IListProxy proxy)
		{
			Proxy = proxy;

			CreateFrame();
			AddHeaderButton(_addIcon.Content, proxy.AddTooltip, AddButtonUssClassName, AddItem);
			AddItemButton(_removeIcon.Content, proxy.RemoveTooltip, RemoveButtonUssClassName, RemoveItem);
			SetupDragging();
			Refresh();

			AddToClassList(UssClassName);
			ElementHelper.AddStyleSheet(this, _stylesheet);
		}

		public void Refresh()
		{
			while (_itemsContainer.childCount > Proxy.ItemCount)
				_itemsContainer.RemoveAt(_itemsContainer.childCount - 1);

			for (var i = 0; i < _itemsContainer.childCount; i++)
				UpdateItem(i);

			for (var i = _itemsContainer.childCount; i < Proxy.ItemCount; i++)
				CreateItem(i);

			EnableInClassList(EmptyUssClassName, Proxy.ItemCount == 0);
			EnableInClassList(AddDisabledUssClassName, Proxy.AllowAdd);
			EnableInClassList(RemoveDisabledUssClassName, Proxy.AllowRemove);
			EnableInClassList(MoveDisabledUssClassName, Proxy.AllowReorder);
		}

		#region Element Creation

		public void AddHeaderButton(Texture icon, string tooltip, string ussClassName, Action action)
		{
			var button = ElementHelper.CreateIconButton(icon, tooltip, action);
			button.AddToClassList(HeaderButtonUssClassName);

			if (!string.IsNullOrEmpty(ussClassName))
				button.AddToClassList(ussClassName);

			_headerButtonsContainer.Add(button);
		}

		public void AddFooterButton(Texture icon, string tooltip, string ussClassName, Action action)
		{
			var button = ElementHelper.CreateIconButton(icon, tooltip, action);
			button.AddToClassList(FooterButtonUssClassName);

			if (!string.IsNullOrEmpty(ussClassName))
				button.AddToClassList(ussClassName);

			_footerButtonsContainer.Add(button);
		}

		private struct ItemButton
		{
			public Texture Icon;
			public string Tooltip;
			public string UssClassName;
			public Action<int> Action;
		}

		public void AddItemButton(Texture icon, string tooltip, string ussClassName, Action<int> action)
		{
			var button = new ItemButton
			{
				Icon = icon,
				Tooltip = tooltip,
				UssClassName = ussClassName,
				Action = action
			};

			_itemButtons.Add(button);
			_itemButtonsQuery.ForEach(buttons => CreateItemButton(buttons, button));
		}

		private void CreateFrame()
		{
			var header = new VisualElement();
			header.AddToClassList(HeaderUssClassName);
			Add(header);

			var label = new Label(Proxy.Label) { tooltip = Proxy.Tooltip };
			label.AddToClassList(LabelUssClassName);
			header.Add(label);

			_headerButtonsContainer = new VisualElement();
			_headerButtonsContainer.AddToClassList(HeaderButtonsUssClassName);
			header.Add(_headerButtonsContainer);
			
			var content = new VisualElement();
			content.AddToClassList(ContentUssClassName);
			Add(content);

			var empty = new Label(Proxy.EmptyLabel) { tooltip = Proxy.EmptyTooltip };
			empty.AddToClassList(EmptyLabelUssClassName);
			content.Add(empty);

			_itemsContainer = new VisualElement();
			_itemsContainer.AddToClassList(ItemsUssClassName);
			content.Add(_itemsContainer);

			var footer = new VisualElement();
			footer.AddToClassList(FooterUssClassName);
			Add(footer);

			_footerButtonsContainer = new VisualElement();
			_footerButtonsContainer.AddToClassList(FooterButtonsUssClassName);
			footer.Add(_footerButtonsContainer);

			_itemContentQuery = this.Query(null, ItemContentUssClassName).Build();
			_itemButtonsQuery = this.Query(null, ItemButtonsUssClassName).Build();
		}

		private void CreateItem(int index)
		{
			var item = new VisualElement();
			item.AddToClassList(ItemUssClassName);
			item.AddToClassList(index % 2 == 0 ? ItemEvenUssClassName : ItemOddUssClassName);
			_itemsContainer.Add(item);

			var dragHandle = new Image { image = _dragIcon.Content, tooltip = "Move this item" };
			dragHandle.AddToClassList(DragHandleUssClassName);
			dragHandle.RegisterCallback((MouseDownEvent e) => StartDrag(e, GetItemIndex(item)));
			item.Add(dragHandle);

			var content = Proxy.CreateItem(index);
			content.AddToClassList(ItemContentUssClassName);
			item.Add(content);

			var buttons = new VisualElement();
			buttons.AddToClassList(ItemButtonsUssClassName);
			item.Add(buttons);

			foreach (var itemButton in _itemButtons)
				CreateItemButton(buttons, itemButton);
		}

		private void UpdateItem(int index)
		{
			var item = _itemsContainer.ElementAt(index);
			item.EnableInClassList(ItemEvenUssClassName, index % 2 == 0);
			item.EnableInClassList(ItemOddUssClassName, index % 2 != 0);

			if (Proxy.NeedsUpdate(item, index))
			{
				var content = Proxy.CreateItem(index);
				content.AddToClassList(ItemContentUssClassName);
				item.RemoveAt(1);
				item.Insert(1, content);
			}
		}

		private void CreateItemButton(VisualElement container, ItemButton itemButton)
		{
			var button = ElementHelper.CreateIconButton(itemButton.Icon, itemButton.Tooltip, () => itemButton.Action(GetItemIndex(container.parent)));

			button.AddToClassList(ItemButtonUssClassName);

			if (!string.IsNullOrEmpty(itemButton.UssClassName))
				button.AddToClassList(itemButton.UssClassName);

			container.Add(button);
		}

		private int GetItemIndex(VisualElement item)
		{
			return item.parent.IndexOf(item);
		}

		#endregion

		#region Item Management

		private void AddItem()
		{
			if (Proxy.AllowAdd)
			{
				Proxy.AddItem();
				Refresh();

				using (var e = ItemAddedEvent.GetPooled(Proxy.ItemCount - 1))
				{
					e.target = this;
					SendEvent(e);
				}
			}
		}

		private void RemoveItem(int index)
		{
			if (Proxy.AllowRemove)
			{
				Proxy.RemoveItem(index);
				Refresh();

				using (var e = ItemRemovedEvent.GetPooled(index))
				{
					e.target = this;
					SendEvent(e);
				}
			}
		}

		private void ReorderItem(int from, int to)
		{
			if (Proxy.AllowReorder)
			{
				var item = _itemsContainer.ElementAt(from);
				_itemsContainer.RemoveAt(from);
				_itemsContainer.Insert(to, item);

				Proxy.ReorderItem(from, to);
				Refresh();

				using (var e = ItemMovedEvent.GetPooled(from, to))
				{
					e.target = this;
					SendEvent(e);
				}
			}
		}

		#endregion

		#region Dragging

		private void SetupDragging()
		{
			RegisterCallback<MouseMoveEvent>(UpdateDrag);
			RegisterCallback<MouseUpEvent>(StopDrag);

			_dragPlaceholder = new VisualElement();
			_dragPlaceholder.AddToClassList(DragPlaceholderUssClassName);
		}

		private void StartDrag(MouseDownEvent e, int index)
		{
			var mousePosition = this.ChangeCoordinatesTo(_itemsContainer, e.localMousePosition);

			_dragFromIndex = index;
			_dragToIndex = index;

			_dragElement = _itemsContainer.ElementAt(index);
			_dragElement.AddToClassList(ItemDraggingUssClassName);
			_dragElement.BringToFront();
			_dragElement.style.left = mousePosition.x;
			_dragElement.style.top = mousePosition.y;

			_itemsContainer.Insert(index, _dragPlaceholder);

			this.CaptureMouse();
		}

		private void UpdateDrag(MouseMoveEvent e)
		{
			if (_dragElement != null)
			{
				var mousePosition = this.ChangeCoordinatesTo(_itemsContainer, e.localMousePosition);

				_dragElement.style.left = mousePosition.x;
				_dragElement.style.top = mousePosition.y;

				var nextIndex = -1;
				VisualElement nextElement = null;

				for (var i = 0; i < _itemsContainer.childCount - 1; i++)
				{
					if (mousePosition.y < _itemsContainer.ElementAt(i).localBound.center.y)
					{
						nextIndex = i;
						nextElement = _itemsContainer.ElementAt(i);
						break;
					}
				}

				if (nextElement != null)
				{
					_dragToIndex = nextIndex > _dragToIndex ? nextIndex - 1 : nextIndex;
					_dragPlaceholder.PlaceBehind(nextElement);
				}
				else
				{
					_dragToIndex = _itemsContainer.childCount - 1;
					_dragPlaceholder.PlaceBehind(_dragElement);
				}
			}
		}

		private void StopDrag(MouseUpEvent e)
		{
			this.ReleaseMouse();

			if (_dragElement != null)
			{
				_dragElement.style.left = 0;
				_dragElement.style.top = 0;
				_dragElement.PlaceBehind(_dragPlaceholder);
				_dragElement.RemoveFromClassList(ItemDraggingUssClassName);
			}

			_dragPlaceholder.RemoveFromHierarchy();

			if (_dragFromIndex != _dragToIndex)
				ReorderItem(_dragFromIndex, _dragToIndex);

			_dragElement = null;
			_dragFromIndex = -1;
			_dragToIndex = -1;
		}

		#endregion
	}

	#region Events

	public class ItemAddedEvent : EventBase<ItemAddedEvent>
	{
		public int Index { get; protected set; }

		public static ItemAddedEvent GetPooled(int index)
		{
			var e = GetPooled();
			e.Index = index;
			return e;
		}

		protected override void Init()
		{
			base.Init();
			Index = -1;
		}
	}

	public class ItemRemovedEvent : EventBase<ItemRemovedEvent>
	{
		public int Index { get; protected set; }

		public static ItemRemovedEvent GetPooled(int index)
		{
			var e = GetPooled();
			e.Index = index;
			return e;
		}

		protected override void Init()
		{
			base.Init();
			Index = -1;
		}
	}

	public class ItemMovedEvent : EventBase<ItemMovedEvent>
	{
		public int FromIndex { get; protected set; }
		public int ToIndex { get; protected set; }

		public static ItemMovedEvent GetPooled(int fromIndex, int toIndex)
		{
			var e = GetPooled();
			e.FromIndex = fromIndex;
			e.ToIndex = toIndex;
			return e;
		}

		protected override void Init()
		{
			base.Init();
			FromIndex = -1;
			ToIndex = -1;
		}
	}

	#endregion
}