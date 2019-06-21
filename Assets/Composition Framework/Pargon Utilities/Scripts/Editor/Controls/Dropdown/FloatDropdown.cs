using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace PiRhoSoft.PargonUtilities.Editor
{
	public class FloatDropdown : Dropdown<float>
	{
		public FloatDropdown(SerializedProperty property) : base(property) { }
		public FloatDropdown(Object owner, Func<float> getValue, Action<float> setValue) : base(owner, getValue, setValue) { }

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
