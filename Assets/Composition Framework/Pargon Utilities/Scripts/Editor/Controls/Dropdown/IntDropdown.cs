using System;
using UnityEditor;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace PiRhoSoft.PargonUtilities.Editor
{
	public class IntDropdown : Dropdown<int>
	{
		public IntDropdown(SerializedProperty property) : base(property) { }
		public IntDropdown(Object owner, Func<int> getValue, Action<int> setValue) : base(owner, getValue, setValue) { }

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
