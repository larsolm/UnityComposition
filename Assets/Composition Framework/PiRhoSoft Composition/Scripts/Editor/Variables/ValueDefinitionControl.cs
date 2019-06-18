using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using PiRhoSoft.CompositionEngine;
using PiRhoSoft.PargonUtilities.Editor;
using PiRhoSoft.UtilityEditor;
using PiRhoSoft.UtilityEngine;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PiRhoSoft.CompositionEditor
{
	public class ValueDefinitionControl : PropertyControl
	{
		private readonly static GUIContent _initializerLabel = new GUIContent("Initializer", "The Expression to execute and assign when initializing, resetting, or updating the variable");
		private readonly static GUIContent _defaultLabel = new GUIContent("Default", "The default value to use for the variable");

		private readonly static GUIContent _numberConstraintLabel = new GUIContent("Constraint", "The range of values allowed for the variable");
		private readonly static GUIContent _stringConstraintLabel = new GUIContent("Constraint", "The comma separated list of valid values for the variable");
		private readonly static GUIContent _objectConstraintLabel = new GUIContent("Constraint", "The Object type that the assigned object must be derived from or have an instance of");
		private readonly static GUIContent _enumConstraintLabel = new GUIContent("Constraint", "The enum type of values added to the list");
		private readonly static GUIContent _listConstraintLabel = new GUIContent("Constraint", "The variable type of values added to the list");
		private readonly static GUIContent _storeConstraintLabel = new GUIContent("Constraint", "The schema the store must use");
		private readonly static GUIContent _tagLabel = new GUIContent("Tag", "An identifier that can be used to reset or persist this variable");
		private readonly static GUIContent _useRangeConstraintLabel = new GUIContent();
		private readonly static GUIContent _minimumConstraintLabel = new GUIContent("Between");
		private readonly static GUIContent _maximumConstraintLabel = new GUIContent("and");

		private static readonly Label _addButton = new Label(Icon.Add, "", "Add a string to this constraint");
		private static readonly Label _removeButton = new Label(Icon.Remove, "", "Remove this string constraint");

		private static Expression _expression = new Expression();

		#region Static Interface

		public static float GetHeight(ValueDefinition definition, VariableInitializerType initializerType, TagList tags, bool isExpanded)
		{
			var height = EditorGUIUtility.singleLineHeight;

			if (HasConstraint(definition.Type, definition.Constraint, definition.IsConstraintLocked))
				height += GetConstraintHeight(definition.Type, definition.Constraint);

			if (HasInitializer(definition.Type, initializerType))
			{
				if (initializerType == VariableInitializerType.Expression && definition.Initializer != null)
					height += ExpressionControl.GetFoldoutHeight(definition.Initializer, isExpanded, true, 2, 3) + RectHelper.VerticalSpace;
				else
					height += RectHelper.LineHeight;
			}

			if (HasTags(tags))
				height += RectHelper.LineHeight;

			return height;
		}

		public static ValueDefinition Draw(GUIContent label, ValueDefinition definition, VariableInitializerType initializer, TagList tags, bool showConstraintLabel, ref bool isExpanded)
		{
			var rect = EditorGUILayout.GetControlRect(false, GetHeight(definition, initializer, tags, false));
			return Draw(rect, label, definition, initializer, tags, showConstraintLabel, ref isExpanded);
		}

		public static ValueDefinition Draw(Rect position, GUIContent label, ValueDefinition definition, VariableInitializerType initializer, TagList tags, bool showConstraintLabel, ref bool isExpanded)
		{
			var tag = definition.Tag;
			var constraint = definition.Constraint;

			var hasInitializer = HasInitializer(definition.Type, initializer);
			var hasConstraint = HasConstraint(definition.Type, definition.Constraint, definition.IsConstraintLocked);
			var hasTag = HasTags(tags);

			var typeRect = RectHelper.TakeLine(ref position);

			if (label != GUIContent.none)
			{
				var labelRect = RectHelper.TakeWidth(ref typeRect, RectHelper.CurrentLabelWidth);
				EditorGUI.LabelField(labelRect, label);
			}

			var type = DrawType(typeRect, definition.IsTypeLocked, definition.Type);

			if (hasConstraint)
			{
				var constraintHeight = GetConstraintHeight(definition.Type, definition.Constraint);
				var constraintRect = RectHelper.TakeHeight(ref position, constraintHeight);

				DrawConstraint(constraintRect, type, definition.IsConstraintLocked, ref constraint, showConstraintLabel);
			}

			if (hasInitializer && definition.Initializer != null)
			{
				if (initializer == VariableInitializerType.Expression)
				{
					var initializerHeight = ExpressionControl.GetFoldoutHeight(definition.Initializer, isExpanded, true, 2, 3);
					var initializerRect = RectHelper.TakeHeight(ref position, initializerHeight);
					RectHelper.TakeVerticalSpace(ref position);
					DrawInitializer(initializerRect, ref definition, ref isExpanded);
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

			return ValueDefinition.Create(type, constraint, tag, definition.Initializer, definition.IsTypeLocked, definition.IsConstraintLocked);
		}

		#endregion

		#region Flags

		private static bool HasConstraint(VariableType type, VariableConstraint constraint, bool isConstraintLocked)
		{
			if (!isConstraintLocked)
			{
				switch (type)
				{
					case VariableType.Int:
					case VariableType.Float:
					case VariableType.String:
					case VariableType.Enum:
					case VariableType.Object:
					case VariableType.Store:
					case VariableType.List: return true;
					default: return false;
				}
			}
			else
			{
				return constraint != null;
			}
		}

		private static bool HasInitializer(VariableType type, VariableInitializerType initializer)
		{
			if (initializer == VariableInitializerType.None)
				return false;

			switch (type)
			{
				case VariableType.Bool:
				case VariableType.Float:
				case VariableType.Int:
				case VariableType.Int2:
				case VariableType.Int3:
				case VariableType.IntRect:
				case VariableType.IntBounds:
				case VariableType.Vector2:
				case VariableType.Vector3:
				case VariableType.Vector4:
				case VariableType.Quaternion:
				case VariableType.Rect:
				case VariableType.Bounds:
				case VariableType.Color:
				case VariableType.String: return true;
				default: return false;
			}
		}

		private static bool HasTags(TagList tags)
		{
			return tags != null && tags.Count > 0;
		}

		#endregion

		#region Drawing

		private static void DrawIndentedLabel(ref Rect rect, GUIContent label)
		{
			var labelRect = RectHelper.TakeWidth(ref rect, RectHelper.CurrentLabelWidth);
			RectHelper.TakeWidth(ref labelRect, RectHelper.Indent);
			EditorGUI.LabelField(labelRect, label);
		}

		private static VariableType DrawType(Rect position, bool isTypeLocked, VariableType type)
		{
			using (new EditorGUI.DisabledScope(isTypeLocked))
				return (VariableType)EditorGUI.EnumPopup(position, type);
		}

		private static void DrawInitializer(Rect rect, ref ValueDefinition definition, ref bool isExpanded)
		{
			ExpressionControl.DrawFoldout(rect, definition.Initializer, _initializerLabel, ref isExpanded, true);
		}

		private static void DrawDefaultValue(Rect rect, ref ValueDefinition definition)
		{
			var value = definition.Initializer.Execute(null, null); // context isn't necessary since the object that would be the context is currently drawing

			if (value.IsEmpty) // If the initializer hasn't been set, use the default value.
				value = VariableHandler.CreateDefault(definition.Type, definition.Constraint);

			DrawIndentedLabel(ref rect, _defaultLabel);

			using (var changes = new EditorGUI.ChangeCheckScope())
			{
				value = VariableValueDrawer.Draw(rect, GUIContent.none, value, definition, true);

				if (changes.changed)
				{
					switch (definition.Type)
					{
						case VariableType.Bool: definition.Initializer.SetStatement(value.Bool ? "true" : "false"); break;
						case VariableType.Float: definition.Initializer.SetStatement(value.Float.ToString()); break;
						case VariableType.Int: definition.Initializer.SetStatement(value.Int.ToString()); break;
						case VariableType.Int2: definition.Initializer.SetStatement(string.Format("Vector2Int({0}, {1})", value.Int2.x, value.Int2.y)); break;
						case VariableType.Int3: definition.Initializer.SetStatement(string.Format("Vector3Int({0}, {1}, {2})", value.Int3.x, value.Int3.y, value.Int3.z)); break;
						case VariableType.IntRect: definition.Initializer.SetStatement(string.Format("RectInt({0}, {1}, {2}, {3})", value.IntRect.x, value.IntRect.y, value.IntRect.width, value.IntRect.height)); break;
						case VariableType.IntBounds: definition.Initializer.SetStatement(string.Format("BoundsInt({0}, {1}, {2}, {3}, {4}, {5})", value.IntBounds.x, value.IntBounds.y, value.IntBounds.z, value.IntBounds.size.x, value.IntBounds.size.y, value.IntBounds.size.z)); break;
						case VariableType.Vector2: definition.Initializer.SetStatement(string.Format("Vector2({0}, {1})", value.Vector2.x, value.Vector2.y)); break;
						case VariableType.Vector3: definition.Initializer.SetStatement(string.Format("Vector3({0}, {1}, {2})", value.Vector3.x, value.Vector3.y, value.Vector3.z)); break;
						case VariableType.Vector4: definition.Initializer.SetStatement(string.Format("Vector4({0}, {1}, {2}, {3})", value.Vector4.x, value.Vector4.y, value.Vector4.z, value.Vector4.w)); break;
						case VariableType.Quaternion: var euler = value.Quaternion.eulerAngles; definition.Initializer.SetStatement(string.Format("Quaternion({0}, {1}, {2})", euler.x, euler.y, euler.z)); break;
						case VariableType.Rect: definition.Initializer.SetStatement(string.Format("Rect({0}, {1}, {2}, {3})", value.Rect.x, value.Rect.y, value.Rect.width, value.Rect.height)); break;
						case VariableType.Bounds: definition.Initializer.SetStatement(string.Format("Bounds({0}, {1})", value.Bounds.center, value.Bounds.extents)); break;
						case VariableType.Color: definition.Initializer.SetStatement(string.Format("#{0:X2}{1:X2}{2:X2}{3:X2}", Mathf.RoundToInt(value.Color.r * 255), Mathf.RoundToInt(value.Color.g * 255), Mathf.RoundToInt(value.Color.b * 255), Mathf.RoundToInt(value.Color.a * 255))); break;
						case VariableType.String: definition.Initializer.SetStatement("\"" + value.String + "\""); break;
					}
				}
			}
		}

		#endregion

		#region Constraints

		private static float GetConstraintHeight(VariableType type, VariableConstraint constraint)
		{
			var height = RectHelper.LineHeight;

			if (type == VariableType.List && constraint is ListVariableConstraint listConstraint)
			{
				if (HasConstraint(listConstraint.ItemType, listConstraint.ItemConstraint, false))
					height += GetConstraintHeight(listConstraint.ItemType, listConstraint.ItemConstraint);
			}
			else if (type == VariableType.String && constraint is StringVariableConstraint stringConstraint)
			{
				height = (1 + stringConstraint.Values.Length) * RectHelper.LineHeight;
			}

			return height;
		}

		private static void DrawConstraint(Rect rect, VariableType type, bool isConstraintLocked, ref VariableConstraint constraint, bool top)
		{
			RectHelper.TakeTrailingHeight(ref rect, RectHelper.VerticalSpace);

			using (new EditorGUI.DisabledScope(isConstraintLocked))
			{
				switch (type)
				{
					case VariableType.Int:
					case VariableType.Float:
					{
						if (top) DrawIndentedLabel(ref rect, _numberConstraintLabel);
						DrawNumberConstraint(rect, type, ref constraint);
						break;
					}
					case VariableType.String:
					{
						if (top) DrawIndentedLabel(ref rect, _stringConstraintLabel);
						DrawStringConstraint(rect, ref constraint);
						break;
					}
					case VariableType.Object:
					{
						if (top) DrawIndentedLabel(ref rect, _objectConstraintLabel);
						DrawObjectConstraint(rect, ref constraint);
						break;
					}
					case VariableType.Enum:
					{
						if (top) DrawIndentedLabel(ref rect, _enumConstraintLabel);
						DrawEnumConstraint(rect, ref constraint);
						break;
					}
					case VariableType.Store:
					{
						if (top) DrawIndentedLabel(ref rect, _storeConstraintLabel);
						DrawStoreConstraint(rect, ref constraint);
						break;
					}
					case VariableType.List:
					{
						if (top) DrawIndentedLabel(ref rect, _listConstraintLabel);
						DrawListConstraint(rect, ref constraint);
						break;
					}
				}
			}
		}

		private static void DrawNumberConstraint(Rect rect, VariableType type, ref VariableConstraint constraint)
		{
			var fromLabel = _minimumConstraintLabel;
			var toLabel = _maximumConstraintLabel;

			var fromSize = EditorStyles.label.CalcSize(fromLabel);
			var toSize = EditorStyles.label.CalcSize(toLabel);
			var spacing = 5.0f;

			var inputWidth = (rect.width - rect.height - fromSize.x - toSize.x - spacing * 4) * 0.5f;

			var checkboxRect =	RectHelper.TakeWidth(ref rect, rect.height);
								RectHelper.TakeWidth(ref rect, spacing);
			var fromRect =		RectHelper.TakeWidth(ref rect, fromSize.x);
								RectHelper.TakeWidth(ref rect, spacing);
			var minimumRect =	RectHelper.TakeWidth(ref rect, inputWidth);
								RectHelper.TakeWidth(ref rect, spacing);
			var toRect =		RectHelper.TakeWidth(ref rect, toSize.x);
								RectHelper.TakeWidth(ref rect, spacing);
			var maximumRect =	RectHelper.TakeWidth(ref rect, inputWidth);

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
		}

		private static void DrawStringConstraint(Rect rect, ref VariableConstraint constraint)
		{
			if (!(constraint is StringVariableConstraint stringConstraint))
			{
				stringConstraint = new StringVariableConstraint { Values = new string[] { } };
				constraint = stringConstraint;
			}

			var remove = -1;

			for (var i = 0; i < stringConstraint.Values.Length; i++)
			{
				if (i != 0)
					RectHelper.TakeVerticalSpace(ref rect);

				var item = stringConstraint.Values[i];
				var itemRect = RectHelper.TakeLine(ref rect);
				var removeRect = RectHelper.TakeTrailingIcon(ref itemRect);

				stringConstraint.Values[i] = EditorGUI.TextField(itemRect, item);

				if (GUI.Button(removeRect, _removeButton.Content, GUIStyle.none))
					remove = i;
			}

			var addRect = RectHelper.TakeTrailingIcon(ref rect);

			if (GUI.Button(addRect, _addButton.Content, GUIStyle.none))
				ArrayUtility.Add(ref stringConstraint.Values, string.Empty);

			if (remove >= 0)
				ArrayUtility.RemoveAt(ref stringConstraint.Values, remove);
		}

		private static void DrawObjectConstraint(Rect rect, ref VariableConstraint constraint)
		{
			if (!(constraint is ObjectVariableConstraint objectConstraint))
			{
				objectConstraint = new ObjectVariableConstraint { Type = typeof(Object) };
				constraint = objectConstraint;
			}

			objectConstraint.Type = TypeDisplayDrawer.Draw<Object>(rect, GUIContent.none, objectConstraint.Type, false, true);
		}

		private static void DrawEnumConstraint(Rect rect, ref VariableConstraint constraint)
		{
			if (!(constraint is EnumVariableConstraint enumConstraint))
			{
				enumConstraint = new EnumVariableConstraint { Type = null };
				constraint = enumConstraint;
			}

			enumConstraint.Type = TypeDisplayDrawer.Draw<Enum>(rect, GUIContent.none, enumConstraint.Type, false, true);
		}

		private static void DrawStoreConstraint(Rect rect, ref VariableConstraint constraint)
		{
			if (!(constraint is StoreVariableConstraint storeConstraint))
			{
				storeConstraint = new StoreVariableConstraint { Schema = null };
				constraint = storeConstraint;
			}

			storeConstraint.Schema = AssetDisplayDrawer.Draw(rect, GUIContent.none, storeConstraint.Schema, false, false, AssetLocation.None, null);
		}

		private static void DrawListConstraint(Rect rect, ref VariableConstraint constraint)
		{
			if (!(constraint is ListVariableConstraint listConstraint))
			{
				listConstraint = new ListVariableConstraint { ItemType = VariableType.Empty, ItemConstraint = null };
				constraint = listConstraint;
			}

			var typeRect = RectHelper.TakeLine(ref rect);

			listConstraint.ItemType = (VariableType)EditorGUI.EnumPopup(typeRect, listConstraint.ItemType);

			if (HasConstraint(listConstraint.ItemType, listConstraint.ItemConstraint, false))
				DrawConstraint(rect, listConstraint.ItemType, false, ref listConstraint.ItemConstraint, false);

		}

		private static string DrawTag(Rect rect, string tag, TagList tags)
		{
			DrawIndentedLabel(ref rect, _tagLabel);

			var tagIndex = tags.IndexOf(tag);
			tagIndex = EditorGUI.Popup(rect, tagIndex, tags.ToArray());
			return tagIndex >= 0 ? tags[tagIndex] : string.Empty;
		}

		#endregion

		#region Drawer Interface

		private SerializedProperty _typeProperty;
		private SerializedProperty _constraintProperty;
		private SerializedProperty _objectsProperty;
		private SerializedProperty _isTypeLockedProperty;
		private SerializedProperty _isConstraintLockedProperty;

		private VariableConstraint _constraint;
		private List<Object> _objects;

		public override void Setup(SerializedProperty property, FieldInfo fieldInfo, PropertyAttribute attribute)
		{
			_typeProperty = property.FindPropertyRelative("_type");
			_constraintProperty = property.FindPropertyRelative("_constraint");
			_objectsProperty = property.FindPropertyRelative("_objects");
			_isTypeLockedProperty = property.FindPropertyRelative("_isTypeLocked");
			_isConstraintLockedProperty = property.FindPropertyRelative("_isConstraintLocked");

			var data = _constraintProperty.stringValue;
			_objects = new List<Object>();

			for (var i = 0; i < _objectsProperty.arraySize; i++)
				_objects.Add(_objectsProperty.GetArrayElementAtIndex(i).objectReferenceValue);

			_constraint = VariableHandler.LoadConstraint(ref data, ref _objects);
		}

		public override float GetHeight(SerializedProperty property, GUIContent label)
		{
			var type = (VariableType)_typeProperty.enumValueIndex;
			var definition = ValueDefinition.Create(type, _constraint, null, null, _isTypeLockedProperty.boolValue, _isConstraintLockedProperty.boolValue);

			return GetHeight(definition, VariableInitializerType.None, null, property.isExpanded);
		}

		public override void Draw(Rect position, SerializedProperty property, GUIContent label)
		{
			var expanded = property.isExpanded;
			var definition = ValueDefinition.Create((VariableType)_typeProperty.enumValueIndex, _constraint, null, null, _isTypeLockedProperty.boolValue, _isConstraintLockedProperty.boolValue);
			definition = Draw(position, label, definition, VariableInitializerType.None, null, true, ref expanded);

			_typeProperty.enumValueIndex = (int)definition.Type;
			_constraint = definition.Constraint;
			_constraintProperty.stringValue = _constraint != null ? VariableHandler.SaveConstraint(definition.Type, _constraint, ref _objects) : string.Empty;

			if (_objects != null)
			{
				_objectsProperty.arraySize = _objects.Count;

				for (var i = 0; i < _objects.Count; i++)
					_objectsProperty.GetArrayElementAtIndex(i).objectReferenceValue = _objects[i];
			}
			else
			{
				_objectsProperty.arraySize = 0;
			}

			property.isExpanded = expanded;
		}

		#endregion
	}

	[CustomPropertyDrawer(typeof(ValueDefinition))]
	public class ValueDefinitionDrawer : PropertyDrawer<ValueDefinitionControl>
	{
	}
}
