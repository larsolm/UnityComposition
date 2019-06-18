using PiRhoSoft.CompositionExample;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace PiRhoSoft.ExamplesEditor
{
	[CustomEditor(typeof(AutocompleteNode))]
	public class AutocompleteNodeEditor : Editor
	{
		public override VisualElement CreateInspectorGUI()
		{
			var container = new VisualElement();

			var property = serializedObject.FindProperty(nameof(AutocompleteNode.Variable));
			Bind(container, property);
			return container;
		}

		private void Bind(VisualElement container, SerializedProperty property)
		{
			var propertyField = new PropertyField(property);
			propertyField.Bind(serializedObject);
			container.Add(propertyField);
		}
	}
}