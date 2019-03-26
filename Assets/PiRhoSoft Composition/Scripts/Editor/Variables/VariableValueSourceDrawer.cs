using PiRhoSoft.CompositionEngine;
using PiRhoSoft.UtilityEditor;
using UnityEditor;
using UnityEngine;

namespace PiRhoSoft.CompositionEditor
{
	[CustomPropertyDrawer(typeof(VariableValueSource))]
	public class VariableValueSourceDrawer : PropertyDrawer
	{
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return RectHelper.LineHeight * 2;
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var target = PropertyHelper.GetObject<VariableValueSource>(property);
			var typeRect = RectHelper.TakeLine(ref position);

			RectHelper.TakeLabel(ref position);

			using (new EditObjectScope(property.serializedObject))
			{
				using (new UndoScope(property.serializedObject.targetObject, false))
				{
					target.Type = (VariableSourceType)EnumButtonsDrawer.Draw(typeRect, label, (int)target.Type, typeof(VariableSourceType), 50);

					if (target.Type == VariableSourceType.Value)
					{
						if (!target.Definition.IsTypeLocked)
						{
							var variableRect = RectHelper.TakeWidth(ref position, position.width * 0.5f);
							RectHelper.TakeHorizontalSpace(ref position);
							var definitionType = (VariableType)EditorGUI.EnumPopup(variableRect, target.Definition.Type);

							target.Definition = VariableDefinition.Create(target.Definition.Name, definitionType, target.Definition.Constraint, target.Definition.Tag, target.Definition.Initializer, false, false);
						}

						target.Value = VariableValueDrawer.Draw(position, GUIContent.none, target.Value, target.Definition);
					}
					else if (target.Type == VariableSourceType.Reference)
					{
						VariableReferenceControl.Draw(position, target.Reference, GUIContent.none);
					}
				}
			}
		}
	}
}
