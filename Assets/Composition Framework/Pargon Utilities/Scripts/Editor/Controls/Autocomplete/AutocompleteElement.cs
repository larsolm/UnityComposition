using PiRhoSoft.PargonUtilities.Editor;
using PiRhoSoft.PargonUtilities.Engine;
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace PiRhoSoft.PargonInspector.Editor
{
	public class AutocompleteElement : VisualElement, IBindableProperty<string>, IBindableObject<string>
	{
		private const string _styleSheetPath = Utilities.AssetPath + "Controls/Autocomplete/Autocomplete.uss";

		private AutocompleteSource _source;
		private TextField _text;
		private AutocompletePopup _popup;
		private bool _isOpen;

		private Func<string> _getValue;
		private Action<string> _setValue;

		public AutocompleteElement(SerializedProperty property)
		{
			ElementHelper.Bind(this, this, property);
		}

		public AutocompleteElement(Object owner, Func<string> getValue, Action<string> setValue)
		{
			ElementHelper.Bind(this, this, owner);

			_getValue = getValue;
			_setValue = setValue;
		}

		public void Setup(AutocompleteSource source)
		{
			_source = source;
			_text = new TextField();
			_popup = new AutocompletePopup();

			SetupEvents(_text);

			Add(_text);

			ElementHelper.AddStyleSheet(this, _styleSheetPath);
			_popup.Setup(source);
		}

		#region Bindings

		public string GetValueFromElement(VisualElement element) => _text.value;
		public string GetValueFromProperty(SerializedProperty property) => property.stringValue;
		public string GetValueFromObject(Object owner) => _getValue();
		public void UpdateElement(string value, VisualElement element, SerializedProperty property) => UpdateElement(value);
		public void UpdateElement(string value, VisualElement element, Object owner) => UpdateElement(value);
		public void UpdateProperty(string value, VisualElement element, SerializedProperty property) => property.stringValue = value;
		public void UpdateObject(string value, VisualElement element, Object owner) => _setValue(value);

		private void UpdateElement(string value)
		{
			if (_isOpen)
				Filter(value);
		}

		#endregion

		private void SetupEvents(TextField field)
		{
			field.ElementAt(0).RegisterCallback<KeyDownEvent>(OnKeyDown, TrickleDown.TrickleDown);
			field.RegisterValueChangedCallback(OnValueChanged);
		}

		private static readonly char[] _skip = new char[] { '\t', ' ', '\n' };

		private void OnKeyDown(KeyDownEvent e)
		{
			switch (e.keyCode)
			{
				case KeyCode.Tab: Select(); e.PreventDefault(); break;
				case KeyCode.Return: Select(); e.PreventDefault(); break;
				case KeyCode.KeypadEnter: Select(); e.PreventDefault(); break;
				case KeyCode.Space: Select(); e.PreventDefault(); break;
				case KeyCode.Escape: Toggle(); e.PreventDefault(); break;
				case KeyCode.DownArrow: Next(); e.PreventDefault(); break;
				case KeyCode.UpArrow: Previous(); e.PreventDefault(); break;
			}

			if (e.keyCode == KeyCode.None && Array.IndexOf(_skip, e.character) >= 0)
				e.PreventDefault();
		}

		private void OnValueChanged(ChangeEvent<string> e)
		{
			//value = e.newValue;

			if (string.IsNullOrEmpty(e.previousValue) && !_isOpen)
				Open();
		}

		private void Open()
		{
			_isOpen = true;
			var window = EditorWindow.focusedWindow;
			//_popup.Show(worldBound, value);
			window.Focus();
			schedule.Execute(() =>
			{
				_text.ElementAt(0).Focus();
				MoveToEnd(); // Focus highlights the text
			});
		}

		private void MoveToEnd()
		{
			_text.SelectRange(_text.text.Length, _text.text.Length);
		}

		private void Close()
		{
			_isOpen = false;
			_popup.Hide();
		}

		private void Toggle()
		{
			if (_isOpen)
				Close();
			else
				Open();
		}

		private void Next()
		{
			_popup.Next();
		}

		private void Previous()
		{
			_popup.Previous();
		}

		private void Filter(string value)
		{
			_popup.Filter(value);
		}

		private void Select()
		{
			if (_isOpen)
			{
				_text.value = _popup.Select();
				MoveToEnd();
				Close();
			}
		}
	}
}