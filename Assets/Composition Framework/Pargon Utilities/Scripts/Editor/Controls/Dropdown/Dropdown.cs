using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;

namespace PiRhoSoft.PargonUtilities.Editor
{
	public abstract class Dropdown<T> : BindableValueElement<T>
	{
		private const string _styleSheetPath = "Assets/PargonUtilities/Scripts/Editor/Controls/Dropdown/Dropdown.uss";

		private PopupField<T> _popup;
		protected List<string> _options;
		protected List<T> _values;

		public void Setup(List<string> options, List<T> values, SerializedProperty property)
		{
			Setup(options, values, GetValueFromProperty(property));
			BindToProperty(property);
		}

		public void Setup(List<string> options, List<T> values, T initialValue)
		{
			ElementHelper.AddStyleSheet(this, _styleSheetPath);

			_options = options;
			_values = values;
			_popup = new PopupField<T>(values, initialValue, GetName, GetName);

			AddToClassList("base-dropdown");
			Add(_popup);

			SetValueWithoutNotify(initialValue);
		}

		protected override void Refresh()
		{
			_popup.value = value;
		}

		private string GetName(T value)
		{
			var index = _values.IndexOf(value);
			return index < 0 ? value.ToString() : _options[index];
		}
	}
}
