using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	public class ChangeTriggerControl<T> : BindableElement, INotifyValueChanged<T>
	{
		private T _value;
		private Action<T, T> _onChanged;

		public T value
		{
			get => _value;
			set
			{
				var previous = _value;
				SetValueWithoutNotify(value);
				_onChanged(_value, value);
			}
		}

		public ChangeTriggerControl(T value, Action<T, T> onChanged)
		{
			_value = value;
			_onChanged = onChanged;

			style.display = DisplayStyle.None;
		}

		public void Reset(SerializedProperty property)
		{
			if (this.IsBound())
				binding.Release();

			this.BindProperty(property);
		}

		public void SetValueWithoutNotify(T newValue)
		{
			_value = newValue;
		}
	}
}