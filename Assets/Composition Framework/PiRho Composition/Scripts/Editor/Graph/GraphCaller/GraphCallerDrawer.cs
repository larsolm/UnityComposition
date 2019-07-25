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
			var field = new GraphCallerField(property.displayName, property.GetObject<GraphCaller>());
			return field.ConfigureProperty(property, this.GetTooltip());
		}
	}
}
