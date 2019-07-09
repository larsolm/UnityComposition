using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace PiRhoSoft.Utilities.Editor
{
	public class StringDropdown : Dropdown<string>
	{
		public StringDropdown(List<string> options, List<string> values, string value, SerializedProperty property) : base(options, values, value, property) { }
		public StringDropdown(List<string> options, List<string> values, string value, Object owner, Func<string> getValue, Action<string> setValue) : base(options, values, value, owner, getValue, setValue) { }

		public override string GetValueFromProperty(SerializedProperty property)
		{
			return property.stringValue;
		}

		public override void UpdateProperty(string value, VisualElement element, SerializedProperty property)
		{
			property.stringValue = value;
		}
	}
}
