using PiRhoSoft.Utilities.Editor;
using UnityEditor;
using UnityEngine.UIElements;

namespace PiRhoSoft.Composition.Editor
{
	[CustomPropertyDrawer(typeof(CustomVariableCollection))]
	public class CustomVariableCollectionDrawer : PropertyDrawer
	{
		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var field = new CustomVariableCollectionField(property.displayName, property.GetObject<CustomVariableCollection>(), property.serializedObject.targetObject);
			return field.ConfigureProperty(property, this.GetTooltip());
		}
	}
}
