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

		public static VariableDefinition Draw(VariableDefinition definition, VariableInitializerType initializer, TagList tags, bool showConstraintLabel)
		{
			var rect = EditorGUILayout.GetControlRect(false, ValueDefinitionControl.GetHeight(definition.Definition, initializer, tags));
			return Draw(rect, definition, initializer, tags, showConstraintLabel);
		}

		public static VariableDefinition Draw(Rect position, VariableDefinition definition, VariableInitializerType initializer, TagList tags, bool showConstraintLabel)
		{
			definition.Definition = ValueDefinitionControl.Draw(position, new GUIContent(definition.Name), definition.Definition, initializer, tags, showConstraintLabel);
			return definition;
		}

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
