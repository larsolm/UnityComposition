using PiRhoSoft.Utilities;
using PiRhoSoft.Utilities.Editor;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Composition.Editor
{
	public class VariableReferenceTextControl : VisualElement
	{
		private static readonly List<KeyCode> _selectKeys = new List<KeyCode> { KeyCode.Tab, KeyCode.KeypadEnter, KeyCode.Return, KeyCode.Period, KeyCode.KeypadPeriod };
		private static readonly List<KeyCode> _openKeys = new List<KeyCode> { KeyCode.Escape, KeyCode.Tab };
		private static readonly List<KeyCode> _closeKeys = new List<KeyCode> { KeyCode.Escape };
		private static readonly char[] _separators = { VariableReference.Separator, VariableReference.LookupOpen, VariableReference.LookupClose };
		private static readonly Vector2 _windowSize = new Vector2(150, 300);

		private VariableReferenceControl _control;
		private AutocompletePopup _autocomplete;
		private TextField _textField;
		private TextElement _measure;

		public VariableReferenceTextControl(VariableReferenceControl control)
		{
			_control = control;
			_autocomplete = new AutocompletePopup(this);
			_textField = new TextField();
			_measure = new TextElement();

			Add(_measure);
			Add(_textField);

			RegisterCallback<AttachToPanelEvent>(evt => evt.destinationPanel.visualTree.Q<TemplateContainer>().Add(_autocomplete));
			RegisterCallback<DetachFromPanelEvent>(evt => _autocomplete.RemoveFromHierarchy());

			_textField.Q<VisualElement>(className: TextField.inputUssClassName).RegisterCallback<KeyDownEvent>(OnKeyDown);
			_textField.RegisterValueChangedCallback(evt => TextChanged());
			_textField.RegisterCallback<FocusEvent>(evt => _autocomplete.Show());
			_textField.RegisterCallback<BlurEvent>(evt => _autocomplete.Hide());
		}
		
		public void Refresh()
		{
			_textField.SetValueWithoutNotify(_control.Value.Variable);

			var source = GetCurrentSource();
			var filter = GetFilter();

			_autocomplete.SetSource(source);
			_autocomplete.UpdateFilter(filter);
		}

		private void TextChanged()
		{
			if (!_autocomplete.IsOpen)
				_autocomplete.Show();
		}

		private AutocompleteSource GetCurrentSource()
		{
			var source = _control.Source;
			var index = _textField.text.LastIndexOfAny(_separators, Math.Max(0, _textField.cursorIndex - 1));
			if (index >= 0)
			{
				var subtext = _textField.text.Substring(0, index);
				var names = subtext.Split(_separators);

				foreach (var name in names)
				{
					var nextSource = GetNextSource(source, name);
					if (nextSource == null)
						break;

					source = nextSource;
				}
			}

			return source;
		}

		private AutocompleteSource GetNextSource(AutocompleteSource source, string name)
		{
			foreach (var item in source.Items)
			{
				if (item.Name == name)
					return item.Source;
			}

			return null;
		}

		private int GetLastSeparatorIndex()
		{
			return string.IsNullOrEmpty(_textField.text) || _textField.cursorIndex == 0 ? -1 : _textField.text.LastIndexOfAny(_separators, _textField.cursorIndex - 1);
		}

		private int GetNextSeparatorIndex()
		{
			return _textField.text.IndexOfAny(_separators, _textField.cursorIndex);
		}

		private string GetFilter()
		{
			var index = GetLastSeparatorIndex();
			if (index < 0)
				return _textField.cursorIndex == 0 ? string.Empty : _textField.text.Substring(0, _textField.cursorIndex - 1);
			else if (index == _textField.text.Length - 1)
				return string.Empty;
			else
				return _textField.text.Substring(index + 1, _textField.cursorIndex - index - 1);
		}

		private Rect GetPopupRect()
		{
			var index = GetLastSeparatorIndex();
			var subtext = index < 0 ? string.Empty : _textField.text.Substring(0, index);
			var size = _measure.MeasureTextSize(subtext, 0, MeasureMode.Undefined, worldBound.height, MeasureMode.Exactly);
			var position = worldBound.position + size;

			return new Rect(position, _windowSize);
		}

		private void OnKeyDown(KeyDownEvent evt)
		{
			if (_autocomplete.IsOpen)
			{
				if (evt.keyCode == KeyCode.UpArrow)
				{
					_autocomplete.Previous();
					evt.PreventDefault();
				}
				else if (evt.keyCode == KeyCode.DownArrow)
				{
					_autocomplete.Next();
					evt.PreventDefault();
				}
				else if (evt.keyCode == KeyCode.LeftArrow)
				{
					Refresh();
				}
				else if (evt.keyCode == KeyCode.RightArrow)
				{
					Refresh();
				}
				else if (_closeKeys.Contains(evt.keyCode))
				{
					_autocomplete.Hide();
					evt.PreventDefault();
				}
				else if (_selectKeys.Contains(evt.keyCode))
				{
					Select();
					evt.PreventDefault();
					// TODO: these don't actual prevent the enter/tab events text box stuff from firing for some reason
				}
			}
			else
			{
				if (_openKeys.Contains(evt.keyCode))
				{
					_autocomplete.Show();
					evt.PreventDefault();
				}
			}
		}

		private void Select()
		{
			if (_autocomplete.ActiveItem != null)
			{
				var previousIndex = GetLastSeparatorIndex() + 1; // Add one to bypass the separator
				var nextIndex = GetNextSeparatorIndex();
				var prefix = previousIndex == _textField.text.Length ? _textField.text : nextIndex < 0 ? _textField.text.Remove(previousIndex) : _textField.text.Remove(previousIndex, nextIndex - previousIndex);
				_textField.value = prefix.Insert(previousIndex, _autocomplete.ActiveItem.Name);

				var cursor = previousIndex + _autocomplete.ActiveItem.Name.Length;
				_textField.SelectRange(cursor, cursor);
			}
		}

		private class AutocompletePopup : VisualElement
		{
			public const string Stylesheet = "Variables/VariableReference/VariableReferenceTextStyle.uss";
			public const string UssClassName = "pirho-variable-reference-text";
			public const string OpenClassName = UssClassName + "--open";
			public const string NoneValidUssClassName = UssClassName + "--none-valid";
			public const string ScrollViewUssClassName = UssClassName + "__scroll-view";
			public const string ItemUssClassName = UssClassName + "__item";
			public const string ItemActiveUssClassName = ItemUssClassName + "--active";
			public const string ItemValidUssClassName = ItemUssClassName + "--valid";
			public const string NoneUssClassName = ItemUssClassName + "--none";

			public bool IsOpen { get; private set; }
			public AutocompleteItem ActiveItem => _activeItem?.Item;

			private VariableReferenceTextControl _control;
			private AutocompleteSource _source;
			private ScrollView _scrollView;
			private ItemElement _activeItem;
			private string _filter = null;
			private int _validCount = 0;

			private TextElement _noneValidItem;

			private UQueryState<ItemElement> _items;
			private UQueryState<ItemElement> _validItems;

			public AutocompletePopup(VariableReferenceTextControl control)
			{
				_control = control;

				_scrollView = new ScrollView(ScrollViewMode.Vertical);
				_scrollView.AddToClassList(ScrollViewUssClassName);

				_noneValidItem = new TextElement() { text = "None" };
				_noneValidItem.AddToClassList(NoneUssClassName);

				_items = _scrollView.contentContainer.Query<ItemElement>().Build();
				_validItems = _scrollView.contentContainer.Query<ItemElement>(className: ItemValidUssClassName).Build();

				Add(_scrollView);

				this.AddStyleSheet(CompositionEditor.EditorPath, Stylesheet);
				AddToClassList(UssClassName);
			}

			public void SetSource(AutocompleteSource source)
			{
				_source = source;
				_scrollView.contentContainer.Clear();
				_scrollView.contentContainer.Add(_noneValidItem);

				foreach (var item in _source.Items)
				{
					// TODO: add clicking and hovering to select these
					var element = new ItemElement(item);
					element.AddToClassList(ItemUssClassName);

					_scrollView.contentContainer.Add(element);
				}
			}

			public void Show()
			{
				IsOpen = true;
				AddToClassList(OpenClassName);

				var world = _control.GetPopupRect();
				var local = parent.WorldToLocal(world);
				style.left = local.x;
				style.top = local.y;
				style.width = local.width;
				style.height = local.height;
			}

			public void Hide()
			{
				IsOpen = false;
				RemoveFromClassList(OpenClassName);
			}

			public void Previous()
			{
				if (_activeItem != null)
					SetActive(_activeItem.ValidIndex - 1);
				else
					SetActive(0);
			}

			public void Next()
			{
				if (_activeItem != null)
					SetActive(_activeItem.ValidIndex + 1);
				else
					SetActive(0);
			}

			public void UpdateFilter(string filter)
			{
				_filter = filter;

				SetActive(null);

				var index = 0;
				_items.ForEach(item =>
				{
					var valid = string.IsNullOrEmpty(_filter) || item.Item.Name.IndexOf(_filter, StringComparison.CurrentCultureIgnoreCase) >= 0;

					item.ValidIndex = valid ? index++ : -1;
					item.EnableInClassList(ItemValidUssClassName, valid);

					if (valid && _activeItem == null)
						SetActive(item);
				});

				_validCount = index;

				EnableInClassList(NoneValidUssClassName, _validCount == 0);
			}

			private void SetActive(int index)
			{
				if (index >= 0 && index < _validCount)
				{
					var item = _validItems.AtIndex(index);
					SetActive(item);
				}
			}

			private void SetActive(ItemElement item)
			{
				if (_activeItem != null)
					_activeItem.RemoveFromClassList(ItemActiveUssClassName);

				_activeItem = item;

				if (_activeItem != null)
				{
					_activeItem.AddToClassList(ItemActiveUssClassName);
					_scrollView.ScrollTo(_activeItem);
				}
			}

			private class ItemElement : TextElement
			{
				public AutocompleteItem Item;
				public int ValidIndex = -1;

				public ItemElement(AutocompleteItem item)
				{
					Item = item;
					text = item.Name;
				}
			}
		}
	}
}
