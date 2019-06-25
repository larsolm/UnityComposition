using PiRhoSoft.CompositionEngine;
using PiRhoSoft.PargonUtilities.Editor;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace PiRhoSoft.CompositionEditor
{
	public class ValueDefinitionElement : VisualElement, IBindableProperty<ValueDefinition>, IBindableObject<ValueDefinition>
	{
		private VisualElement _typeContainer;
		private VisualElement _constraintContainer;
		private VisualElement _initializerContainer;
		private VisualElement _tagContainer;

		private readonly Object _owner;
		private readonly Func<ValueDefinition> _getValue;
		private readonly Action<ValueDefinition> _setValue;
		private readonly Func<VariableInitializerType> _getInitializerType;
		private readonly Func<TagList> _getTags;
		private readonly bool _showConstrantLabel;

		private ValueDefinition _definition;
		private VariableInitializerType _initializer;
		private TagList _tags;
		
		private readonly SerializedProperty _typeProperty;
		private readonly SerializedProperty _tagProperty;
		private readonly SerializedProperty _initializerProperty;
		private readonly SerializedProperty _constraintProperty;
		private readonly SerializedProperty _objectsProperty;
		private readonly SerializedProperty _isTypeLockedProperty;
		private readonly SerializedProperty _isConstraintLockedProperty;
		private List<Object> _objects;

		public ValueDefinitionElement(SerializedProperty property)
		{
			_owner = property.serializedObject.targetObject;
			_typeProperty = property.FindPropertyRelative("_type");
			_tagProperty = property.FindPropertyRelative("_tag");
			_initializerProperty = property.FindPropertyRelative("_initializer._statement");
			_constraintProperty = property.FindPropertyRelative("_constraint");
			_objectsProperty = property.FindPropertyRelative("_objects");
			_isTypeLockedProperty = property.FindPropertyRelative("_isTypeLocked");
			_isConstraintLockedProperty = property.FindPropertyRelative("_isConstraintLocked");

			var definition = GetValueFromProperty(property);

			Setup(definition, VariableInitializerType.None, null);

			ElementHelper.Bind(this, this, property);
		}

		public ValueDefinitionElement(Object owner, Func<ValueDefinition> getValue, Action<ValueDefinition> setValue, Func<VariableInitializerType> getInitializerType, Func<TagList> getTags, bool showConstraintLabel)
		{
			_owner = owner;
			_getValue = getValue;
			_setValue = setValue;
			_getInitializerType = getInitializerType;
			_getTags = getTags;
			_showConstrantLabel = showConstraintLabel;

			schedule.Execute(() =>
			{
				var initializer = _getInitializerType();
				var tags = _getTags();

				if (initializer != _initializer)
					SetupInitializer(initializer);

				if (tags != _tags) // TODO: pretty sure this needs to be a deep check
					SetupTag(tags);
			}).Every(0);

			Setup(_getValue(), _getInitializerType(), _getTags());

			ElementHelper.Bind(this, this, _owner);
		}

		#region IBindable Property

		public ValueDefinition GetValueFromElement(VisualElement element) => _definition;
		public ValueDefinition GetValueFromObject(Object owner) => _getValue();
		public void UpdateElement(ValueDefinition value, VisualElement element, Object owner) => Setup(value, _initializer, _tags);
		public void UpdateElement(ValueDefinition value, VisualElement element, SerializedProperty property) => Setup(value, _initializer, _tags);
		public void UpdateObject(ValueDefinition value, VisualElement element, Object owner) => _setValue(value);

		public ValueDefinition GetValueFromProperty(SerializedProperty property)
		{
			var data = _constraintProperty.stringValue;
			var objects = new List<Object>();

			for (var i = 0; i < _objectsProperty.arraySize; i++)
				objects.Add(_objectsProperty.GetArrayElementAtIndex(i).objectReferenceValue);

			var constraint = VariableHandler.LoadConstraint(ref data, ref objects);

			var initializer = new Expression();
			initializer.SetStatement(_initializerProperty.stringValue);

			return ValueDefinition.Create((VariableType)_typeProperty.enumValueIndex, constraint, _tagProperty.stringValue, initializer, _isTypeLockedProperty.boolValue, _isConstraintLockedProperty.boolValue);
		}

		public void UpdateProperty(ValueDefinition value, VisualElement element, SerializedProperty property)
		{
			var constraint = value.Constraint;

			_typeProperty.enumValueIndex = (int)value.Type;
			_tagProperty.stringValue = value.Tag;
			_initializerProperty.stringValue = value.Initializer.Statement;
			_isTypeLockedProperty.boolValue = value.IsTypeLocked;
			_isConstraintLockedProperty.boolValue = value.IsConstraintLocked;
			_constraintProperty.stringValue = constraint != null ? VariableHandler.SaveConstraint(value.Type, constraint, ref _objects) : string.Empty;

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
		}

		#endregion

		#region Flags

		private bool HasConstraint(bool isLocked, VariableConstraint constraint, VariableType type)
		{
			if (!isLocked)
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

		private bool HasInitializer(VariableInitializerType initializer)
		{
			if (initializer == VariableInitializerType.None)
				return false;

			switch (_definition.Type)
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

		private bool HasTags(TagList tags)
		{
			return tags != null && tags.Count > 0;
		}

		#endregion

		public void Setup(ValueDefinition definition, VariableInitializerType initializerType, TagList tags)
		{
			Clear();

			_definition = definition;

			SetupType();
			SetupConstraint();
			SetupInitializer(initializerType);
			SetupTag(tags);
		}

		private void ReplaceElement(ref VisualElement oldElement, VisualElement newElement)
		{
			if (oldElement != null)
			{
				var index = IndexOf(oldElement);
				RemoveAt(index);
				Insert(index, newElement);
			}
			else
			{
				Add(newElement);
			}

			oldElement = newElement;
		}

		private void SetupType()
		{
			var container = new VisualElement() { tooltip = "The type of variable this defines" };

			if (_definition.IsTypeLocked)
				container.SetEnabled(false);

			var dropdown = new EnumDropdown(_owner, () => (int)_definition.Type, type =>
			{
				var variableType = (VariableType)type;
				var definition = ValueDefinition.Create(variableType);

				Setup(definition, _initializer, _tags);
			});

			ReplaceElement(ref _typeContainer, container);
		}

		private void SetupInitializer(VariableInitializerType initializer)
		{
			_initializer = initializer;

			var container = new VisualElement();

			if (HasInitializer(initializer) && _definition.Initializer != null)
			{
				if (initializer == VariableInitializerType.Expression)
				{
					container.tooltip = "The Expression to execute and assign when initializing, resetting, or updating the variable";
					container.Add(new Label("Initializer"));

					//ExpressionControl.DrawFoldout(rect, definition.Initializer, _initializerLabel, ref isExpanded, true);
				}
				else if (initializer == VariableInitializerType.DefaultValue)
				{
					container.tooltip = "The default value to use for the variable";
					container.Add(new Label("Default"));
					container.Add(new VariableValueElement(_owner, () =>
					{
						var value = _definition.Initializer.Execute(null, null); // context isn't necessary since the object that would be the context is currently drawing
					if (value.IsEmpty) // If the initializer hasn't been set, use the default value.
						value = VariableHandler.CreateDefault(_definition.Type, _definition.Constraint);

						return value;

					},
					value =>
					{
						switch (_definition.Type)
						{
							case VariableType.Bool: _definition.Initializer.SetStatement(value.Bool ? "true" : "false"); break;
							case VariableType.Float: _definition.Initializer.SetStatement(value.Float.ToString()); break;
							case VariableType.Int: _definition.Initializer.SetStatement(value.Int.ToString()); break;
							case VariableType.Int2: _definition.Initializer.SetStatement(string.Format("Vector2Int({0}, {1})", value.Int2.x, value.Int2.y)); break;
							case VariableType.Int3: _definition.Initializer.SetStatement(string.Format("Vector3Int({0}, {1}, {2})", value.Int3.x, value.Int3.y, value.Int3.z)); break;
							case VariableType.IntRect: _definition.Initializer.SetStatement(string.Format("RectInt({0}, {1}, {2}, {3})", value.IntRect.x, value.IntRect.y, value.IntRect.width, value.IntRect.height)); break;
							case VariableType.IntBounds: _definition.Initializer.SetStatement(string.Format("BoundsInt({0}, {1}, {2}, {3}, {4}, {5})", value.IntBounds.x, value.IntBounds.y, value.IntBounds.z, value.IntBounds.size.x, value.IntBounds.size.y, value.IntBounds.size.z)); break;
							case VariableType.Vector2: _definition.Initializer.SetStatement(string.Format("Vector2({0}, {1})", value.Vector2.x, value.Vector2.y)); break;
							case VariableType.Vector3: _definition.Initializer.SetStatement(string.Format("Vector3({0}, {1}, {2})", value.Vector3.x, value.Vector3.y, value.Vector3.z)); break;
							case VariableType.Vector4: _definition.Initializer.SetStatement(string.Format("Vector4({0}, {1}, {2}, {3})", value.Vector4.x, value.Vector4.y, value.Vector4.z, value.Vector4.w)); break;
							case VariableType.Quaternion: var euler = value.Quaternion.eulerAngles; _definition.Initializer.SetStatement(string.Format("Quaternion({0}, {1}, {2})", euler.x, euler.y, euler.z)); break;
							case VariableType.Rect: _definition.Initializer.SetStatement(string.Format("Rect({0}, {1}, {2}, {3})", value.Rect.x, value.Rect.y, value.Rect.width, value.Rect.height)); break;
							case VariableType.Bounds: _definition.Initializer.SetStatement(string.Format("Bounds({0}, {1})", value.Bounds.center, value.Bounds.extents)); break;
							case VariableType.Color: _definition.Initializer.SetStatement(string.Format("#{0:X2}{1:X2}{2:X2}{3:X2}", Mathf.RoundToInt(value.Color.r * 255), Mathf.RoundToInt(value.Color.g * 255), Mathf.RoundToInt(value.Color.b * 255), Mathf.RoundToInt(value.Color.a * 255))); break;
							case VariableType.String: _definition.Initializer.SetStatement("\"" + value.String + "\""); break;
						}
					},
					() => _definition));
				}
			}

			ReplaceElement(ref _initializerContainer, container);
		}

		private void SetupTag(TagList tags)
		{
			_tags = tags;

			var container = new VisualElement();

			if (HasTags(_tags))
			{
				container.Add(new Label("Tag") { tooltip = "An identifier that can be used to reset or persist this variable" });

				var dropdown = new StringDropdown(_owner, () => _definition.Tag, tag => _definition = ValueDefinition.Create(_definition.Type, _definition.Constraint, tag, _definition.Initializer, _definition.IsTypeLocked, _definition.IsConstraintLocked));
				dropdown.Setup(tags.List, tags.List, _definition.Tag);

				container.Add(dropdown);
			}

			ReplaceElement(ref _tagContainer, container);
		}

		#region Constraints

		private void SetupConstraint()
		{
			var container = new VisualElement();
			var constraint = _definition.Constraint;

			if (HasConstraint(_definition.IsConstraintLocked, constraint, _definition.Type))
				SetupConstraint(container, _definition.IsConstraintLocked, ref constraint, _definition.Type, _showConstrantLabel);

			ReplaceElement(ref _constraintContainer, container);
		}

		private void SetupConstraint(VisualElement container, bool isLocked, ref VariableConstraint constraint, VariableType type, bool showLabel)
		{
			var label = new Label("Constraint");

			if (isLocked)
				container.SetEnabled(false);

			if (showLabel)
				container.Add(label);

			switch (type)
			{
				case VariableType.Int:
				{
					label.tooltip = "The range of values allowed for the variable";
					container.Add(SetupIntConstraint(ref constraint));
					break;
				}
				case VariableType.Float:
				{
					label.tooltip = "The range of values allowed for the variable";
					container.Add(SetupFloatConstraint(ref constraint));
					break;
				}
				case VariableType.String:
				{
					label.tooltip = "The list of valid string values for the variable";
					container.Add(SetupStringConstraint(ref constraint));
					break;
				}
				case VariableType.Object:
				{
					label.tooltip = "The Object type that the assigned object must be derived from or have an instance of";
					container.Add(SetupObjectConstraint(ref constraint));
					break;
				}
				case VariableType.Enum:
				{
					label.tooltip = "The enum type of values added to the list";
					container.Add(SetupEnumConstraint(ref constraint));
					break;
				}
				case VariableType.Store:
				{
					label.tooltip = "The schema the store must use";
					container.Add(SetupStoreConstraint(ref constraint));
					break;
				}
				case VariableType.List:
				{
					label.tooltip = "The variable type of values added to the list";
					container.Add(SetupListConstraint(ref constraint));
					break;
				}
			}
		}

		private VisualElement SetupIntConstraint(ref VariableConstraint constraint)
		{
			if (!(constraint is IntVariableConstraint intConstraint))
			{
				intConstraint = new IntVariableConstraint { HasRange = false, Minimum = 0, Maximum = 100 };
				constraint = intConstraint;
			}

			var container = new VisualElement();
			var toggle = new Toggle() { value = intConstraint.HasRange };
			var intContainer = new VisualElement();
			var intMin = new IntegerField() { value = intConstraint.Minimum };
			var intMax = new IntegerField() { value = intConstraint.Maximum };

			ElementHelper.Bind(this, toggle, _owner, () => intConstraint.HasRange, hasConstraint =>
			{
				intConstraint.HasRange = hasConstraint;
				ElementHelper.SetVisible(intContainer, hasConstraint);
			});

			intContainer.Add(new Label("Between"));
			intContainer.Add(intMin);
			intContainer.Add(new Label("and"));
			intContainer.Add(intMax);

			ElementHelper.Bind(this, intMin, _owner, () => intConstraint.Minimum, minimum => intConstraint.Minimum = minimum);
			ElementHelper.Bind(this, intMax, _owner, () => intConstraint.Maximum, maximum => intConstraint.Maximum = maximum);

			container.Add(toggle);
			container.Add(intContainer);

			return container;
		}

		private VisualElement SetupFloatConstraint(ref VariableConstraint constraint)
		{
			if (!(constraint is FloatVariableConstraint floatConstraint))
			{
				floatConstraint = new FloatVariableConstraint { HasRange = false, Minimum = 0, Maximum = 100 };
				constraint = floatConstraint;
			}

			var container = new VisualElement();
			var toggle = new Toggle() { value = floatConstraint.HasRange };
			var floatContainer = new VisualElement();
			var floatMin = new FloatField() { value = floatConstraint.Minimum };
			var floatMax = new FloatField() { value = floatConstraint.Maximum };

			ElementHelper.Bind(this, toggle, _owner, () => floatConstraint.HasRange, hasConstraint =>
			{
				floatConstraint.HasRange = hasConstraint;
				ElementHelper.SetVisible(floatContainer, hasConstraint);
			});

			floatContainer.Add(new Label("Between"));
			floatContainer.Add(floatMin);
			floatContainer.Add(new Label("and"));
			floatContainer.Add(floatMax);

			ElementHelper.Bind(this, floatMin, _owner, () => floatConstraint.Minimum, minimum => floatConstraint.Minimum = minimum);
			ElementHelper.Bind(this, floatMax, _owner, () => floatConstraint.Maximum, maximum => floatConstraint.Maximum = maximum);

			container.Add(toggle);
			container.Add(floatContainer);

			return container;
		}

		private VisualElement SetupStringConstraint(ref VariableConstraint constraint)
		{
			if (!(constraint is StringVariableConstraint stringConstraint))
			{
				stringConstraint = new StringVariableConstraint { Values = new string[] { } };
				constraint = stringConstraint;
			}

			var container = new VisualElement();
			var proxy = new ArrayListProxy<string>(stringConstraint.Values, (value, index) =>
			{
				var field = new TextField() { value = value };
				ElementHelper.Bind(field, field, _owner, () => stringConstraint.Values[index], text => stringConstraint.Values[index] = text);
				return field;
			});

			container.Add(new ListElement(proxy, "Valid Strings", "The list of valid string values for the variable"));

			return container;
		}

		private VisualElement SetupObjectConstraint(ref VariableConstraint constraint)
		{
			if (!(constraint is ObjectVariableConstraint objectConstraint))
			{
				objectConstraint = new ObjectVariableConstraint { Type = typeof(Object) };
				constraint = objectConstraint;
			}

			var container = new VisualElement();
			var picker = new TypePicker(_owner, () => objectConstraint.Type.AssemblyQualifiedName, type => objectConstraint.Type = Type.GetType(type));
			picker.Setup(typeof(Object), true, objectConstraint.Type?.AssemblyQualifiedName);

			container.Add(picker);

			return container;
		}

		private VisualElement SetupEnumConstraint(ref VariableConstraint constraint)
		{
			if (!(constraint is EnumVariableConstraint enumConstraint))
			{
				enumConstraint = new EnumVariableConstraint { Type = null };
				constraint = enumConstraint;
			}

			var container = new VisualElement();
			var picker = new TypePicker(_owner, () => enumConstraint.Type.AssemblyQualifiedName, type => enumConstraint.Type = Type.GetType(type));
			picker.Setup(typeof(Enum), false, enumConstraint.Type?.AssemblyQualifiedName);

			container.Add(picker);

			return container;
		}

		private VisualElement SetupStoreConstraint(ref VariableConstraint constraint)
		{
			if (!(constraint is StoreVariableConstraint storeConstraint))
			{
				storeConstraint = new StoreVariableConstraint { Schema = null };
				constraint = storeConstraint;
			}
			
			var container = new VisualElement();
			var picker = new ObjectPicker(_owner, () => storeConstraint.Schema, schema => storeConstraint.Schema = schema as VariableSchema);
			picker.Setup(typeof(VariableSchema), storeConstraint.Schema);

			container.Add(picker);

			return container;
		}

		private VisualElement SetupListConstraint(ref VariableConstraint constraint)
		{
			if (!(constraint is ListVariableConstraint listConstraint))
			{
				listConstraint = new ListVariableConstraint { ItemType = VariableType.Empty, ItemConstraint = null };
				constraint = listConstraint;
			}
			
			var container = new VisualElement();
			var dropdown = new EnumDropdown(_owner, () => (int)listConstraint.ItemType, type => listConstraint.ItemType = (VariableType)type);
			dropdown.Setup(typeof(VariableType), (int)listConstraint.ItemType);

			container.Add(dropdown);

			if (HasConstraint(false, listConstraint.ItemConstraint, listConstraint.ItemType))
			{
				var constraintElement = new VisualElement();
				SetupConstraint(container, false, ref listConstraint.ItemConstraint, listConstraint.ItemType, false);
				container.Add(constraintElement);
			}

			return container;
		}

		#endregion
	}
}
