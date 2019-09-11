using PiRhoSoft.Composition;
using PiRhoSoft.Utilities.Editor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace PiRhoSoft.Composition.Editor
{
	[CustomPropertyDrawer(typeof(Message))]
	public class MessageDrawer : PropertyDrawer
	{
		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var container = new FieldContainer(property.displayName, this.GetTooltip());
			var textProperty = property.FindPropertyRelative(nameof(Message.Text));
			var text = new PropertyField(textProperty) { label = null };

			container.Add(text);

			return container;
		}
	}
}
