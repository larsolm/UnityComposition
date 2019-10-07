using PiRhoSoft.Utilities.Editor;
using UnityEditor;
using UnityEngine.UIElements;

namespace PiRhoSoft.Composition.Editor
{
	[CustomPropertyDrawer(typeof(Message))]
	public class MessageDrawer : PropertyDrawer
	{
		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var autocomplete = Autocomplete.GetItem(property.serializedObject.targetObject);
			var field = new MessageField(property.displayName, property.GetObject<Message>(), autocomplete);
			return field.ConfigureProperty(property, this.GetTooltip());
		}
	}
}
