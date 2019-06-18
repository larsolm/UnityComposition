using PiRhoSoft.PargonUtilities.Engine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace PiRhoSoft.PargonUtilities.Editor
{
	[CustomEditor(typeof(Examples))]
	public class ExamplesEditor : UnityEditor.Editor
	{
		public override VisualElement CreateInspectorGUI()
		{
			var container = new VisualElement();

			var property = serializedObject.GetIterator();
			property.Next(true);
			Bind(container, property);

			while (property.Next(false))
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
