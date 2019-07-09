using PiRhoSoft.Utilities.Editor;
using PiRhoSoft.Utilities.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	public class AutocompletePopup : VisualElement
	{
		private string _containerName = "pargon-autocomplete-container";
		private string _itemClass = "pargon-autocomplete-item";
		private string _itemActiveClass = "pargon-autocomplete-item-active";
		private string _itemValidClass = "pargon-autocomplete-item-valid";
		private string _itemInvalidClass = "pargon-autocomplete-item-invalid";

		private const string _uxmlPath = Utilities.AssetPath + "Controls/Autocomplete/AutocompletePopup.uxml";
		private const string _styleSheetPath = Utilities.AssetPath + "Autocomplete/AutocompletePopup.uss";

		private List<string> _items;
		private VisualElement _container;
		private AutocompleteWindow _window;
		private string _filter;
		private int _activeItem = -1;

		public void Setup(AutocompleteSource source)
		{
			_items = source.Items.Select(item => item.Name).OrderBy(item => item).ToList();

			ElementHelper.AddStyleSheet(this, _styleSheetPath);
			ElementHelper.AddVisualTree(this, _uxmlPath);

			_container = this.Query(_containerName);

			foreach (var item in _items)
			{
				var element = new ItemElement(item);
				element.AddToClassList(_itemClass);
				_container.Add(element);
			}
		}

		public void Show(Rect location, string value)
		{
			var position = GUIUtility.GUIToScreenPoint(location.position);

			_window = ScriptableObject.CreateInstance<AutocompleteWindow>();
			_window.rootVisualElement.Add(this);
			_window.ShowPopup();
			_window.position = new Rect(position + new Vector2(0, 18), new Vector2(location.width, 200));

			Filter(value);
		}

		public void Hide()
		{
			_window.rootVisualElement.Remove(this);
			_window.Close();
			_window = null;
		}

		public void Filter(string value)
		{
			if (_filter != value)
			{
				var activeItem = -1;

				for (var i = 0; i < _container.childCount; i++)
				{
					var element = _container.ElementAt(i);
					var isValid = (element as ItemElement).Item.IndexOf(value, StringComparison.CurrentCultureIgnoreCase) >= 0;

					if (isValid && activeItem < 0)
						activeItem = i;

					ElementHelper.AlternateClass(element, _itemValidClass, _itemInvalidClass, isValid);
				}

				_filter = value;
				SetActive(activeItem);
			}
		}

		public void Previous()
		{
			if (_activeItem > 0)
				SetActive(_activeItem - 1);
		}

		public void Next()
		{
			SetActive(_activeItem + 1);
		}

		public void SetActive(int index)
		{
			if (_activeItem >= 0)
				ElementHelper.ToggleClass(_container.ElementAt(_activeItem), _itemActiveClass, false);

			_activeItem = Mathf.Clamp(index, -1, _container.childCount - 1);

			if (_activeItem >= 0)
			{
				ElementHelper.ToggleClass(_container.ElementAt(_activeItem), _itemActiveClass, true);
				(_container as ScrollView).ScrollTo(_container.ElementAt(_activeItem));
			}
		}

		public string Select()
		{
			return _activeItem >= 0 ? (_container.ElementAt(_activeItem) as ItemElement).Item:  "";
		}

		private class ItemElement : TextElement
		{
			public string Item;

			public ItemElement(string item)
			{
				Item = item;
				text = item;
			}
		}
	}
}