using PiRhoSoft.UtilityEngine;
using UnityEditor;
using UnityEngine;

namespace PiRhoSoft.UtilityEditor
{
	[CustomPropertyDrawer(typeof(EulerAnglesAttribute))]
	class EulerAnglesDrawer : PropertyDrawer
	{
		private const string _invalidTypeWarning = "Invalid type for EulerAnglesDrawer on field {0}: EulerAngles can only be applied to Quaternion fields";

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			label.tooltip = Label.GetTooltip(fieldInfo);

			if (property.propertyType == SerializedPropertyType.Quaternion)
			{
				var euler = EditorGUI.Vector3Field(position, label, property.quaternionValue.eulerAngles);
				property.quaternionValue = Quaternion.Euler(euler);
			}
			else
			{
				Debug.LogWarningFormat(_invalidTypeWarning, property.propertyPath);
			}
		}
	}
}
