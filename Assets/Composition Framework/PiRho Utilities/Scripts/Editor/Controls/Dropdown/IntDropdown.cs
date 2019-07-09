using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace PiRhoSoft.Utilities.Editor
{
	public class IntDropdown : Dropdown<int>
	{
		public IntDropdown(List<string> options, List<int> values, int value, SerializedProperty property) : base(options, values, value, property) { }
		public IntDropdown(List<string> options, List<int> values, int value, Object owner, Func<int> getValue, Action<int> setValue) : base(options, values, value, owner, getValue, setValue) { }

		public override int GetValueFromProperty(SerializedProperty property)
		{
			return property.intValue;
		}

		public override void UpdateProperty(int value, VisualElement element, SerializedProperty property)
		{
			property.intValue = value;
		}
	}
}
