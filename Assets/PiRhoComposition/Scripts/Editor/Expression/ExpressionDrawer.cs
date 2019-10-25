using UnityEditor;
using UnityEngine.UIElements;

namespace PiRhoSoft.Composition.Editor
{
	[CustomPropertyDrawer(typeof(Expression))]
	public class ExpressionDrawer : PropertyDrawer
	{
		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var autocomplete = Autocomplete.GetItem(property.serializedObject.targetObject);
			return new ExpressionField(property, autocomplete);
		}
	}
}
