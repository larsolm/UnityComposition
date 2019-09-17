using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	[CustomPropertyDrawer(typeof(InlineAttribute))]
	class InlineDrawer : PropertyDrawer
	{
		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var inlineAttribute = attribute as InlineAttribute;
			var label = inlineAttribute.ShowMemberLabels ? null : property.displayName;
			var tooltip = inlineAttribute.ShowMemberLabels ? null : this.GetTooltip();
			var element = new FieldContainer(label, tooltip);
			element.style.flexDirection = FlexDirection.Column;

			foreach (var child in property.Children())
			{
				var field = inlineAttribute.ShowMemberLabels ? new PropertyField(child) : new PropertyField(child, null);
				element.Add(field);
			}

			return element;
		}
	}
}