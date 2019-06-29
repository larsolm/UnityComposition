using PiRhoSoft.CompositionEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace PiRhoSoft.CompositionEditor
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
