using PiRhoSoft.Utilities.Editor;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Composition.Editor
{
	public interface IAutocompleteProxy
	{
		IAutocompleteItem Autocomplete { get; }
		Vector2 ControlPosition { get; }

		string Variable { get; set; }
		int Cursor { get; set; }
	}

	public class AutocompleteControl : VisualElement
	{
		private static readonly List<KeyCode> _openKeys = new List<KeyCode> { KeyCode.Escape, KeyCode.Tab };
		private static readonly List<KeyCode> _closeKeys = new List<KeyCode> { KeyCode.Escape };
		private static readonly List<KeyCode> _selectKeys = new List<KeyCode> { KeyCode.Tab, KeyCode.KeypadEnter, KeyCode.Return, KeyCode.KeypadPeriod, KeyCode.Period, KeyCode.LeftBracket, KeyCode.RightBracket, KeyCode.Space };
		private static readonly char[] _separators = { VariableReference.Separator, VariableReference.LookupOpen, VariableReference.LookupClose, ' ' };
		private static readonly Vector2 _windowSize = new Vector2(150, 300);

		private static readonly string Space = " ";
		private static readonly string Separator = VariableReference.Separator.ToString();
		private static readonly string LookupOpen = VariableReference.LookupOpen.ToString();
		private static readonly string LookupClose = VariableReference.LookupClose.ToString();
		private static readonly string Cast = $"{Space}{VariableReference.Cast}{Space}";

		private readonly IAutocompleteProxy _proxy;
		private readonly AutocompletePopup _autocomplete;
		private readonly TextElement _measure;
		private readonly TextField _textField;

		public AutocompleteControl(IAutocompleteProxy proxy, TextField textField)
		{
			_proxy = proxy;
			_textField = textField;
			_autocomplete = new AutocompletePopup(this);
			_measure = new TextElement();

			Add(_measure);

			RegisterCallback<AttachToPanelEvent>(evt => evt.destinationPanel.visualTree.Q<TemplateContainer>().Add(_autocomplete));
			RegisterCallback<DetachFromPanelEvent>(evt => _autocomplete.RemoveFromHierarchy());

			_textField.RegisterCallback<KeyDownEvent>(OnKeyDown);
			_textField.Q(className: TextField.inputUssClassName).RegisterCallback<MouseUpEvent>(evt => Refresh());
			_textField.RegisterCallback<FocusEvent>(evt => Show());
			_textField.RegisterCallback<BlurEvent>(evt => Hide());
		}

		public void Show()
		{
			if (!_autocomplete.IsOpen)
				_autocomplete.Show();

			Refresh();
		}

		public void Hide()
		{
			if (_autocomplete.IsOpen)
				_autocomplete.Hide();
		}

		public void Refresh()
		{
			if (_autocomplete.IsOpen)
			{
				var cursorIndex = Mathf.Clamp(_proxy.Cursor, 0, _proxy.Variable.Length);
				var autocomplete = GetCurrentAutocomplete(cursorIndex);
				var filter = GetFilter(cursorIndex);
				var cast = CheckCast(cursorIndex);
				var position = GetPopupRect(cursorIndex);

				_autocomplete.UpdatePosition(position);
				_autocomplete.UpdateAutocomplete(autocomplete, filter, cast);
			}
		}

		private IAutocompleteItem GetCurrentAutocomplete(int cursorIndex)
		{
			var autocomplete = _proxy.Autocomplete;
			if (autocomplete != null)
			{
				var index = _proxy.Variable.LastIndexOfAny(_separators, Mathf.Max(0, cursorIndex - 1));
				if (index >= 0)
				{
					var subtext = _proxy.Variable.Substring(0, index);
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
			}

			return autocomplete;
		}

		private int GetLastSeparatorIndex(int cursorIndex)
		{
			return string.IsNullOrEmpty(_proxy.Variable) || cursorIndex <= 0 ? -1 : _proxy.Variable.LastIndexOfAny(_separators, cursorIndex - 1);
		}

		private int GetNextSeparatorIndex(int cursorIndex)
		{
			return _proxy.Variable.IndexOfAny(_separators, cursorIndex);
		}

		private string GetSubtext(int index)
		{
			return index < 0 ? string.Empty : _proxy.Variable.Substring(0, index);
		}

		private bool CheckCast(int cursorIndex)
		{
			var index = GetLastSeparatorIndex(cursorIndex);
			return index > 3 && _proxy.Variable.Substring(index - 3).StartsWith(Cast);
		}

		private string GetFilter(int cursorIndex)
		{
			var index = GetLastSeparatorIndex(cursorIndex);
			if (index < 0)
				return GetSubtext(cursorIndex);
			else if (index >= _proxy.Variable.Length - 1)
				return _proxy.Variable[_proxy.Variable.Length - 1].ToString();
			else
				return _proxy.Variable.Substring(index, cursorIndex - index);
		}

		private Rect GetPopupRect(int cursorIndex)
		{
			var index = GetLastSeparatorIndex(cursorIndex);
			var subtext = GetSubtext(index);
			var size = _measure.MeasureTextSize(subtext, 0, MeasureMode.Undefined, 0, MeasureMode.Undefined);
			var position = _proxy.ControlPosition + size;

			return new Rect(position, _windowSize);
		}

		private void OnKeyDown(KeyDownEvent evt)
		{
			if (_autocomplete.IsOpen)
			{
				if (evt.keyCode == KeyCode.UpArrow)
				{
					_autocomplete.Previous();
					evt.StopPropagation();
					evt.PreventDefault();
				}
				else if (evt.keyCode == KeyCode.DownArrow)
				{
					_autocomplete.Next();
					evt.StopPropagation();
					evt.PreventDefault();
				}
				else if (_closeKeys.Contains(evt.keyCode))
				{
					Hide();
					evt.StopPropagation();
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
				var previousIndex = GetLastSeparatorIndex(_proxy.Cursor) + 1; // Add one to bypass the separator
				var nextIndex = GetNextSeparatorIndex(_proxy.Cursor);
				var previous = previousIndex == _proxy.Variable.Length ? _proxy.Variable : nextIndex < 0 ? _proxy.Variable.Remove(previousIndex) : _proxy.Variable.Remove(previousIndex, nextIndex - previousIndex);
				var cursor = previousIndex + _autocomplete.ActiveName.Length;
				var value = previous.Insert(previousIndex, _autocomplete.ActiveName);

				_proxy.Variable = value;
				_proxy.Cursor = cursor;

				if (!_textField.multiline && (key == KeyCode.Tab || key == KeyCode.KeypadEnter || key == KeyCode.Return))
				{
					schedule.Execute(() =>
					{
						_textField.Q(className: TextField.inputUssClassName).Focus();
						_proxy.Cursor = cursor;
					}).StartingIn(0);
				}
			}
		}

		private class AutocompletePopup : VisualElement
		{
			public const string Stylesheet = "Autocomplete/AutocompleteStyle.uss";
			public const string UssClassName = "pirho-autocomplete";
			public const string OpenClassName = UssClassName + "--open";
			public const string ScrollViewUssClassName = UssClassName + "__scroll-view";
			public const string ItemUssClassName = UssClassName + "__item";
			public const string ItemActiveUssClassName = ItemUssClassName + "--active";

			public bool IsOpen { get; private set; }
			public string ActiveName => _activeItem?.text;

			private AutocompleteControl _control;
			private ScrollView _scrollView;
			private TextElement _activeItem;
			private UQueryState<TextElement> _items;

			public AutocompletePopup(AutocompleteControl control)
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

				if (autocomplete != null)
				{
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
				if (index >= 0 && index <= (int)_items.Last().userData)
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
