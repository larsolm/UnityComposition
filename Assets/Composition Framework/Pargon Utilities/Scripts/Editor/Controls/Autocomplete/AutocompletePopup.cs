using PiRhoSoft.PargonUtilities.Editor;
using PiRhoSoft.PargonUtilities.Engine;
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.PargonUtilities.Editor
{
	public interface IAutocompleteElement
	{
		string GetAutocompleteFilter(TextField input);
		Vector2 GetAutocompleteLocation(TextField input);
		void ApplyAutocomplete(TextField input, KeyCode selector, AutocompleteItem item);
	}

	public class AutocompletePopup : VisualElement
	{
		private readonly static KeyCode[] _defaultOpenKeys = new KeyCode[] { KeyCode.Escape };
		private readonly static KeyCode[] _defaultCloseKeys = new KeyCode[] { KeyCode.Escape, KeyCode.Mouse0 };
		private readonly static KeyCode[] _defaultSelectKeys = new KeyCode[] { KeyCode.Tab, KeyCode.Space, KeyCode.Return, KeyCode.KeypadEnter, KeyCode.Mouse0 };

		private const string _uxmlPath = Utilities.AssetPath + "Controls/Autocomplete/AutocompletePopup.uxml";
		private const string _styleSheetPath = Utilities.AssetPath + "Autocomplete/AutocompletePopup.uss";
		private const string _containerName = "pargon-autocomplete-container";
		private const string _itemClass = "pargon-autocomplete-item";
		private const string _itemActiveClass = "pargon-autocomplete-item-active";
		private const string _itemValidClass = "pargon-autocomplete-item-valid";
		private const string _itemInvalidClass = "pargon-autocomplete-item-invalid";

		private const string _uxmlPath = "Assets/Composition Framework/Pargon Utilities/Scripts/Editor/Controls/Autocomplete/AutocompletePopup.uxml";
		private const string _styleSheetPath = "Assets/Composition Framework/Pargon Utilities/Scripts/Editor/Controls/Autocomplete/AutocompletePopup.uss";

		private IAutocompleteElement _owner;
		private TextField _input;
		private AutocompleteSource _source;
		private AutocompleteWindow _window;
		private VisualElement _container;

		private string _filter = string.Empty;
		private int _activeItem = -1;

		public bool IsOpen => _window != null;

		public KeyCode[] OpenKeys = _defaultOpenKeys;
		public KeyCode[] CloseKeys = _defaultCloseKeys;
		public KeyCode[] SelectKeys = _defaultSelectKeys;

		public AutocompletePopup(IAutocompleteElement owner, TextField input, AutocompleteSource source)
		{
			ElementHelper.AddStyleSheet(this, _styleSheetPath);
			ElementHelper.AddVisualTree(this, _uxmlPath);

			_owner = owner;
			_input = input;
			_source = source;

			_window = null;
			_container = this.Query(_containerName);
			_activeItem = -1;

			foreach (var item in source.Items)
			{
				var element = new ItemElement(item);
				element.AddToClassList(_itemClass);
				_container.Add(element);
			}

			input.ElementAt(0).RegisterCallback<KeyDownEvent>(OnKeyDown);
			input.RegisterValueChangedCallback(OnFilterChanged);
		}

		public void Show()
		{
			if (_window == null)
			{
				var location = _owner.GetAutocompleteLocation(_input);
				var position = GUIUtility.GUIToScreenPoint(location);

				_window = ScriptableObject.CreateInstance<AutocompleteWindow>();
				_window.Show(this, position);

				this.CaptureMouse();
				FocusText(); // showing a popup focuses the popup window no matter what
			}
		}

		public void Hide()
		{
			if (_window != null)
			{
				_window.Hide();
				_window = null;
			}

			this.ReleaseMouse();
		}

		public void FocusText()
		{
			// Scheduled because most of the time this is in response to something else grabbing focus which doesn't
			// necessarily happen right away. Also, the schedule is added to _input so it gets called even when the
			// popup is detached.

			_input.schedule.Execute(() =>
			{
				var window = ElementHelper.GetWindow(_input);
				var cursor = _input.cursorIndex; // focus does a SelectAll for non multiline TextFields

				window.Focus();
				_input.ElementAt(0).Focus();
				_input.SelectRange(cursor, cursor);
			});
		}

		public void Filter()
		{
			var filter = _owner.GetAutocompleteFilter(_input);

			if (_filter != filter)
			{
				var activeItem = -1;

				for (var i = 0; i < _container.childCount; i++)
				{
					var element = _container.ElementAt(i);
					var isValid = (element as ItemElement).Item.Name.IndexOf(filter, StringComparison.CurrentCultureIgnoreCase) >= 0;

					if (isValid && activeItem < 0)
						activeItem = i;

					ElementHelper.AlternateClass(element, _itemValidClass, _itemInvalidClass, isValid);
				}

				_filter = filter;
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

				if (_container is ScrollView scroller)
					scroller.ScrollTo(_container.ElementAt(_activeItem));
			}
		}

		public void Select(KeyCode selector)
		{
			if (_activeItem >= 0)
			{
				var item = (_container.ElementAt(_activeItem) as ItemElement).Item;
				_owner.ApplyAutocomplete(_input, selector, item);
			}
		}

		private class ItemElement : TextElement
		{
			public AutocompleteItem Item;

			public ItemElement(AutocompleteItem item)
			{
				Item = item;
				text = item.Name;
			}
		}

		public void OnKeyDown(KeyDownEvent evt)
		{
			if (IsOpen)
			{
				if (evt.keyCode == KeyCode.UpArrow)
				{
					Previous();
					evt.PreventDefault();
				}
				else if (evt.keyCode == KeyCode.DownArrow)
				{
					Next();
					evt.PreventDefault();
				}
				else if (CloseKeys.Contains(evt.keyCode))
				{
					Hide();
					evt.PreventDefault();
				}
				else if (SelectKeys.Contains(evt.keyCode))
				{
					Select(evt.keyCode);
					Hide();
					evt.PreventDefault();
				}
			}
			else
			{
				if (OpenKeys.Contains(evt.keyCode))
				{
					Show();
					evt.PreventDefault();
				}
			}
		}

		public void OnFilterChanged(ChangeEvent<string> evt)
		{
			if (string.IsNullOrEmpty(_filter))
				Show();

			if (IsOpen)
				Filter();
		}

		protected override void ExecuteDefaultAction(EventBase evt)
		{
			if (_window != null)
			{
				if (evt is MouseCaptureOutEvent release)
				{
					// need to handle something else capturing the mouse so the window can't get stuck open
					Hide();
				}
				else if (evt is MouseDownEvent down)
				{
					if (!worldBound.Contains(down.mousePosition))
						Hide();

					// there doesn't seem to be a consistent way of maintaining focus on the input element so its
					// easiest to just set the focus back

					var window = EditorWindow.focusedWindow;
					var cursor = _input.cursorIndex;

					FocusText();
				}
			}

			base.ExecuteDefaultAction(evt);
		}
	}
}