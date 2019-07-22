using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	public class DictionaryControl : VisualElement
	{
		private const string _stylesheet = "Dictionary/DictionaryStyle.uss";

		#region Class Names

		public static readonly string UssClassName = "pirho-list";
		public static readonly string EmptyUssClassName = UssClassName + "--empty";
		public static readonly string AddDisabledUssClassName = UssClassName + "--add-disabled";
		public static readonly string RemoveDisabledUssClassName = UssClassName + "--remove-disabled";
		public static readonly string EmptyLabelUssClassName = UssClassName + "__empty-label";
		public static readonly string HeaderUssClassName = UssClassName + "__header";
		public static readonly string LabelUssClassName = UssClassName + "__label";
		public static readonly string ContentUssClassName = UssClassName + "__content";
		public static readonly string FooterUssClassName = UssClassName + "__footer";
		public static readonly string ItemsUssClassName = UssClassName + "__items";
		public static readonly string HeaderKeyTextUssClassName = HeaderUssClassName + "__key-text";
		public static readonly string HeaderButtonsUssClassName = HeaderUssClassName + "__buttons";
		public static readonly string FooterButtonsUssClassName = FooterUssClassName + "__buttons";
		public static readonly string HeaderButtonUssClassName = HeaderUssClassName + "__button";
		public static readonly string FooterButtonUssClassName = FooterUssClassName + "__button";
		public static readonly string AddButtonUssClassName = UssClassName + "__add-button";
		public static readonly string RemoveButtonUssClassName = UssClassName + "__remove-button";
		public static readonly string ItemUssClassName = UssClassName + "__item";
		public static readonly string ItemEvenUssClassName = ItemUssClassName + "--even";
		public static readonly string ItemOddUssClassName = ItemUssClassName + "--odd";
		public static readonly string ItemButtonsUssClassName = ItemUssClassName + "__buttons";
		public static readonly string ItemButtonUssClassName = ItemUssClassName + "__button";
		public static readonly string ItemContentUssClassName = ItemUssClassName + "__content";

		#endregion

		#region Icons

		private static readonly Icon _addIcon = Icon.Add;
		private static readonly Icon _removeIcon = Icon.Remove;

		#endregion

		public IDictionaryProxy Proxy { get; private set; }

		private VisualElement _headerButtonsContainer;
		private VisualElement _footerButtonsContainer;
		private VisualElement _itemsContainer;
		private TextField _keyField;
		private UQueryState<VisualElement> _itemContentQuery;
		private UQueryState<VisualElement> _itemButtonsQuery;

		private List<ItemButton> _itemButtons = new List<ItemButton>();

		public DictionaryControl(IDictionaryProxy proxy)
		{
			Proxy = proxy;

			CreateFrame();
			AddHeaderButton(_addIcon.Texture, proxy.AddTooltip, AddButtonUssClassName, AddItem);
			AddItemButton(_removeIcon.Texture, proxy.RemoveTooltip, RemoveButtonUssClassName, RemoveItem);
			Refresh();

			AddToClassList(UssClassName);
			this.AddStyleSheet(_stylesheet);
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
		}

		#region Element Creation

		public void AddHeaderButton(Texture icon, string tooltip, string ussClassName, Action action)
		{
			var button = new IconButton(icon, tooltip, action);
			button.AddToClassList(HeaderButtonUssClassName);

			if (!string.IsNullOrEmpty(ussClassName))
				button.AddToClassList(ussClassName);

			_headerButtonsContainer.Add(button);
		}

		public void AddFooterButton(Texture icon, string tooltip, string ussClassName, Action action)
		{
			var button = new IconButton(icon, tooltip, action);
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

			_keyField = new TextField();
			_keyField.AddToClassList(HeaderKeyTextUssClassName);
			header.Add(_keyField);

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

			var content = Proxy.CreateField(index);
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
				var content = Proxy.CreateField(index);
				content.AddToClassList(ItemContentUssClassName);
				item.RemoveAt(1);
				item.Insert(1, content);
			}
		}

		private void CreateItemButton(VisualElement container, ItemButton itemButton)
		{
			var button = new IconButton(itemButton.Icon, itemButton.Tooltip, () => itemButton.Action(GetItemIndex(container.parent)));

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
			var key = _keyField.text;

			if (Proxy.AllowAdd && !string.IsNullOrEmpty(key)) // && isn't a duplicate
			{
				Proxy.AddItem(key);
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

		#endregion
	}
}