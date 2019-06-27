using System;
using UnityEngine.UIElements;

namespace PiRhoSoft.PargonUtilities.Editor
{
	public class DictionaryElement : VisualElement
	{
		private const string _uxmlPath = Utilities.AssetPath + "Controls/Dictionary/Dictionary.uxml";
		private const string _ussPath = Utilities.AssetPath + "Controls/Dictionary/Dictionary.uss";

		private const string _ussLabelName = "dictionary-label";
		private const string _ussAddButtonName = "dictionary-add-button";
		private const string _ussContainerName = "dictionary-container";

		private const string _ussBaseClass = "pargon-dictionary";
		private const string _ussEmptyClass = "empty-label";
		private const string _ussAddKeyClass = "add-key";
		private const string _ussItemClass = "item";
		private const string _ussItemOddClass = "odd";
		private const string _ussItemEvenClass = "even";
		private const string _ussItemContainerClass = "item-container";
		private const string _ussKeyClass = "key";
		private const string _ussValueClass = "value";
		private const string _ussRemoveButtonClass = "remove-item";

		public const string UssInvalidKeyClass = "invalid-key";

		private readonly bool _allowAdd;
		private readonly bool _allowRemove;
		private readonly string _emptyLabel;

		private readonly int _addButtonIndex;

		private readonly DictionaryProxy _proxy;
		private readonly VisualElement _itemContainer;

		public event Action OnItemAdded;
		public event Action<int> OnItemRemoved;

		public DictionaryElement(DictionaryProxy proxy, string label, string tooltip, bool allowAdd = true, bool allowRemove = true, string emptyLabel = "List is empty")
		{
			ElementHelper.AddVisualTree(this, _uxmlPath);
			ElementHelper.AddStyleSheet(this, _ussPath);

			AddToClassList(_ussBaseClass);

			_allowAdd = allowAdd;
			_allowRemove = allowRemove;
			_emptyLabel = emptyLabel;

			_proxy = proxy;
			_itemContainer = this.Q(_ussContainerName);

			var text = this.Q<Label>(_ussLabelName);
			text.text = label;
			text.tooltip = tooltip;

			var addButton = this.Q<Image>(_ussAddButtonName);
			addButton.image = Icon.Add.Content;
			addButton.AddManipulator(new Clickable(AddItem));

			ElementHelper.SetVisible(addButton, _allowAdd);

			var keyElement = _proxy.CreateAddElement();
			keyElement.AddToClassList(_ussAddKeyClass);
			Insert(IndexOf(addButton), keyElement);

			Rebuild();
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

					var key = _proxy.CreateKeyElement(i);
					key.AddToClassList(_ussKeyClass);

					var value = _proxy.CreateKeyElement(i);
					value.AddToClassList(_ussValueClass);

					var removeButton = new Image() { image = Icon.Remove.Content };
					removeButton.AddToClassList(_ussRemoveButtonClass);
					removeButton.AddManipulator(new Clickable(() => RemoveItem(i)));

					ElementHelper.SetVisible(removeButton, _allowRemove);

					container.Add(key);
					container.Add(value);

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
	}
}