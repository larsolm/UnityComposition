using System;
using System.Collections.Generic;
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

		private const string _ussAddKeyClass = "pargon-dictionary-add-key";
		private const string _ussItemClass = "pargon-dictionary-item";
		private const string _ussItemOddClass = "pargon-dictionary-item-odd";
		private const string _ussItemEvenClass = "pargon-dictionary-item-even";
		private const string _ussItemContainerClass = "pargon-dictionary-item-container";
		private const string _ussKeyClass = "pargon-dictionary-key";
		private const string _ussValueClass = "pargon-dictionary-value";
		private const string _ussRemoveButtonClass = "pargon-dictionary-remove-item";

		public const string UssInvalidKeyClass = "pargon-dictionary-invalid-key";

		public bool AllowAdd = true;
		public bool AllowRemove = true;

		public event Action OnItemAdded;
		public event Action<int> OnItemRemoved;

		public DictionaryProxy Proxy { get; private set; }
		protected VisualElement ItemContainer { get; private set; }

		public DictionaryElement(DictionaryProxy proxy, string label, string tooltip)
		{
			ElementHelper.AddVisualTree(this, _uxmlPath);
			ElementHelper.AddStyleSheet(this, _ussPath);

			Proxy = proxy;
			ItemContainer = this.Q(_ussContainerName);

			var text = this.Q<Label>(_ussLabelName);
			text.text = label;
			text.tooltip = tooltip;

			var addButton = this.Q<Image>(_ussAddButtonName);
			addButton.image = Icon.Add.Content;
			addButton.AddManipulator(new Clickable(AddItem));

			var keyElement = Proxy.CreateAddElement();
			keyElement.AddToClassList(_ussAddKeyClass);
			Insert(IndexOf(addButton), keyElement);

			Rebuild();
		}

		public void Rebuild()
		{
			ItemContainer.Clear();

			for (var i = 0; i < Proxy.Count; i++)
			{
				var item = new VisualElement();
				item.AddToClassList(_ussItemClass);
				item.AddToClassList(i % 2 == 0 ? _ussItemEvenClass : _ussItemOddClass);

				var container = new VisualElement();
				container.AddToClassList(_ussItemContainerClass);

				var key = Proxy.CreateKeyElement(i);
				key.AddToClassList(_ussKeyClass);

				var value = Proxy.CreateKeyElement(i);
				value.AddToClassList(_ussValueClass);

				var removeButton = new Image() { image = Icon.Remove.Content };
				removeButton.AddToClassList(_ussRemoveButtonClass);
				removeButton.AddManipulator(new Clickable(() => RemoveItem(i)));

				container.Add(key);
				container.Add(value);

				item.Add(container);
				item.Add(removeButton);
				ItemContainer.Add(item);
			}
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

		#region UXML

		private new class UxmlFactory : UxmlFactory<DictionaryElement, UxmlTraits> { }

		private new class UxmlTraits : VisualElement.UxmlTraits
		{
			private UxmlBoolAttributeDescription _allowAdd = new UxmlBoolAttributeDescription { name = "allow-add", defaultValue = true };
			private UxmlBoolAttributeDescription _allowRemove = new UxmlBoolAttributeDescription { name = "allow-remove", defaultValue = true };

			public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
			{
				get { yield break; }
			}

			public override void Init(VisualElement element, IUxmlAttributes bag, CreationContext context)
			{
				base.Init(element, bag, context);

				var dictionary = element as DictionaryElement;
				dictionary.AllowAdd = _allowAdd.GetValueFromBag(bag, context);
				dictionary.AllowRemove = _allowRemove.GetValueFromBag(bag, context);
			}
		}

		#endregion
	}
}