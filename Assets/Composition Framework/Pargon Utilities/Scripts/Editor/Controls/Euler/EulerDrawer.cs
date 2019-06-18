using PiRhoSoft.PargonUtilities.Engine;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.PargonUtilities.Editor
{
	[CustomPropertyDrawer(typeof(EulerAttribute))]
	class EulerDrawer : PropertyDrawer
	{
		private const string _invalidTypeWarning = "(PICED) Invalid type for EulerAttribute on field {0}: Euler can only be applied to quaternion fields";

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var container = ElementHelper.CreatePropertyContainer(property.displayName);

			if (property.propertyType == SerializedPropertyType.Quaternion)
			{
				var euler = new Euler();

				euler.Setup(property);
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
