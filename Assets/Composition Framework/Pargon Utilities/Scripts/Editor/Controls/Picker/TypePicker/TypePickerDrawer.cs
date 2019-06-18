using PiRhoSoft.PargonUtilities.Engine;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.PargonUtilities.Editor
{
	[CustomPropertyDrawer(typeof(TypePickerAttribute))]
	public class TypePickerDrawer : PropertyDrawer
	{
		private const string _invalidTypeWarning = "(PITPDIT) Invalid type for TypePickerAttribute on field {0}: TypePicker can only be applied to string fields";

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var container = ElementHelper.CreatePropertyContainer(property.displayName);

			if (property.propertyType == SerializedPropertyType.String)
			{
				var typeAttribute = attribute as TypePickerAttribute;
				var picker = new TypePickerButton();
				picker.Setup(typeAttribute.BaseType, typeAttribute.ShowAbstract, property);

				container.Add(picker);
			}
			else
			{
				Debug.LogWarningFormat(_invalidTypeWarning, property.propertyPath);
			}

			return container;
		}
	}
}
