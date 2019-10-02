using PiRhoSoft.Utilities.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Composition.Editor
{
	public class VariableReferenceTextControl : VisualElement
	{
		public const string TextUssClassName = VariableReferenceControl.AdvancedUssClassName + "__text";

		private static readonly List<KeyCode> _openKeys = new List<KeyCode> { KeyCode.Escape, KeyCode.Tab };
		private static readonly List<KeyCode> _closeKeys = new List<KeyCode> { KeyCode.Escape };
		private static readonly char[] _separators = { VariableReference.Separator, VariableReference.LookupOpen, VariableReference.LookupClose, ' ' };
		private static readonly Vector2 _windowSize = new Vector2(150, 300);

		private static readonly Dictionary<KeyCode, string> _selectKeys = new Dictionary<KeyCode, string>
		{
			{ KeyCode.Tab, null },
			{ KeyCode.KeypadEnter, null },
			{ KeyCode.Return, null },
			{ KeyCode.KeypadPeriod, "." },
			{ KeyCode.Period, "." },
			{ KeyCode.LeftBracket, "["},
			{ KeyCode.RightBracket, "]"}
		};

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
			var autocomplete = GetCurrentAutocomplete(cursorIndex);
			var filter = GetFilter(cursorIndex);

			_autocomplete.SetAutocomplete(autocomplete);
			_autocomplete.UpdateFilter(filter);
		}

		private IAutocompleteItem GetCurrentAutocomplete(int cursorIndex)
		{
			var autocomplete = _control.Autocomplete;
			var index = _textField.text.LastIndexOfAny(_separators, Math.Max(0, cursorIndex - 1));
			if (index >= 0)
			{
				var subtext = _textField.text.Substring(0, index);
				var names = subtext.Split(_separators);
				var cast = false;

				foreach (var name in names)
				{
					if (name == VariableReference.Cast)
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
						var nextAutocomplete = autocomplete.GetField(name);
						if (nextAutocomplete == null)
							break;

						autocomplete = nextAutocomplete;
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

		private string GetFilter(int cursorIndex)
		{
			var index = GetLastSeparatorIndex(cursorIndex);
			if (index < 0)
				return cursorIndex <= 0 ? string.Empty : _textField.text.Substring(0, cursorIndex - 1);
			else if (index >= _textField.text.Length - 1)
				return string.Empty;
			else
				return _textField.text.Substring(index + 1, cursorIndex - index - 1);
		}

		private Rect GetPopupRect()
		{
			var index = GetLastSeparatorIndex(_textField.cursorIndex);
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
				else if (_selectKeys.ContainsKey(evt.keyCode))
				{
					Select(evt.keyCode);

					if (evt.keyCode != KeyCode.Period && evt.keyCode != KeyCode.KeypadPeriod)
					{
						evt.StopImmediatePropagation();
						evt.PreventDefault();

						schedule.Execute(() =>
						{
							_textField.Q(className: TextField.inputUssClassName).Focus();
							_textField.SelectRange(_textField.text.Length, _textField.text.Length);
						}).StartingIn(0);
					}
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

			public bool IsOpen { get; private set; }
			public string ActiveName => _activeItem?.Name;

			private VariableReferenceTextControl _control;
			private IAutocompleteItem _autocomplete;
			private ScrollView _scrollView;
			private ItemElement _activeItem;
			private CastElement _castElement;
			private string _filter = null;
			private int _validCount = 0;

			private UQueryState<ItemElement> _items;
			private UQueryState<ItemElement> _validItems;

			public AutocompletePopup(VariableReferenceTextControl control)
			{
				_control = control;

				_scrollView = new ScrollView(ScrollViewMode.Vertical);
				_scrollView.AddToClassList(ScrollViewUssClassName);

				_items = _scrollView.contentContainer.Query<ItemElement>().Build();
				_validItems = _scrollView.contentContainer.Query<ItemElement>(className: ItemValidUssClassName).Build();

				Add(_scrollView);

				this.AddStyleSheet(Configuration.EditorPath, Stylesheet);
				AddToClassList(UssClassName);
			}

			public void SetAutocomplete(IAutocompleteItem autocomplete)
			{
				_autocomplete = autocomplete;
				_scrollView.contentContainer.Clear();

				if (_autocomplete.IsCastable)
				{
					if (_filter == VariableReference.Cast)
					{
						var types = _autocomplete.GetTypes();

						if (types != null)
						{
							foreach (var type in types)
							{
								var item = Autocomplete.GetItem(type);

								if (item != null)
									AddElement(new AutocompleteElement(item));
							}
						}
					}
					else
					{
						AddElement(new CastElement());
					}
				}

				var fields = _autocomplete.GetFields();

				if (fields != null)
				{
					foreach (var field in _autocomplete.GetFields())
						AddElement(new AutocompleteElement(field));
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
					var valid = string.IsNullOrEmpty(_filter) || item.Name.IndexOf(_filter, StringComparison.CurrentCultureIgnoreCase) >= 0;

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

			private void AddElement(ItemElement element)
			{
				element.RegisterCallback<MouseOverEvent>(e => SetActive(element));
				element.RegisterCallback<MouseDownEvent>(e => _control.Select(KeyCode.Return), TrickleDown.TrickleDown); // This must trickle down and can't be a clickable be cause it gets overriden be the FocusOut event on the base text field
				element.AddToClassList(ItemUssClassName);

				_scrollView.contentContainer.Add(element);
			}

			private class ItemElement : TextElement
			{
				public int ValidIndex = -1;
				public string Name;

				public ItemElement(string label, string name)
				{
					text = label;
					Name = name;
				}
			}

			private class AutocompleteElement : ItemElement
			{
				public IAutocompleteItem Item;

				public AutocompleteElement(IAutocompleteItem item) : base(item.Name, item.Name)
				{
					Item = item;
				}
			}

			private class CastElement : ItemElement
			{
				public CastElement() : base(VariableReference.Cast, $" {VariableReference.Cast} ")
				{
				}
			}
		}
	}
}
