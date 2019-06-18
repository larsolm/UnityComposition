using PiRhoSoft.CompositionEngine;
using PiRhoSoft.PargonUtilities.Editor;
using PiRhoSoft.UtilityEditor;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace PiRhoSoft.CompositionEditor
{
	public class ExpressionControl : ObjectControl<Expression>
	{
		private static readonly Label _errorLabel = new Label(Icon.BuiltIn("console.erroricon"));

		#region Static Object Interface

		public static float GetHeight(Expression expression, bool fullWidth, int minimumLines, int maximumLines)
		{
			var expressionHeight = StringDisplayDrawer.GetAreaHeight(expression.Statement, fullWidth, false, minimumLines, maximumLines);
			var errorHeight = GetErrorHeight(expression);

			return expressionHeight + errorHeight;
		}

		public static float GetFoldoutHeight(Expression expression, bool isExpanded, bool fullWidth, int minimumLines, int maximumLines)
		{
			var expressionHeight = StringDisplayDrawer.GetFoldoutAreaHeight(expression.Statement, isExpanded, fullWidth, false, minimumLines, maximumLines);
			var errorHeight = isExpanded ? GetErrorHeight(expression) : 0.0f;

			return expressionHeight + errorHeight;
		}

		public static void Draw(Expression expression, GUIContent label, bool fullWidth, int minimumLines, int maximumLines)
		{
			var height = GetHeight(expression, fullWidth, minimumLines, maximumLines);
			var rect = EditorGUILayout.GetControlRect(false, height);
			Draw(rect, expression, label, fullWidth);
		}

		public static void Draw(Rect position, Expression expression, GUIContent label, bool fullWidth)
		{
			var errorHeight = GetErrorHeight(expression);
			var errorRect = RectHelper.TakeTrailingHeight(ref position, errorHeight);
			var expanded = true;

			DrawExpression(position, expression, label, fullWidth, false, ref expanded);
			DrawError(errorRect, expression, label, fullWidth);
		}

		public static void DrawFoldout(Expression expression, GUIContent label, ref bool isExpanded, bool fullWidth, int minimumLines, int maximumLines)
		{
			var height = GetFoldoutHeight(expression, isExpanded, fullWidth, minimumLines, maximumLines);
			var rect = EditorGUILayout.GetControlRect(false, height);
			DrawFoldout(rect, expression, label, ref isExpanded, fullWidth);
		}

		public static void DrawFoldout(Rect position, Expression expression, GUIContent label, ref bool isExpanded, bool fullWidth)
		{
			var errorHeight = GetErrorHeight(expression);
			var errorRect = RectHelper.TakeTrailingHeight(ref position, errorHeight);

			DrawExpression(position, expression, label, fullWidth, true, ref isExpanded);

			if (isExpanded)
				DrawError(errorRect, expression, label, fullWidth);
		}

		private static float GetErrorHeight(Expression expression)
		{
			if (expression.HasError)
			{
				_errorLabel.Content.text = expression.CompilationResult.Message;
				var height = EditorStyles.helpBox.CalcHeight(_errorLabel.Content, RectHelper.CurrentViewWidth - RectHelper.Indent);

				return RectHelper.VerticalSpace + height;
			}
			else
			{
				return 0.0f;
			}
		}

		private static void DrawExpression(Rect position, Expression expression, GUIContent label, bool fullWidth, bool foldout, ref bool isExpanded)
		{
			using (new InvalidScope(!expression.HasError))
			{
				using (var changes = new EditorGUI.ChangeCheckScope())
				{
					var statement = foldout
						? StringDisplayDrawer.DrawFoldoutArea(position, label, expression.Statement, ref isExpanded, fullWidth, false)
						: StringDisplayDrawer.DrawArea(position, label, expression.Statement, fullWidth, false);

					if (changes.changed)
						expression.SetStatement(statement);
				}
			}
		}

		private static void DrawError(Rect rect, Expression expression, GUIContent label, bool fullWidth)
		{
			if (expression.HasError)
			{
				RectHelper.TakeVerticalSpace(ref rect);

				if (fullWidth)
					RectHelper.TakeWidth(ref rect, RectHelper.Indent);
				else if (!string.IsNullOrEmpty(label.text))
					RectHelper.TakeLabel(ref rect);

				EditorGUI.HelpBox(rect, expression.CompilationResult.Message, MessageType.Error);
			}
		}

		#endregion

		#region Control Interface

		private SerializedProperty _property;
		private Expression _expression;
		private bool _foldout = false;
		private bool _fullWidth = true;
		private int _minimumLines = 2;
		private int _maximumLines = 8;

		public override void Setup(Expression target, SerializedProperty property, FieldInfo fieldInfo, PropertyAttribute attribute)
		{
			var display = TypeHelper.GetAttribute<ExpressionDisplayAttribute>(fieldInfo);

			_property = property;
			_expression = target;

			if (display != null)
			{
				_foldout = display.Foldout;
				_fullWidth = display.FullWidth;
				_minimumLines = display.MinimumLines;
				_maximumLines = display.MaximumLines;
			}
		}

		public override float GetHeight(GUIContent label)
		{
			return _foldout
				? GetFoldoutHeight(_expression, _property.isExpanded, _fullWidth, _minimumLines, _maximumLines)
				: GetHeight(_expression, _fullWidth, _minimumLines, _maximumLines);
		}

		public override void Draw(Rect position, GUIContent label)
		{
			var expanded = _property.isExpanded;

			if (_foldout)
				DrawFoldout(position, _expression, label, ref expanded, _fullWidth);
			else
				Draw(position, _expression, label, _fullWidth);

			_property.isExpanded = expanded;
		}

		#endregion
	}

	[CustomPropertyDrawer(typeof(Expression))]
	public class ExpressionDrawer : PropertyDrawer<ExpressionControl>
	{
	}
}
