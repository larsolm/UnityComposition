using UnityEditor;
using UnityEngine.UIElements;

namespace PiRhoSoft.Composition.Editor
{
	[CustomPropertyDrawer(typeof(SerializedVariableList))]
	public class SerializedVariableListDrawer : PropertyDrawer
	{
		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var field = new SerializedVariableListField();
			var proxy = new SerializedVariableListProxy(property);
			
			field.Setup(property, proxy);

			return field;
		}
	}
}
