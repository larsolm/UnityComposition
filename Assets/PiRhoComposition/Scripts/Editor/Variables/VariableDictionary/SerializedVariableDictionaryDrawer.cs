using UnityEditor;
using UnityEngine.UIElements;

namespace PiRhoSoft.Composition.Editor
{
	[CustomPropertyDrawer(typeof(SerializedVariableDictionary))]
	public class SerializedVariableDictionaryDrawer : PropertyDrawer
	{
		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var field = new SerializedVariableDictionaryField();
			var proxy = new SerializedVariableDictionaryProxy(property);
			
			field.Setup(proxy);

			return field;
		}
	}
}
