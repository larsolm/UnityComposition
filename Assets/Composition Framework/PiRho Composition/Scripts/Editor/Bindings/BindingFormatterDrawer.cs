using PiRhoSoft.Composition;
using UnityEditor;
using UnityEngine.UIElements;

namespace PiRhoSoft.Composition.Editor
{
	[CustomPropertyDrawer(typeof(BindingFormatter))]
	public class BindingFormatterDrawer : PropertyDrawer
	{
		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			return new BindingFormatterElement(property);
		}
	}
}
