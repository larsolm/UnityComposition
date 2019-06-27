using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace PiRhoSoft.PargonUtilities.Editor
{
	public class FloatDropdown : Dropdown<float>
	{
		public FloatDropdown(List<string> options, List<float> values, float value, SerializedProperty property) : base(options, values, value, property) { }
		public FloatDropdown(List<string> options, List<float> values, float value, Object owner, Func<float> getValue, Action<float> setValue) : base(options, values, value, owner, getValue, setValue) { }

		public override float GetValueFromProperty(SerializedProperty property)
		{
			return property.floatValue;
		}

		public override void UpdateProperty(float value, VisualElement element, SerializedProperty property)
		{
			property.floatValue = value;
		}
	}
}
