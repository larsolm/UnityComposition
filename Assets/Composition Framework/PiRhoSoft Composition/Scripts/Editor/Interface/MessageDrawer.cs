using PiRhoSoft.CompositionEngine;
using PiRhoSoft.UtilityEditor;
using UnityEditor;
using UnityEngine;

namespace PiRhoSoft.CompositionEditor
{
	[CustomPropertyDrawer(typeof(Message))]
	public class MessageDrawer : PropertyDrawer
	{
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			var text = property.FindPropertyRelative(nameof(Message.Text));
			return EditorGUI.GetPropertyHeight(text);
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			label.tooltip = Label.GetTooltip(fieldInfo);
			var text = property.FindPropertyRelative(nameof(Message.Text));
			EditorGUI.PropertyField(position, text, label);
		}
	}
}
