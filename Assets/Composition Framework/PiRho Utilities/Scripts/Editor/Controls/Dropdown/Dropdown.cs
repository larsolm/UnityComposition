using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace PiRhoSoft.Utilities.Editor
{
	public abstract class Dropdown<T> : VisualElement, IBindableProperty<T>, IBindableObject<T>
	{
		private const string _styleSheetPath = Utilities.AssetPath + "Controls/Dropdown/Dropdown.uss";
		private const string _ussBase = "pargon-dropdown";

		private PopupField<T> _popup;
		private List<string> _options;
		private List<T> _values;
		private Func<T> _getValue;
		private Action<T> _setValue;

		public abstract T GetValueFromProperty(SerializedProperty property);
		public abstract void UpdateProperty(T value, VisualElement element, SerializedProperty property);

		public Dropdown(List<string> options, List<T> values, T value, SerializedProperty property)
		{
			Setup(options, values, value);

			ElementHelper.Bind(this, this, property);
		}

		public Dropdown(List<string> options, List<T> values, T value, Object owner, Func<T> getValue, Action<T> setValue)
		{
			Setup(options, values, value);

			ElementHelper.Bind(this, this, owner);

			_getValue = getValue;
			_setValue = setValue;
		}

		private void Setup(List<string> options, List<T> values, T value)
		{
			ElementHelper.AddStyleSheet(this, _styleSheetPath);

			_options = options;
			_values = values;
			_popup = new PopupField<T>(values, value, GetName, GetName);

			AddToClassList(_ussBase);
			Add(_popup);
		}

		public T GetValueFromElement(VisualElement element)
		{
			return _popup.value;
		}

		public T GetValueFromObject(Object owner)
		{
			return _getValue();
		}

		public void UpdateElement(T value, VisualElement element, SerializedProperty property)
		{
			_popup.SetValueWithoutNotify(value);
		}

		public void UpdateElement(T value, VisualElement element, Object owner)
		{
			_popup.SetValueWithoutNotify(value);
		}

		public void UpdateObject(T value, VisualElement element, Object owner)
		{
			_setValue(value);
		}

		private string GetName(T value)
		{
			var index = _values.IndexOf(value);
			return index < 0 ? value.ToString() : _options[index];
		}
	}
}
