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
		private string _lastError;

		public static float GetHeight(Expression expression, bool foldout)
		{
			var expressionHeight = foldout ? FoldoutStringDrawer.GetHeight(expression.IsExpanded) : RectHelper.LineHeight * 5;
			var errorHeight = GetErrorHeight(expression, foldout);

			return expressionHeight + errorHeight;
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
			var errorHeight = GetErrorHeight(expression, false);
			var errorRect = RectHelper.TakeTrailingHeight(ref position, errorHeight);

			EditorGUI.LabelField(rect, label);

			using (new InvalidScope(!expression.HasError))
			{
				using (var changes = new EditorGUI.ChangeCheckScope())
				{
					var statement = EditorGUI.TextArea(position, expression.Statement);

					if (changes.changed || (expression.HasError && string.IsNullOrEmpty(expression.LastResult)))
					{
						var result = expression.SetStatement(statement);
						expression.LastResult = result.Success ? null : result.Message;
					}
				}
			}

			DrawError(errorRect, expression, false);
		}

		public static void DrawFoldout(Expression expression, GUIContent label)
		{
			var height = GetHeight(expression, true);
			var rect = EditorGUILayout.GetControlRect(false, height);
			Draw(rect, expression, label);
		}

		public static void DrawFoldout(Rect position, Expression expression, GUIContent label)
		{
			var errorHeight = GetErrorHeight(expression, true);
			var errorRect = RectHelper.TakeTrailingHeight(ref position, errorHeight);

			using (new InvalidScope(!expression.HasError))
			{
				using (var changes = new EditorGUI.ChangeCheckScope())
				{
					var statement = FoldoutStringDrawer.Draw(position, label, expression.Statement, ref expression.IsExpanded);

					if (changes.changed || (expression.HasError && string.IsNullOrEmpty(expression.LastResult)))
					{
						var result = expression.SetStatement(statement);
						expression.LastResult = result.Success ? null : result.Message;
					}
				}
			}

			DrawError(errorRect, expression, true);
		}

		private static float GetErrorHeight(Expression expression, bool foldout)
		{
			if (!foldout || expression.IsExpanded)
				return RectHelper.VerticalSpace + EditorGUIUtility.singleLineHeight * 3;
			else
				return 0.0f;
		}

		private static void DrawError(Rect rect, Expression expression, bool foldout)
		{
			if ((!foldout || expression.IsExpanded) && expression.HasError)
			{
				RectHelper.TakeVerticalSpace(ref rect);
				EditorGUI.HelpBox(rect, expression.LastResult, MessageType.Error);
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
