﻿using PiRhoSoft.Composition;
using UnityEditor;
using UnityEngine.UIElements;

namespace PiRhoSoft.Composition.Editor
{
	[CustomPropertyDrawer(typeof(VariableValueSource))]
	public class VariableValueSourceDrawer : PropertyDrawer
	{
		//public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		//{
		//	var target = PropertyHelper.GetObject<VariableValueSource>(property);
		//	var typeRect = RectHelper.TakeLine(ref position);
		//
		//	RectHelper.TakeLabel(ref position);
		//
		//	using (new EditObjectScope(property.serializedObject))
		//	{
		//		using (new UndoScope(property.serializedObject.targetObject, false))
		//		{
		//			//target.Type = (VariableSourceType)EnumDisplayDrawer.Draw(typeRect, label, (int)target.Type, typeof(VariableSourceType), EnumDisplayType.Buttons, false, 50);
		//
		//			if (target.Type == VariableSourceType.Value)
		//			{
		//				if (!target.Definition.IsTypeLocked)
		//				{
		//					var variableRect = RectHelper.TakeWidth(ref position, position.width * 0.5f);
		//					RectHelper.TakeHorizontalSpace(ref position);
		//					var definitionType = (VariableType)EditorGUI.EnumPopup(variableRect, target.Definition.Type);
		//
		//					if (definitionType != target.Definition.Type)
		//					{
		//						target.Definition = ValueDefinition.Create(definitionType, target.Definition.Constraint, target.Definition.Tag, target.Definition.Initializer, false, false);
		//						target.Value = target.Definition.Generate(null);
		//					}
		//				}
		//
		//				//target.Value = VariableValueDrawer.Draw(position, GUIContent.none, target.Value, target.Definition, true);
		//			}
		//			else if (target.Type == VariableSourceType.Reference)
		//			{
		//				VariableReferenceControl.Draw(position, target.Reference, GUIContent.none);
		//			}
		//		}
		//	}
		//}

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var container = new VisualElement();
			return container;
		}
	}
}
