using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace PiRhoSoft.PargonUtilities.Editor
{
	public abstract class BasePickerButton<ValueType> : VisualElement, IBindableProperty<ValueType>, IBindableObject<ValueType>
	{
		private const string _styleSheetPath = Utilities.AssetPath + "Controls/Picker/BasePicker/BasePickerButton.uss";
		private const string _ussBaseButton = "pargon-picker-button";
		
		public ValueType Value { get; protected set; }

		private Image _icon;
		private Label _label;

		private Func<ValueType> _getValue;
		private Action<ValueType> _setValue;

		public BasePickerButton(SerializedProperty property)
		{
			ElementHelper.Bind(this, this, property);
		}

		public BasePickerButton(Object owner, Func<ValueType> getValue, Action<ValueType> setValue)
		{
			ElementHelper.Bind(this, this, owner);

			_getValue = getValue;
			_setValue = setValue;
		}

		protected void Setup<PickerType>(BasePicker<PickerType> picker, ValueType value) where PickerType : class
		{
			ElementHelper.AddStyleSheet(this, _styleSheetPath);
			AddToClassList(_ussBaseButton);

			Value = value;

			var button = new Button();
			button.clickable.clicked += () => BasePickerWindow.Show(button.worldBound, picker);

			_icon = new Image();
			_label = new Label();

			button.Add(_icon);
			button.Add(_label);

			Add(button);

			UpdateElement(value);
		}

		public abstract ValueType GetValueFromProperty(SerializedProperty property);
		public abstract void UpdateProperty(ValueType value, VisualElement element, SerializedProperty property);
		protected abstract void UpdateElement(ValueType value);

		public ValueType GetValueFromElement(VisualElement element)
		{
			return Value;
		}

		public ValueType GetValueFromObject(Object owner)
		{
			return _getValue();
		}

		public void UpdateElement(ValueType value, VisualElement element, SerializedProperty property)
		{
			UpdateElement(value);
		}

		public void UpdateElement(ValueType value, VisualElement element, Object owner)
		{
			UpdateElement(value);
		}

		public void UpdateObject(ValueType value, VisualElement element, Object owner)
		{
			_setValue(value);
		}

		protected void SetLabel(Texture icon, string text)
		{
			_icon.image = icon;
			_label.text = text;

			ElementHelper.SetVisible(_icon, icon != null);
		}
	}
}
