using UnityEditor;
using UnityEngine.UIElements;

namespace PiRhoSoft.Composition.Editor
{
	[CustomPropertyDrawer(typeof(SchemaVariableCollection))]
	public class SchemaVariableCollectionDrawer : PropertyDrawer
	{
		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var field = new SchemaVariableCollectionField();
			var proxy = new SchemaVariableCollectionProxy(property);

			field.Setup(proxy);

			return field;
		}
	}
}
