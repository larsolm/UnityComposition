using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UIElements;

namespace PiRhoSoft.PargonUtilities.Editor
{
	public abstract class BindableValueElement<ValueType> : BindableElement, INotifyValueChanged<ValueType>
	{
		private const int _scheduleTime = 100;

		#region INotifyValueChanged Implementation

		private ValueType _value;
		public ValueType value
		{
			get
			{
				return _value;
			}
			set
			{
				if (!EqualityComparer<ValueType>.Default.Equals(_value, value))
				{
					if (panel != null)
					{
						using (var changeEvent = ChangeEvent<ValueType>.GetPooled(_value, value))
						{
							changeEvent.target = this;
							SetValueWithoutNotify(value);
							SendEvent(changeEvent);
						}
					}
					else
					{
						SetValueWithoutNotify(value);
					}
				}
			}
		}

		public void SetValueWithoutNotify(ValueType newValue)
		{
			_value = newValue;
			Refresh();
		}

		#endregion

		protected void BindToProperty(SerializedProperty property)
		{
			this.RegisterValueChangedCallback(e =>
			{
				SetValueToProperty(property, e.newValue);
				property.serializedObject.ApplyModifiedProperties();
			});

			schedule.Execute(() =>
			{
				var value = GetValueFromProperty(property);
				SetValueWithoutNotify(value);
			}).Every(_scheduleTime);
		}

		protected abstract void SetValueToProperty(SerializedProperty property, ValueType value);
		protected abstract ValueType GetValueFromProperty(SerializedProperty property);
		protected abstract void Refresh();
	}
}
