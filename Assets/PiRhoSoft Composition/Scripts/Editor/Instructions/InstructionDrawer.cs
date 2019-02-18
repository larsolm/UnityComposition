using System;
using PiRhoSoft.CompositionEngine;
using PiRhoSoft.UtilityEditor;
using UnityEditor;
using UnityEngine;

namespace PiRhoSoft.CompositionEditor
{
	[CustomPropertyDrawer(typeof(Instruction))]
	public class InstructionDrawer : PropertyDrawer
	{
		private readonly static IconButton _editButton = new IconButton(IconButton.Edit, "Edit the selected Instruction");

		public static float GetHeight()
		{
			return EditorGUIUtility.singleLineHeight;
		}

		public static Instruction Draw(Rect position, GUIContent label, Instruction instruction, Type instructionType)
		{
			var popup = AssetHelper.GetAssetList(instructionType ?? typeof(Instruction), true, true);
			var index = popup.GetIndex(instruction);

			var buttonRect = instruction == null ? position : RectHelper.TakeTrailingIcon(ref position);
			var selectedIndex = EditorGUI.Popup(position, label, index, popup.Names);

			if (selectedIndex != index)
			{
				instruction = popup.GetAsset(selectedIndex) as Instruction;

				if (instruction == null)
				{
					var type = popup.GetType(selectedIndex);

					if (type != null)
						instruction = AssetHelper.CreateAsset(type.Name, type) as Instruction;
				}
			}
			
			if (instruction != null)
			{
				if (GUI.Button(buttonRect, _editButton.Content, GUIStyle.none))
					Selection.activeObject = instruction;
			}

			return instruction;
		}

		public static Instruction Draw(GUIContent label, Instruction instruction, Type instructionType)
		{
			var height = GetHeight();
			var rect = GUILayoutUtility.GetRect(0.0f, height, GUILayout.ExpandWidth(true));
			return Draw(rect, label, instruction, instructionType);
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return GetHeight();
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var instruction = property.objectReferenceValue as Instruction;
			var type = TypeHelper.GetAttribute<InstructionTypeAttribute>(fieldInfo)?.Type;

			property.objectReferenceValue = Draw(position, label, instruction, type);
		}
	}
}
