using PiRhoSoft.CompositionEngine;
using PiRhoSoft.UtilityEditor;
using UnityEditor;
using UnityEngine;

namespace PiRhoSoft.CompositionEditor
{
	[CustomPropertyDrawer(typeof(InstructionGraphNode))]
	public class InstructionGraphNodeDrawer : PropertyDrawer
	{
		private static readonly Label _editIcon = new Label(Icon.BuiltIn(Icon.Edit), "", "Select and edit this instruction graph node");

		public static void Draw(GUIContent label, InstructionGraphNode target)
		{
			var rect = EditorGUILayout.GetControlRect(false);
			Draw(rect, label, target);
		}

		public static void Draw(Rect rect, GUIContent label, InstructionGraphNode target)
		{
			var editRect = RectHelper.TakeTrailingIcon(ref rect);
			var targetLabel = target ? target.Name : "Unconnected";

			EditorGUI.LabelField(rect, label, new GUIContent(targetLabel));

			if (target)
			{
				if (GUI.Button(editRect, _editIcon.Content, GUIStyle.none))
					InstructionGraphEditor.SelectNode(target);
			}
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return EditorGUIUtility.singleLineHeight;
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			label.tooltip = Label.GetTooltip(fieldInfo);
			Draw(position, label, property.objectReferenceValue as InstructionGraphNode);
		}
	}
}
