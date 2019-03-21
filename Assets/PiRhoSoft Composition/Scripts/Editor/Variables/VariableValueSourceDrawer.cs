using PiRhoSoft.CompositionEngine;
using PiRhoSoft.UtilityEditor;
using UnityEditor;
using UnityEngine;

namespace PiRhoSoft.CompositionEditor
{
	[CustomPropertyDrawer(typeof(VariableValueSource))]
	public class VaribaleValueSourceDrawer : PropertyDrawer
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
						target.Value = VariableValueDrawer.Draw(position, GUIContent.none, target.Value, target.Definition);
					else if (target.Type == VariableSourceType.Reference)
						VariableReferenceControl.Draw(position, target.Reference, GUIContent.none);
				}
			}
		}
	}
}
