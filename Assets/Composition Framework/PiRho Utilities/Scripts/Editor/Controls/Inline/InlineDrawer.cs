using PiRhoSoft.Utilities.Engine;
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
			var inline = attribute as InlineAttribute;
			var label = inline.ShowMemberLabels ? null : property.displayName;
			var tooltip = inline.ShowMemberLabels ? null : ElementHelper.GetTooltip(fieldInfo);
			var container = ElementHelper.CreatePropertyContainer(label, tooltip);
			var end = property.GetEndProperty();

			property.NextVisible(true);

			while (!SerializedProperty.EqualContents(property, end))
			{
				var field = inline.ShowMemberLabels ? new PropertyField(property) : new PropertyField(property, null);

				container.Add(field);
				property.NextVisible(false);
			}

			return container;
		}
	}
}
