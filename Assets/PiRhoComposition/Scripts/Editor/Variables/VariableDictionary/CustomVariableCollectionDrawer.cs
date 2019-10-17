using UnityEditor;
using UnityEngine.UIElements;

namespace PiRhoSoft.Composition.Editor
{
	[CustomPropertyDrawer(typeof(CustomVariableCollection))]
	public class CustomVariableCollectionDrawer : PropertyDrawer
	{
		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var field = new CustomVariableCollectionField();
			var proxy = new CustomVariableCollectionProxy(property);

			field.Setup(proxy);

			return field;
		}
	}
}
