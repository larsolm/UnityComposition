using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.PargonUtilities.Editor
{
	public abstract class BasePickerButton<ValueType> : BindableValueElement<ValueType>
	{
		private const string _styleSheetPath = "Assets/PargonUtilities/Scripts/Editor/Controls/Picker/BasePicker/BasePickerButton.uss";
		
		private Image _icon;
		private Label _label;

		protected void Setup<PickerType>(BasePicker<PickerType> picker, ValueType initialValue) where PickerType : class
		{
			ElementHelper.AddStyleSheet(this, _styleSheetPath);
			AddToClassList("base-button");

			var button = new Button();
			button.clickable.clicked += () => BasePickerWindow.Show(button.worldBound, picker);

			_icon = new Image();
			_label = new Label();

			button.Add(_icon);
			button.Add(_label);

			Add(button);

			SetValueWithoutNotify(initialValue);
		}

		protected void SetLabel(Texture icon, string text)
		{
			_icon.image = icon;
			_label.text = text;
		}
	}
}
