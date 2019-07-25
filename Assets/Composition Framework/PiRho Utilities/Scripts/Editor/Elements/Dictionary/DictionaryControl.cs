using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	public class DictionaryControl : Frame
	{
		private const string _stylesheet = "Dictionary/DictionaryStyle.uss";

		#region Class Names

		public new static readonly string UssClassName = "pirho-dictionary";
		public static readonly string EmptyUssClassName = UssClassName + "--empty";
		public static readonly string AddDisabledUssClassName = UssClassName + "--add-disabled";
		public static readonly string RemoveDisabledUssClassName = UssClassName + "--remove-disabled";
		public static readonly string AddKeyInvalidUssClassName = UssClassName + "--add-key-invalid";
		public static readonly string EmptyLabelUssClassName = UssClassName + "__empty-label";
		public static readonly string ItemsUssClassName = UssClassName + "__items";
		public static readonly string HeaderKeyTextUssClassName = UssClassName + "__key-text";
		public static readonly string AddButtonUssClassName = UssClassName + "__add-button";
		public static readonly string RemoveButtonUssClassName = UssClassName + "__remove-button";
		public static readonly string ItemUssClassName = UssClassName + "__item";
		public static readonly string ItemEvenUssClassName = ItemUssClassName + "--even";
		public static readonly string ItemOddUssClassName = ItemUssClassName + "--odd";
		public static readonly string ItemContentUssClassName = ItemUssClassName + "__content";

		#endregion

		#region Icons

		private static readonly Icon _addIcon = Icon.Add;
		private static readonly Icon _removeIcon = Icon.Remove;

		#endregion

		public IDictionaryProxy Proxy { get; private set; }

		private VisualElement _itemsContainer;
		private TextField _keyField;
		private IconButton _addButton;

		public DictionaryControl(IDictionaryProxy proxy)
		{
			Proxy = proxy;

			CreateFrame();

			_addButton = AddHeaderButton(_addIcon.Texture, proxy.AddTooltip, AddButtonUssClassName, AddItem);
			_addButton.SetEnabled(false);

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
			EnableInClassList(AddDisabledUssClassName, !Proxy.AllowAdd);
			EnableInClassList(RemoveDisabledUssClassName, !Proxy.AllowRemove);
		}

		#region Element Creation

		private void CreateFrame()
		{
			SetLabel(Proxy.Label, Proxy.Tooltip);

			_keyField = new TextField();
			_keyField.AddToClassList(HeaderKeyTextUssClassName);
			_keyField.RegisterValueChangedCallback(evt => AddKeyChanged(evt.newValue));
			_keyField.Q(TextField.textInputUssName).RegisterCallback<KeyDownEvent>(evt => KeyPressed(evt));

			var keyPlaceholder = new PlaceholderControl(Proxy.AddPlaceholder);
			keyPlaceholder.AddToField(_keyField);

			Header.Add(_keyField);
			_keyField.PlaceInFront(Label);

			var empty = new Label(Proxy.EmptyLabel) { tooltip = Proxy.EmptyTooltip };
			empty.AddToClassList(EmptyLabelUssClassName);
			Content.Add(empty);

			_itemsContainer = new VisualElement();
			_itemsContainer.AddToClassList(ItemsUssClassName);
			Content.Add(_itemsContainer);
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

			var remove = new IconButton(_removeIcon.Texture, Proxy.RemoveTooltip, () => RemoveItem(index));
			remove.AddToClassList(RemoveButtonUssClassName);
			item.Add(remove);
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
				item.RemoveAt(0);
				item.Insert(0, content);
			}
		}

		private void KeyPressed(KeyDownEvent evt)
		{
			if (evt.keyCode == KeyCode.KeypadEnter || evt.keyCode == KeyCode.Return)
			{
				AddItem();
				evt.StopPropagation();
				evt.PreventDefault();
			}
		}

		private void AddKeyChanged(string newValue)
		{
			// Separately check for empty because we don't want empty to be addable but we also don't want it be show as invalid
			var valid = IsKeyValid(newValue) || string.IsNullOrEmpty(newValue);
			EnableInClassList(AddKeyInvalidUssClassName, !valid);
			_addButton.SetEnabled(valid && !string.IsNullOrEmpty(newValue));
		}

		private bool IsKeyValid(string key)
		{
			return !string.IsNullOrEmpty(key) && Proxy.IsKeyValid(key);
		}

		#endregion

		#region Item Management

		private void AddItem()
		{
			var key = _keyField.text;

			if (Proxy.AllowAdd && IsKeyValid(key))
			{
				Proxy.AddItem(key);
				_keyField.value = string.Empty;
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