using System;
using UnityEditor;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace PiRhoSoft.PargonUtilities.Editor
{
	public class StringDropdown : Dropdown<string>
	{
		public StringDropdown(SerializedProperty property) : base(property) { }
		public StringDropdown(Object owner, Func<string> getValue, Action<string> setValue) : base(owner, getValue, setValue) { }

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
