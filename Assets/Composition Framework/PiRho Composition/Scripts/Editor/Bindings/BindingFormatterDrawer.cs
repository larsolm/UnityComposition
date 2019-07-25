using PiRhoSoft.Utilities.Editor;
using UnityEditor;
using UnityEngine.UIElements;

namespace PiRhoSoft.Composition.Editor
{
	[CustomPropertyDrawer(typeof(BindingFormatter))]
	public class BindingFormatterDrawer : PropertyDrawer
	{
		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var field = new BindingFormatterField(property.displayName, property.GetObject<BindingFormatter>());
			return field.ConfigureProperty(property, this.GetTooltip());
		}
	}
}
