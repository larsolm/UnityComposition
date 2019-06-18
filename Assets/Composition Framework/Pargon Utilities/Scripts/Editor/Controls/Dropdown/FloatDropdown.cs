using UnityEditor;

namespace PiRhoSoft.PargonUtilities.Editor
{
	public class FloatDropdown : Dropdown<float>
	{
		protected override float GetValueFromProperty(SerializedProperty property)
		{
			return property.floatValue;
		}

		protected override void SetValueToProperty(SerializedProperty property, float value)
		{
			property.floatValue = value;
		}
	}
}
