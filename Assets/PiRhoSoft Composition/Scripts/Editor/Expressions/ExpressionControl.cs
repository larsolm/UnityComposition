using PiRhoSoft.CompositionEngine;
using PiRhoSoft.UtilityEditor;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace PiRhoSoft.CompositionEditor
{
	public class ExpressionControl : ObjectControl<Expression>
	{
		private Expression _expression;

		public static float GetHeight(Expression expression, bool foldout)
		{
			return foldout ? FoldoutStringDrawer.GetHeight(expression.IsExpanded) : RectHelper.LineHeight * 5;
		}

		public static void Draw(Expression expression, GUIContent label)
		{
			var height = GetHeight(expression, false);
			var rect = EditorGUILayout.GetControlRect(false, height);
			Draw(rect, expression, label);
		}

		public static void Draw(Rect position, Expression expression, GUIContent label)
		{
			var rect = RectHelper.TakeLine(ref position);

			EditorGUI.LabelField(rect, label);

			using (new InvalidScope(!expression.HasError))
			{
				using (var changes = new EditorGUI.ChangeCheckScope())
				{
					var statement = EditorGUI.TextArea(position, expression.Statement);

					if (changes.changed)
						expression.SetStatement(statement);
				}
			}
		}

		public static void DrawFoldout(Expression expression, GUIContent label)
		{
			var height = GetHeight(expression, true);
			var rect = EditorGUILayout.GetControlRect(false, height);
			Draw(rect, expression, label);
		}

		public static void DrawFoldout(Rect position, Expression expression, GUIContent label)
		{
			using (new InvalidScope(!expression.HasError))
			{
				using (var changes = new EditorGUI.ChangeCheckScope())
				{
					var statement = FoldoutStringDrawer.Draw(position, label, expression.Statement, ref expression.IsExpanded);

					if (changes.changed)
						expression.SetStatement(statement);
				}
			}
		}

		public override void Setup(Expression target, SerializedProperty property, FieldInfo fieldInfo, PropertyAttribute attribute)
		{
			_expression = target;
		}

		public override float GetHeight(GUIContent label)
		{
			return GetHeight(_expression, false);
		}

		public override void Draw(Rect position, GUIContent label)
		{
			Draw(position, _expression, label);
		}
	}

	[CustomPropertyDrawer(typeof(Expression))]
	public class ExpressionDrawer : ControlDrawer<ExpressionControl>
	{
	}
}
