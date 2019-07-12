using PiRhoSoft.PargonUtilities.Engine;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.PargonUtilities.Editor
{
	[CustomPropertyDrawer(typeof(EulerAttribute))]
	class EulerDrawer : PropertyDrawer
	{
		private const string _invalidTypeWarning = "(PUEEDIT) invalid type for EulerAttribute on field '{0}': Euler can only be applied to Quaternion fields";

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			if (property.propertyType == SerializedPropertyType.Quaternion)
			{
				var field = new EulerField(property, property.displayName);
				return ElementHelper.SetupPropertyField(field, fieldInfo);
			}
			else
			{
				Debug.LogWarningFormat(_invalidTypeWarning, property.propertyPath);
				return ElementHelper.CreateEmptyPropertyField(property.displayName);
			}
		}
	}
}
