using PiRhoSoft.Composition;
using PiRhoSoft.Utilities.Editor;
using UnityEditor;
using UnityEngine.UIElements;

namespace PiRhoSoft.Composition.Editor
{
	[CustomPropertyDrawer(typeof(GraphCaller))]
	public class GraphCallerDrawer : PropertyDrawer
	{
		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			return new GraphCallerElement(property, ElementHelper.GetTooltip(fieldInfo));
		}
	}
}
