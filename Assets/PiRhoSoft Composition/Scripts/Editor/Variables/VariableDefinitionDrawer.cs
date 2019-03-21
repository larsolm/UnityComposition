﻿using System;
using System.Linq;
using PiRhoSoft.CompositionEngine;
using PiRhoSoft.UtilityEditor;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PiRhoSoft.CompositionEditor
{
	[CustomPropertyDrawer(typeof(VariableDefinition))]
	public class VariableDefinitionDrawer : PropertyDrawer
	{
		private readonly static GUIContent _initializerLabel = new GUIContent("Initializer", "The Expression to execute and assign when initializing, resetting, or updating the variable");
		private readonly static GUIContent _defaultLabel = new GUIContent("Default", "The default value to use for the variable");

		private readonly static GUIContent _numberConstraintLabel = new GUIContent("Constraint", "The range of values allowed for the variable");
		private readonly static GUIContent _stringConstraintLabel = new GUIContent("Constraint", "The comma separated list of valid values for the variable");
		private readonly static GUIContent _objectConstraintLabel = new GUIContent("Constraint", "The Object type that the assigned object must be derived from or have an instance of");
		private readonly static GUIContent _tagLabel = new GUIContent("Tag", "An identifier that can be used to reset or persist this variable");
		private readonly static GUIContent _useRangeConstraintLabel = new GUIContent();
		private readonly static GUIContent _minimumConstraintLabel = new GUIContent("Between");
		private readonly static GUIContent _maximumConstraintLabel = new GUIContent("and");

		private static Expression _expression = new Expression();
		private const float _labelWidth = 100.0f;
		private const float _labelIndent = 4.0f;

		public static float GetHeight(SerializedProperty property, VariableInitializerType initializerType, TagList tags)
		{
			var typeProperty = property.FindPropertyRelative(nameof(VariableDefinition.Type));
			return GetHeight((VariableType)typeProperty.enumValueIndex, initializerType, null, tags);
		}

		public static float GetHeight(VariableDefinition definition, VariableInitializerType initializerType, TagList tags)
		{
			return GetHeight(definition.Type, initializerType, definition.Initializer, tags);
		}

		private static float GetHeight(VariableType type, VariableInitializerType initializerType, Expression initializer, TagList tags)
		{
			var height = EditorGUIUtility.singleLineHeight;

			if (HasInitializer(type, initializerType))
			{
				if (initializerType == VariableInitializerType.Expression && initializer != null)
					height += ExpressionControl.GetHeight(initializer, true) + RectHelper.VerticalSpace;
				else
					height += RectHelper.LineHeight;
			}

			if (HasConstraint(type))
				height += RectHelper.LineHeight;

			if (HasTags(tags))
				height += RectHelper.LineHeight;

			return height;
		}

		public static void Draw(Rect position, SerializedProperty property, VariableInitializerType initializer, TagList tags)
		{
			var nameProperty = property.FindPropertyRelative(nameof(VariableDefinition.Name));
			var typeProperty = property.FindPropertyRelative(nameof(VariableDefinition.Type));
			var initializerProperty = property.FindPropertyRelative(nameof(VariableDefinition.Initializer)).FindPropertyRelative("_statement");
			var tagProperty = property.FindPropertyRelative(nameof(VariableDefinition.Tag));
			var useRangeConstraintProperty = property.FindPropertyRelative(nameof(VariableDefinition.UseRangeConstraint));
			var minimumConstraintProperty = property.FindPropertyRelative(nameof(VariableDefinition.MinimumConstraint));
			var maximumConstraintProperty = property.FindPropertyRelative(nameof(VariableDefinition.MaximumConstraint));
			var typeConstraintProperty = property.FindPropertyRelative(nameof(VariableDefinition.TypeConstraint));

			_expression.SetStatement(initializerProperty.stringValue);

			var definition = VariableDefinition.Create(
				nameProperty.stringValue,
				(VariableType)typeProperty.enumValueIndex,
				useRangeConstraintProperty.boolValue,
				minimumConstraintProperty.floatValue,
				maximumConstraintProperty.floatValue,
				typeConstraintProperty.stringValue,
				tagProperty.stringValue,
				_expression);

			Draw(position, definition, initializer, tags);

			nameProperty.stringValue = definition.Name;
			typeProperty.enumValueIndex = (int)definition.Type;
			initializerProperty.stringValue = definition.Initializer != null ? definition.Initializer.Statement : string.Empty;
			tagProperty.stringValue = definition.Tag;
			useRangeConstraintProperty.boolValue = definition.UseRangeConstraint;
			minimumConstraintProperty.floatValue = definition.MinimumConstraint;
			maximumConstraintProperty.floatValue = definition.MaximumConstraint;
			typeConstraintProperty.stringValue = definition.TypeConstraint;
		}

		public static VariableDefinition Draw(Rect position, VariableDefinition definition, VariableInitializerType initializer, TagList tags)
		{
			var type = definition.Type;
			var tag = definition.Tag;
			var constraint = new Constraint(definition);

			var hasInitializer = HasInitializer(definition.Type, initializer);
			var hasConstraint = HasConstraint(definition.Type);
			var hasTag = HasTags(tags);

			var typeRect = RectHelper.TakeLine(ref position);
			DrawType(typeRect, definition);

			if (hasInitializer && definition.Initializer != null)
			{
				if (initializer == VariableInitializerType.Expression)
				{
					var initializerHeight = ExpressionControl.GetHeight(definition.Initializer, true);
					var initializerRect = RectHelper.TakeHeight(ref position, initializerHeight);
					RectHelper.TakeVerticalSpace(ref position);
					DrawInitializer(initializerRect, ref definition);
				}
				else if (initializer == VariableInitializerType.DefaultValue)
				{
					var initializerRect = RectHelper.TakeLine(ref position);
					DrawDefaultValue(initializerRect, ref definition);
				}
			}

			if (hasConstraint)
			{
				var constraintRect = RectHelper.TakeLine(ref position);
				constraint = DrawConstraint(constraintRect, type, constraint);
			}

			if (hasTag)
			{
				var tagRect = RectHelper.TakeLine(ref position);
				tag = DrawTag(tagRect, tag, tags);
			}

			return VariableDefinition.Create(definition.Name, type, constraint.UseRangeConstraint, constraint.MinimumConstraint, constraint.MaximumConstraint, constraint.TypeConstraint, tag, definition.Initializer);
		}

		private static bool HasInitializer(VariableType type, VariableInitializerType initializer)
		{
			return initializer != VariableInitializerType.None && (type == VariableType.Bool || type == VariableType.Int || type == VariableType.Float || type == VariableType.String);
		}

		private static bool HasConstraint(VariableType type)
		{
			return type == VariableType.Int || type == VariableType.Float || type == VariableType.String || type == VariableType.Object;
		}

		private static bool HasTags(TagList tags)
		{
			return tags != null && tags.Count > 0;
		}

		private static void DrawIndentedLabel(ref Rect rect, GUIContent label)
		{
			var labelRect = RectHelper.TakeWidth(ref rect, _labelWidth);
			RectHelper.TakeWidth(ref labelRect, _labelIndent);
			EditorGUI.LabelField(labelRect, label);
		}

		private static void DrawType(Rect position, VariableDefinition definition)
		{
			var name = string.Format("{0} ({1})", definition.Name, definition.Type);
			EditorGUI.LabelField(position, name);
		}

		private static void DrawInitializer(Rect rect, ref VariableDefinition definition)
		{
			DrawIndentedLabel(ref rect, _initializerLabel);
			ExpressionControl.DrawFoldout(rect, definition.Initializer, GUIContent.none);
		}

		private static void DrawDefaultValue(Rect rect, ref VariableDefinition definition)
		{
			var value = definition.Initializer.Execute(null, null); // context isn't necessary since the object that would be the context is currently drawing

			DrawIndentedLabel(ref rect, _defaultLabel);

			using (var changes = new EditorGUI.ChangeCheckScope())
			{
				value = VariableValueDrawer.Draw(rect, GUIContent.none, value, definition);

				if (changes.changed)
				{
					switch (definition.Type)
					{
						case VariableType.Bool: definition.Initializer.SetStatement(value.Bool ? "true" : "false"); break;
						case VariableType.Int: definition.Initializer.SetStatement(value.Int.ToString()); break;
						case VariableType.Float: definition.Initializer.SetStatement(value.Float.ToString()); break;
						case VariableType.String: definition.Initializer.SetStatement("\"" + value.String + "\""); break;
					}
				}
			}
		}

		private struct Constraint
		{
			public bool UseRangeConstraint;
			public float MinimumConstraint;
			public float MaximumConstraint;
			public string TypeConstraint;

			public Constraint(VariableDefinition definition)
			{
				UseRangeConstraint = definition.UseRangeConstraint;
				MinimumConstraint = definition.MinimumConstraint;
				MaximumConstraint = definition.MaximumConstraint;
				TypeConstraint = definition.TypeConstraint;
			}
		}

		private static Constraint DrawConstraint(Rect rect, VariableType type, Constraint constraint)
		{
			switch (type)
			{
				case VariableType.Int:
				case VariableType.Float:
				{
					DrawIndentedLabel(ref rect, _numberConstraintLabel);

					var fromLabel = _minimumConstraintLabel;
					var toLabel = _maximumConstraintLabel;

					var fromSize = EditorStyles.label.CalcSize(fromLabel);
					var toSize = EditorStyles.label.CalcSize(toLabel);
					var spacing = 5.0f;

					var inputWidth = (rect.width - rect.height - fromSize.x - toSize.x - spacing * 4) * 0.5f;

					var checkboxRect = new Rect(rect.x, rect.y, rect.height, rect.height);
					var fromRect = new Rect(checkboxRect.xMax + spacing, rect.y, fromSize.x, rect.height);
					var minimumRect = new Rect(fromRect.xMax + spacing, rect.y, inputWidth, rect.height);
					var toRect = new Rect(minimumRect.xMax + spacing, rect.y, toSize.x, rect.height);
					var maximumRect = new Rect(toRect.xMax + spacing, rect.y, inputWidth, rect.height);

					constraint.UseRangeConstraint = GUI.Toggle(checkboxRect, constraint.UseRangeConstraint, _useRangeConstraintLabel);

					if (constraint.UseRangeConstraint)
					{
						GUI.Label(fromRect, fromLabel);
						constraint.MinimumConstraint = EditorGUI.FloatField(minimumRect, constraint.MinimumConstraint);
						GUI.Label(toRect, toLabel);
						constraint.MaximumConstraint = EditorGUI.FloatField(maximumRect, constraint.MaximumConstraint);
					}

					break;
				}
				case VariableType.String:
				{
					DrawIndentedLabel(ref rect, _stringConstraintLabel);
					constraint.TypeConstraint = EditorGUI.TextField(rect, constraint.TypeConstraint);
					break;
				}
				case VariableType.Object:
				{
					DrawIndentedLabel(ref rect, _objectConstraintLabel);

					var typeConstraint = !string.IsNullOrEmpty(constraint.TypeConstraint) ? Type.GetType(constraint.TypeConstraint) : null;
					var selectedConstraint = TypePopupDrawer.Draw<Object>(rect, GUIContent.none, typeConstraint, false);

					constraint.TypeConstraint = selectedConstraint != null ? selectedConstraint.AssemblyQualifiedName : string.Empty;

					break;
				}
			}

			return constraint;
		}

		private static string DrawTag(Rect rect, string tag, TagList tags)
		{
			DrawIndentedLabel(ref rect, _tagLabel);

			var tagIndex = tags.IndexOf(tag);
			tagIndex = EditorGUI.Popup(rect, tagIndex, tags.ToArray());
			return tagIndex >= 0 ? tags[tagIndex] : string.Empty;
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return GetHeight(property, VariableInitializerType.Expression, null);
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			Draw(position, property, VariableInitializerType.Expression, null);
		}
	}
}
