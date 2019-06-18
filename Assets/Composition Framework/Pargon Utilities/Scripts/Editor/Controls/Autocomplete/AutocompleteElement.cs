using PiRhoSoft.PargonUtilities.Editor;
using PiRhoSoft.PargonUtilities.Engine;
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.PargonInspector.Editor
{
	public class AutocompleteElement : BindableValueElement<string>
	{
		private const string _styleSheetPath = "Assets/PargonInspector/Scripts/Editor/Controls/Autocomplete/Autocomplete.uss";

		protected override string GetValueFromProperty(SerializedProperty property) => property.stringValue;
		protected override void SetValueToProperty(SerializedProperty property, string value) => property.stringValue = value;

		private AutocompleteSource _source;
		private TextField _text;
		private AutocompletePopup _popup;
		private bool _isOpen;

		public void Setup(SerializedProperty property, AutocompleteSource source)
		{
			_source = source;
			_text = new TextField();
			_popup = new AutocompletePopup();

			SetupEvents(_text);

			Add(_text);
			BindToProperty(property);
			ElementHelper.AddStyleSheet(this, _styleSheetPath);
			_popup.Setup(source);
		}

		protected override void Refresh()
		{
			if (_isOpen)
				Filter(value);
		}

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
			value = e.newValue;

			if (string.IsNullOrEmpty(e.previousValue) && !_isOpen)
				Open();
		}

		private void Open()
		{
			_isOpen = true;
			var window = EditorWindow.focusedWindow;
			_popup.Show(worldBound, value);
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