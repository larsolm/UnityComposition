using PiRhoSoft.Utilities.Editor;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Composition.Editor
{
	public class VariableReferenceTextControl : VisualElement
	{
		public const string TextUssClassName = VariableReferenceControl.AdvancedUssClassName + "__text";

		private static readonly List<KeyCode> _openKeys = new List<KeyCode> { KeyCode.Escape, KeyCode.Tab };
		private static readonly List<KeyCode> _closeKeys = new List<KeyCode> { KeyCode.Escape };
		private static readonly List<KeyCode> _selectKeys = new List<KeyCode> { KeyCode.Tab, KeyCode.KeypadEnter, KeyCode.Return, KeyCode.KeypadPeriod, KeyCode.Period, KeyCode.LeftBracket, KeyCode.Space };
		private static readonly char[] _separators = { VariableReference.Separator, VariableReference.LookupOpen, VariableReference.LookupClose, ' ' };
		private static readonly Vector2 _windowSize = new Vector2(150, 300);

		private static readonly string Space = " ";
		private static readonly string Separator = VariableReference.Separator.ToString();
		private static readonly string LookupOpen = VariableReference.LookupOpen.ToString();
		private static readonly string LookupClose = VariableReference.LookupClose.ToString();
		private static readonly string Cast = $"{Space}{VariableReference.Cast}{Space}";

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

			_textField.AddToClassList(TextUssClassName);
			_textField.RegisterValueChangedCallback(evt => SetValue(evt.newValue));
			_textField.RegisterCallback<KeyDownEvent>(OnKeyDown);
			_textField.RegisterCallback<MouseUpEvent>(evt => UpdateAutocomplete(_textField.cursorIndex));
			_textField.RegisterCallback<FocusEvent>(evt => Show());
			_textField.RegisterCallback<BlurEvent>(evt => Hide());
		}

		public void Refresh()
		{
			_textField.SetValueWithoutNotify(_control.Value.Variable);

			UpdateAutocomplete(_textField.cursorIndex);
		}

		private void SetValue(string value)
		{
			_control.Value.Variable = value;
			this.SendChangeEvent(_control.Value, _control.Value);
		}

		private void Show()
		{
			if (!_autocomplete.IsOpen)
				_autocomplete.Show();

			UpdateAutocomplete(_textField.cursorIndex);
		}

		private void Hide()
		{
			if (_autocomplete.IsOpen)
				_autocomplete.Hide();
		}

		private void UpdateAutocomplete(int cursorIndex)
		{
			cursorIndex = Mathf.Clamp(cursorIndex, 0, _textField.text.Length);

			var autocomplete = GetCurrentAutocomplete(cursorIndex);
			var filter = GetFilter(cursorIndex);
			var cast = CheckCast(cursorIndex);
			var position = GetPopupRect(cursorIndex);

			_autocomplete.UpdatePosition(position);
			_autocomplete.UpdateAutocomplete(autocomplete, filter, cast);
		}

		private IAutocompleteItem GetCurrentAutocomplete(int cursorIndex)
		{
			var autocomplete = _control.Autocomplete;
			var index = _textField.text.LastIndexOfAny(_separators, Mathf.Max(0, cursorIndex - 1));
			if (index >= 0)
			{
				var subtext = _textField.text.Substring(0, index);
				var names = subtext.Split(_separators);
				var cast = false;

				foreach (var name in names)
				{
					if (autocomplete.IsCastable && name == VariableReference.Cast)
					{
						cast = true;
						continue;
					}

					if (cast)
					{
						var nextAutocomplete = autocomplete.GetTypeField(name);
						if (nextAutocomplete == null)
							break;

						autocomplete = nextAutocomplete;
					}
					else
					{
						if (int.TryParse(name, out var number))
						{
							var nextAutocomplete = autocomplete.GetIndexField();
							if (nextAutocomplete == null)
								break;

							autocomplete = nextAutocomplete;
						}
						else
						{
							var nextAutocomplete = autocomplete.GetField(name);
							if (nextAutocomplete == null)
								break;

							autocomplete = nextAutocomplete;
						}
					}

					cast = false;
				}
			}

			return autocomplete;
		}

		private int GetLastSeparatorIndex(int cursorIndex)
		{
			return string.IsNullOrEmpty(_textField.text) || cursorIndex <= 0 ? -1 : _textField.text.LastIndexOfAny(_separators, cursorIndex - 1);
		}

		private int GetNextSeparatorIndex(int cursorIndex)
		{
			return _textField.text.IndexOfAny(_separators, cursorIndex);
		}

		private string GetSubtext(int index)
		{
			return index < 0 ? string.Empty : _textField.text.Substring(0, index);
		}


		private bool CheckCast(int cursorIndex)
		{
			var index = GetLastSeparatorIndex(cursorIndex);
			return index > 3 && _textField.text.Substring(index - 3).StartsWith(Cast);
		}

		private string GetFilter(int cursorIndex)
		{
			var index = GetLastSeparatorIndex(cursorIndex);
			if (index < 0)
				return GetSubtext(cursorIndex - 1);
			else if (index >= _textField.text.Length - 1)
				return _textField.text[_textField.text.Length - 1].ToString();
			else
				return _textField.text.Substring(index, cursorIndex - index);
		}

		private Rect GetPopupRect(int cursorIndex)
		{
			var index = GetLastSeparatorIndex(cursorIndex);
			var subtext = GetSubtext(index);
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
					UpdateAutocomplete(_textField.cursorIndex - 1);
				}
				else if (evt.keyCode == KeyCode.RightArrow)
				{
					UpdateAutocomplete(_textField.cursorIndex + 1);
				}
				else if (_closeKeys.Contains(evt.keyCode))
				{
					Hide();
					evt.PreventDefault();
				}
				else if (_selectKeys.Contains(evt.keyCode))
				{
					Select(evt.keyCode);
				}
			}
			else
			{
				if (_openKeys.Contains(evt.keyCode))
				{
					Show();
					evt.PreventDefault();
				}
			}
		}

		private void Select(KeyCode key)
		{
			if (_autocomplete.ActiveName != null)
			{
				var previousIndex = GetLastSeparatorIndex(_textField.cursorIndex) + 1; // Add one to bypass the separator
				var nextIndex = GetNextSeparatorIndex(_textField.cursorIndex);
				var prefix = previousIndex == _textField.text.Length ? _textField.text : nextIndex < 0 ? _textField.text.Remove(previousIndex) : _textField.text.Remove(previousIndex, nextIndex - previousIndex);
				var cursor = previousIndex + _autocomplete.ActiveName.Length;

				_textField.value = prefix.Insert(previousIndex, _autocomplete.ActiveName);
				_textField.SelectRange(cursor, cursor);

				if (key == KeyCode.Tab || key == KeyCode.KeypadEnter || key == KeyCode.Return)
				{
					schedule.Execute(() =>
					{
						_textField.Q(className: TextField.inputUssClassName).Focus();
						_textField.SelectRange(cursor, cursor);
					}).StartingIn(0);
				}
			}
		}

		private class AutocompletePopup : VisualElement
		{
			public const string Stylesheet = "Variables/VariableReference/VariableReferenceTextStyle.uss";
			public const string UssClassName = "pirho-variable-reference-text";
			public const string OpenClassName = UssClassName + "--open";
			public const string ScrollViewUssClassName = UssClassName + "__scroll-view";
			public const string ItemUssClassName = UssClassName + "__item";
			public const string ItemActiveUssClassName = ItemUssClassName + "--active";

			public bool IsOpen { get; private set; }
			public string ActiveName => _activeItem?.text;

			private VariableReferenceTextControl _control;
			private ScrollView _scrollView;
			private TextElement _activeItem;
			private UQueryState<TextElement> _items;

			public AutocompletePopup(VariableReferenceTextControl control)
			{
				_control = control;

				_scrollView = new ScrollView(ScrollViewMode.Vertical);
				_scrollView.AddToClassList(ScrollViewUssClassName);

				_items = _scrollView.contentContainer.Query<TextElement>().Build();

				Add(_scrollView);

				this.AddStyleSheet(Configuration.EditorPath, Stylesheet);
				AddToClassList(UssClassName);
			}

			public void Show()
			{
				IsOpen = true;
				AddToClassList(OpenClassName);
			}

			public void Hide()
			{
				IsOpen = false;
				RemoveFromClassList(OpenClassName);
			}

			public void Previous()
			{
				if (_activeItem != null)
					SetActive((int)_activeItem.userData - 1);
				else
					SetActive(0);
			}

			public void Next()
			{
				if (_activeItem != null)
					SetActive((int)_activeItem.userData + 1);
				else
					SetActive(0);
			}

			public void UpdateAutocomplete(IAutocompleteItem autocomplete, string filter, bool cast)
			{
				_activeItem = null;
				_scrollView.contentContainer.Clear();

				var index = 0;
				var filterType = GetFilterType(filter);

				if (cast)
				{
					var types = autocomplete.GetTypes();
					if (types != null)
					{
						var subtext = filter.Substring(1);
						foreach (var type in types)
						{
							if (string.IsNullOrWhiteSpace(subtext) || type.Name.IndexOf(subtext, StringComparison.CurrentCultureIgnoreCase) >= 0)
								AddElement(type.Name, index++);
						}
					}
				}
				else if (filterType == FilterType.Cast)
				{
					if (autocomplete.IsCastable)
						AddElement(VariableReference.Cast, index++);
				}
				else if (filterType != FilterType.Lookup)
				{
					var fields = autocomplete.GetFields();
					if (fields != null)
					{
						foreach (var field in fields)
						{
							if (filterType == FilterType.Separator || string.IsNullOrEmpty(filter) || field.Name.IndexOf(filter, StringComparison.CurrentCultureIgnoreCase) >= 0)
								AddElement(field.Name, index++);
						}
					}
				}
			}

			public void UpdatePosition(Rect rect)
			{
				var local = parent.WorldToLocal(rect);
				style.left = local.x;
				style.top = local.y;
				style.width = local.width;
				style.height = local.height;
			}

			private void SetActive(int index)
			{
				if (index >= 0 && index < (int)_items.Last().userData)
				{
					var item = _items.AtIndex(index);
					SetActive(item);
				}
			}

			private void SetActive(TextElement item)
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

			private void AddElement(string label, int index)
			{
				var element = new TextElement { text = label, userData = index };
				element.userData = index;
				element.RegisterCallback<MouseOverEvent>(e => SetActive(element));
				element.RegisterCallback<MouseDownEvent>(e => _control.Select(KeyCode.Return), TrickleDown.TrickleDown); // This must trickle down and can't be a clickable be cause it gets overriden be the FocusOut event on the base text field
				element.AddToClassList(ItemUssClassName);

				_scrollView.contentContainer.Add(element);

				if (index == 0)
					SetActive(element);
			}

			private enum FilterType
			{
				Identifier,
				Separator,
				Lookup,
				Cast
			}

			private FilterType GetFilterType(string filter)
			{
				if (filter.StartsWith(Space)) return FilterType.Cast;
				if (filter.StartsWith(Separator)) return FilterType.Separator;
				if (filter.StartsWith(LookupOpen) || filter.StartsWith(LookupClose)) return FilterType.Lookup;
				return FilterType.Identifier;
			}
		}
	}
}
