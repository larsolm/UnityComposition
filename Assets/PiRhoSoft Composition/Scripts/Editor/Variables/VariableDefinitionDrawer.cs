using PiRhoSoft.CompositionEngine;
using UnityEditor;
using UnityEngine;

namespace PiRhoSoft.CompositionEditor
{
	[CustomPropertyDrawer(typeof(VariableDefinition))]
	public class VariableDefinitionDrawer : PropertyDrawer
	{
		private const float _labelWidth = 120.0f;
		private const float _labelIndent = 12.0f;

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			var definitionProperty = property.FindPropertyRelative(nameof(VariableDefinition.Definition));
			return EditorGUI.GetPropertyHeight(definitionProperty);
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var nameProperty = property.FindPropertyRelative(nameof(VariableDefinition.Name));
			var definitionProperty = property.FindPropertyRelative(nameof(VariableDefinition.Definition));

			EditorGUI.PropertyField(position, definitionProperty, new GUIContent(nameProperty.stringValue));
		}
	}
}
