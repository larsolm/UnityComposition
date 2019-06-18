using UnityEditor;

namespace PiRhoSoft.PargonUtilities.Editor
{
	public class IntDropdown : Dropdown<int>
	{
		protected override int GetValueFromProperty(SerializedProperty property)
		{
			return property.intValue;
		}

		protected override void SetValueToProperty(SerializedProperty property, int value)
		{
			property.intValue = value;
		}
	}
}
