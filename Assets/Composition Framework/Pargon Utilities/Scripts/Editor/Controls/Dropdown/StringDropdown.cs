using UnityEditor;

namespace PiRhoSoft.PargonUtilities.Editor
{
	public class StringDropdown : Dropdown<string>
	{
		protected override string GetValueFromProperty(SerializedProperty property)
		{
			return property.stringValue;
		}

		protected override void SetValueToProperty(SerializedProperty property, string value)
		{
			property.stringValue = value;
		}
	}
}
