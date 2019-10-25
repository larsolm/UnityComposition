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
			return new MessageField(property, autocomplete);
		}
	}
}
