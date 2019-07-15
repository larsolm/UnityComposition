using UnityEditor;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	[CustomPropertyDrawer(typeof(StretchAttribute))]
	class StretchDrawer : PropertyDrawer
	{
		public static readonly string UssClassName = "pirho-stretch";

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var container = ElementHelper.CreateNextDrawer(property, property.displayName, fieldInfo, attribute);

			container.AddToClassList(UssClassName);
			container.style.flexDirection = FlexDirection.Column;

			return container;
		}
	}
}