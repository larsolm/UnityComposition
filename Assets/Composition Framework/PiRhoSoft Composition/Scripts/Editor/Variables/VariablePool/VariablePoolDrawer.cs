using PiRhoSoft.CompositionEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace PiRhoSoft.CompositionEditor
{
	[CustomPropertyDrawer(typeof(VariablePool))]
	public class VariablePoolDrawer : PropertyDrawer
	{
		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			return new VariablePoolElement(property);
		}
	}
}
