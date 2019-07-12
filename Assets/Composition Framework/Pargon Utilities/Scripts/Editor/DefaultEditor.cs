using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace PiRhoSoft.PargonUtilities.Editor
{
	[CustomEditor(typeof(Object), true, isFallback = true)]
	public class DefaultEditor : UnityEditor.Editor
	{
		public override VisualElement CreateInspectorGUI()
		{
			var container = new VisualElement();

			var iterator = serializedObject.GetIterator();
			if (iterator.NextVisible(true))
			{
				do
				{
					var propertyField = new PropertyField(iterator) { name = "PropertyField:" + iterator.propertyPath };

					if (iterator.propertyPath == "m_Script" && serializedObject.targetObject != null)
						propertyField.SetEnabled(value: false);

					container.Add(propertyField);
				}
				while (iterator.NextVisible(false));
			}

			// container.Bind(serializedObject); // this seems to happen automatically except when exiting play mode

			return container;
		}
	}
}