using System.Reflection;
using PiRhoSoft.CompositionEngine;
using PiRhoSoft.UtilityEditor;
using UnityEditor;
using UnityEngine;

namespace PiRhoSoft.CompositionEditor
{
	public class VariableReferenceControl : ObjectControl<VariableReference>
	{
		private VariableReference _variableReference;

		public static void Draw(GUIContent label, VariableReference reference)
		{
			var rect = EditorGUILayout.GetControlRect(false);
			Draw(rect, reference, label);
		}

		public static void Draw(Rect position, VariableReference reference, GUIContent label)
		{
			using (var changes = new EditorGUI.ChangeCheckScope())
			{
				var output = EditorGUI.TextField(position, label, reference.ToString());

				if (changes.changed)
					reference.Update(output);
			}
		}

		public override void Setup(VariableReference target, SerializedProperty property, FieldInfo fieldInfo, PropertyAttribute attribute)
		{
			_variableReference = target;
		}

		public override float GetHeight(GUIContent label)
		{
			return EditorGUIUtility.singleLineHeight;
		}

		public override void Draw(Rect position, GUIContent label)
		{
			Draw(position, _variableReference, label);
		}
	}

	[CustomPropertyDrawer(typeof(VariableReference))]
	public class VariableReferenceDrawer : ControlDrawer<VariableReferenceControl>
	{
	}
}
