using System;
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
		private readonly static GUIContent _enumConstraintLabel = new GUIContent("Constraint", "The enum type of values added to the list");
		private readonly static GUIContent _listConstraintLabel = new GUIContent("Constraint", "The variable type of values added to the list");
		private readonly static GUIContent _tagLabel = new GUIContent("Tag", "An identifier that can be used to reset or persist this variable");
		private readonly static GUIContent _useRangeConstraintLabel = new GUIContent();
		private readonly static GUIContent _minimumConstraintLabel = new GUIContent("Between");
		private readonly static GUIContent _maximumConstraintLabel = new GUIContent("and");

		private static Expression _expression = new Expression();
		private const float _labelWidth = 120.0f;
		private const float _labelIndent = 12.0f;

		public static float GetHeight(VariableDefinition definition, VariableInitializerType initializerType, TagList tags)
		{
			var height = EditorGUIUtility.singleLineHeight;

			if (HasConstraint(definition.Type, definition.Constraint, definition.IsConstraintLocked))
				height += GetConstraintHeight(definition.Type, definition.Constraint);

			if (HasInitializer(definition.Type, initializerType))
			{
				if (initializerType == VariableInitializerType.Expression && definition.Initializer != null)
					height += ExpressionControl.GetHeight(definition.Initializer, true) + RectHelper.VerticalSpace;
				else
					height += RectHelper.LineHeight;
			}

			if (HasTags(tags))
				height += RectHelper.LineHeight;

			return height;
		}

		public static VariableDefinition Draw(VariableDefinition definition, VariableInitializerType initializer, TagList tags)
		{
			var rect = EditorGUILayout.GetControlRect(false, GetHeight(definition, initializer, tags));
			return Draw(rect, definition, initializer, tags);
		}

		public static VariableDefinition Draw(Rect position, VariableDefinition definition, VariableInitializerType initializer, TagList tags)
		{
			var tag = definition.Tag;
			var constraint = definition.Constraint;

			var hasInitializer = HasInitializer(definition.Type, initializer);
			var hasConstraint = HasConstraint(definition.Type, definition.Constraint, definition.IsConstraintLocked);
			var hasTag = HasTags(tags);

			var typeRect = RectHelper.TakeLine(ref position);

			if (!string.IsNullOrEmpty(definition.Name))
			{
				var labelRect = RectHelper.TakeWidth(ref typeRect, _labelWidth);

				EditorGUI.LabelField(labelRect, definition.Name);
			}

			var type = DrawType(typeRect, definition.IsTypeLocked, definition.Type);

			if (hasConstraint || !definition.IsConstraintLocked)
			{
				var constraintHeight = GetConstraintHeight(definition.Type, definition.Constraint);
				var constraintRect = RectHelper.TakeHeight(ref position, constraintHeight);

				DrawConstraint(constraintRect, type, definition.IsConstraintLocked, ref constraint, true);
			}

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

			if (hasTag)
			{
				var tagRect = RectHelper.TakeLine(ref position);
				tag = DrawTag(tagRect, tag, tags);
			}

			return VariableDefinition.Create(definition.Name, type, constraint, tag, definition.Initializer, definition.IsTypeLocked, definition.IsConstraintLocked);
		}

		private static bool HasConstraint(VariableType type, VariableConstraint constraint, bool isConstraintLocked)
		{
			if (!isConstraintLocked)
			{
				return type == VariableType.Int
					|| type == VariableType.Float
					|| type == VariableType.String
					|| type == VariableType.Enum
					|| type == VariableType.Object
					|| type == VariableType.List;
			}
			else
			{
				return constraint != null;
			}
		}

		private static bool HasInitializer(VariableType type, VariableInitializerType initializer)
		{
			return initializer != VariableInitializerType.None && (type == VariableType.Bool || type == VariableType.Int || type == VariableType.Float || type == VariableType.String);
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

		private static VariableType DrawType(Rect position, bool isTypeLocked, VariableType type)
		{
			using (new EditorGUI.DisabledScope(isTypeLocked))
				return (VariableType)EditorGUI.EnumPopup(position, type);
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

		private static float GetConstraintHeight(VariableType type, VariableConstraint constraint)
		{
			var height = RectHelper.LineHeight;

			if (type == VariableType.List && constraint is ListVariableConstraint listConstraint)
			{
				if (HasConstraint(listConstraint.ItemType, listConstraint.ItemConstraint, false))
					height += GetConstraintHeight(listConstraint.ItemType, listConstraint.ItemConstraint);
			}

			return height;
		}

		private static void DrawConstraint(Rect rect, VariableType type, bool isConstraintLocked, ref VariableConstraint constraint, bool top)
		{
			using (new EditorGUI.DisabledScope(isConstraintLocked))
			{
				switch (type)
				{
					case VariableType.Int:
					case VariableType.Float:
					{
						if (top)
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

						var useRangeConstraint = GUI.Toggle(checkboxRect, constraint != null, _useRangeConstraintLabel);

						if (!useRangeConstraint)
						{
							constraint = null;
						}
						else if (type == VariableType.Int)
						{
							if (!(constraint is IntVariableConstraint intConstraint))
							{
								intConstraint = new IntVariableConstraint { Minimum = 0, Maximum = 100 };
								constraint = intConstraint;
							}

							GUI.Label(fromRect, fromLabel);
							intConstraint.Minimum = EditorGUI.IntField(minimumRect, intConstraint.Minimum);
							GUI.Label(toRect, toLabel);
							intConstraint.Maximum = EditorGUI.IntField(maximumRect, intConstraint.Maximum);
						}
						else if (type == VariableType.Float)
						{
							if (!(constraint is FloatVariableConstraint floatConstraint))
							{
								floatConstraint = new FloatVariableConstraint { Minimum = 0, Maximum = 100 };
								constraint = floatConstraint;
							}

							GUI.Label(fromRect, fromLabel);
							floatConstraint.Minimum = EditorGUI.FloatField(minimumRect, floatConstraint.Minimum);
							GUI.Label(toRect, toLabel);
							floatConstraint.Maximum = EditorGUI.FloatField(maximumRect, floatConstraint.Maximum);
						}

						break;
					}
					case VariableType.String:
					{
						if (top)
							DrawIndentedLabel(ref rect, _stringConstraintLabel);

						//constraint.TypeConstraint = EditorGUI.TextField(rect, constraint.TypeConstraint);
						break;
					}
					case VariableType.Object:
					{
						if (top)
							DrawIndentedLabel(ref rect, _objectConstraintLabel);

						if (!(constraint is ObjectVariableConstraint objectConstraint))
						{
							objectConstraint = new ObjectVariableConstraint { Type = typeof(Object) };
							constraint = objectConstraint;
						}

						objectConstraint.Type = TypePopupDrawer.Draw<Object>(rect, GUIContent.none, objectConstraint.Type, false);

						break;
					}
					case VariableType.Enum:
					{
						if (top)
							DrawIndentedLabel(ref rect, _enumConstraintLabel);

						if (!(constraint is EnumVariableConstraint enumConstraint))
						{
							enumConstraint = new EnumVariableConstraint { Type = null };
							constraint = enumConstraint;
						}

						enumConstraint.Type = TypePopupDrawer.Draw<Enum>(rect, GUIContent.none, enumConstraint.Type, false);

						break;
					}
					case VariableType.List:
					{
						if (top)
							DrawIndentedLabel(ref rect, _listConstraintLabel);

						if (!(constraint is ListVariableConstraint listConstraint))
						{
							listConstraint = new ListVariableConstraint { ItemType = VariableType.Empty, ItemConstraint = null };
							constraint = listConstraint;
						}

						var typeRect = RectHelper.TakeLine(ref rect);

						listConstraint.ItemType = (VariableType)EditorGUI.EnumPopup(typeRect, listConstraint.ItemType);

						if (HasConstraint(listConstraint.ItemType, listConstraint.ItemConstraint, false))
							DrawConstraint(rect, listConstraint.ItemType, false, ref listConstraint.ItemConstraint, false);

						break;
					}
				}
			}
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
			var nameProperty = property.FindPropertyRelative("_name");
			var typeProperty = property.FindPropertyRelative("_type");
			var constraintProperty = property.FindPropertyRelative("_constraint");
			var isTypeLockedProperty = property.FindPropertyRelative("_isTypeLocked");
			var isConstraintLockedProperty = property.FindPropertyRelative("_isConstraintLocked");

			var name = nameProperty.stringValue;
			var type = (VariableType)typeProperty.enumValueIndex;
			var constraint = VariableHandler.Get((VariableType)typeProperty.enumValueIndex).CreateConstraint(constraintProperty.stringValue);
			var definition = VariableDefinition.Create(name, type, constraint, null, null, isTypeLockedProperty.boolValue, isConstraintLockedProperty.boolValue);

			return GetHeight(definition, VariableInitializerType.None, null);
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var nameProperty = property.FindPropertyRelative("_name");
			var typeProperty = property.FindPropertyRelative("_type");
			var constraintProperty = property.FindPropertyRelative("_constraint");
			var isTypeLockedProperty = property.FindPropertyRelative("_isTypeLocked");
			var isConstraintLockedProperty = property.FindPropertyRelative("_isConstraintLocked");

			var name = nameProperty.stringValue;
			var type = (VariableType)typeProperty.enumValueIndex;
			var constraint = VariableHandler.Get((VariableType)typeProperty.enumValueIndex).CreateConstraint(constraintProperty.stringValue);
			var definition = VariableDefinition.Create(name, type, constraint, null, null, isTypeLockedProperty.boolValue, isConstraintLockedProperty.boolValue);

			definition = Draw(position, definition, VariableInitializerType.None, null);

			nameProperty.stringValue = definition.Name;
			typeProperty.enumValueIndex = (int)definition.Type;
			constraintProperty.stringValue = definition.Constraint != null ? definition.Constraint.Write() : string.Empty;
		}
	}
}
