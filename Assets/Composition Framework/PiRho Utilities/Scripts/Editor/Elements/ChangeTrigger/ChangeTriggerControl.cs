using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	public abstract class ChangeTriggerControl : BindableElement
	{
		public void Reset(SerializedProperty property)
		{
			if (this.IsBound())
				binding.Release();

			this.BindProperty(property);
		}
	}

	public class ChangeTriggerControl<T> : ChangeTriggerControl, INotifyValueChanged<T>
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
				_onChanged(previous, _value);
			}
		}

		public ChangeTriggerControl(T value, Action<T, T> onChanged)
		{
			_value = value;
			_onChanged = onChanged;

			style.display = DisplayStyle.None;
		}

		public void SetValueWithoutNotify(T newValue)
		{
			_value = newValue;
		}

		protected override void ExecuteDefaultActionAtTarget(EventBase evt)
		{
			base.ExecuteDefaultActionAtTarget(evt);

			if (typeof(T) == typeof(Enum) && this.TryGetPropertyBindEvent(evt, out var property))
			{
				BindingExtensions.DefaultEnumBind(this as INotifyValueChanged<Enum>, property);
				evt.StopPropagation();
			}
		}
	}
}