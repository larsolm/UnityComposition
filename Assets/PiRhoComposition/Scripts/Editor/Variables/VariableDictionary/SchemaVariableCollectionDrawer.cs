using UnityEditor;
using UnityEngine.UIElements;

namespace PiRhoSoft.Composition.Editor
{
	[CustomPropertyDrawer(typeof(SchemaVariableCollection))]
	public class SchemaVariableCollectionDrawer : PropertyDrawer
	{
		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			return new SchemaVariableCollectionField(property);
		}
	}
}
