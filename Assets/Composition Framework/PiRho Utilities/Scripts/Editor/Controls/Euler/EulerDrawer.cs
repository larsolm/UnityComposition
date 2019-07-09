using PiRhoSoft.Utilities.Engine;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	[CustomPropertyDrawer(typeof(EulerAttribute))]
	class EulerDrawer : PropertyDrawer
	{
		private const string _invalidTypeWarning = "(PICED) Invalid type for EulerAttribute on field {0}: Euler can only be applied to quaternion fields";

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var container = ElementHelper.CreatePropertyContainer(property.displayName, ElementHelper.GetTooltip(fieldInfo));

			if (property.propertyType == SerializedPropertyType.Quaternion)
			{
				var euler = new Euler(property);
				euler.Setup(property.quaternionValue);
				container.Add(euler);
			}
			else
			{
				Debug.LogWarningFormat(_invalidTypeWarning, property.propertyPath);
			}

			return container;
		}
	}
}
