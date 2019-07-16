using PiRhoSoft.Composition;
using UnityEditor;
using UnityEngine.UIElements;

namespace PiRhoSoft.Composition.Editor
{
	[CustomPropertyDrawer(typeof(VariableSet))]
	public class VariableSetDrawer : PropertyDrawer
	{
		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			return new VariableSetElement(property);
		}
	}
}
