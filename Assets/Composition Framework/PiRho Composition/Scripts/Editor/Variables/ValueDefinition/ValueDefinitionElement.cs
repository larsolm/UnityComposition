using PiRhoSoft.Utilities.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace PiRhoSoft.Composition.Editor
{
	public class ValueDefinitionElement : VisualElement, IBindableProperty<ValueDefinition>, IBindableObject<ValueDefinition>
	{
		private const string _styleSheetPath = Composition.StylePath + "Variables/ValueDefinition/ValueDefinitionElement.uss";
		private const string _ussBaseClass = "pargon-value-definition";
		private const string _ussRowClass = "row";
		private const string _ussConstraintClass = "constraint";
		private const string _ussConstraintInputClass = "input";
		private const string _ussNumberConstraintClass = "number-constraint";

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

			SetupStyle();
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

				if ((tags == null && _tags != null) || (tags != null && !tags.SequenceEqual(_tags)))
					SetupTag(tags);
			}).Every(100);

			SetupStyle();
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

			//var constraint = VariableHandler.LoadConstraint(ref data, ref objects);

			var initializer = new Expression();
			initializer.SetStatement(_initializerProperty.stringValue);

			return ValueDefinition.Create((VariableType)_typeProperty.enumValueIndex);
		}

		public void UpdateProperty(ValueDefinition value, VisualElement element, SerializedProperty property)
		{
			_typeProperty.enumValueIndex = (int)value.Type;
			_tagProperty.stringValue = value.Tag;
			_initializerProperty.stringValue = value.Initializer.Statement;
			//_isTypeLockedProperty.boolValue = value.IsTypeLocked;
			//_isConstraintLockedProperty.boolValue = value.IsConstraintLocked;
			//_constraintProperty.stringValue = VariableHandler.SaveConstraint(value.Type, value.Constraint, out var objects);
			//_objectsProperty.arraySize = objects?.Count ?? 0;
			
			//for (var i = 0; i < (objects?.Count ?? 0); i++)
			//	_objectsProperty.GetArrayElementAtIndex(i).objectReferenceValue = objects[i];
		}

		#endregion

		#region Flags

		// TODO: delete this when all things have been updated to not be null
		private bool HasConstraint(VariableType type)
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

		private bool HasInitializer(VariableInitializerType initializer)
		{
			if (initializer == VariableInitializerType.None)
				return false;

			switch (_definition.Type)
			{
				case VariableType.Bool:
				case VariableType.Float:
				case VariableType.Int:
				case VariableType.Vector2Int:
				case VariableType.Vector3Int:
				case VariableType.RectInt:
				case VariableType.BoundsInt:
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

		private void SetupStyle()
		{
			ElementHelper.AddStyleSheet(this, _styleSheetPath);
			AddToClassList(_ussBaseClass);
		}

		private void Setup(ValueDefinition definition, VariableInitializerType initializerType, TagList tags)
		{
			if (HasConstraint(definition.Type))
				definition = ValueDefinition.Create(definition.Type);//, ValueDefinition.Create(definition.Type), _definition.Tag, _definition.Initializer, _definition.IsTypeLocked, _definition.IsConstraintLocked);

			Reset();
			
			_definition = definition;

			SetupType();
			SetupConstraint();
			SetupInitializer(initializerType);
			SetupTag(tags);
		}

		private void Reset()
		{
			Clear();

			_typeContainer = null;
			_constraintContainer = null;
			_initializerContainer = null;
			_tagContainer = null;
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

		private void SetDefinition(ValueDefinition definition)
		{
			ElementHelper.SendChangeEvent(this, _definition, definition);
			Setup(definition, _initializer, _tags);
		}

		private void SetupType()
		{
			var container = new VisualElement() { tooltip = "The type of variable this defines" };

			//if (_definition.IsTypeLocked)
			//	container.SetEnabled(false);

			var dropdown = new EnumField(_definition.Type);//, _owner, () => (int)_definition.Type, type =>
			//{
			//	var variableType = (VariableType)type;
			//	var constraint = VariableConstraint.Create(variableType);
			//	var definition = ValueDefinition.Create(variableType, constraint, _definition.Tag, _definition.Initializer, _definition.IsTypeLocked, _definition.IsConstraintLocked);
			//
			//	SetDefinition(definition);
			//});

			container.Add(dropdown);

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
						//if (value.IsEmpty) // If the initializer hasn't been set, use the default value.
						//	value = VariableHandler.CreateDefault(_definition.Type, _definition.Constraint);

						return value;
					},
					value =>
					{
						var definition = _definition;
						switch (definition.Type)
						{
							case VariableType.Bool: definition.Initializer.SetStatement(value.Bool ? "true" : "false"); break;
							case VariableType.Float: definition.Initializer.SetStatement(value.Float.ToString()); break;
							case VariableType.Int: definition.Initializer.SetStatement(value.Int.ToString()); break;
							case VariableType.Vector2Int: definition.Initializer.SetStatement(string.Format("Vector2Int({0}, {1})", value.Int2.x, value.Int2.y)); break;
							case VariableType.Vector3Int: definition.Initializer.SetStatement(string.Format("Vector3Int({0}, {1}, {2})", value.Int3.x, value.Int3.y, value.Int3.z)); break;
							case VariableType.RectInt: definition.Initializer.SetStatement(string.Format("RectInt({0}, {1}, {2}, {3})", value.IntRect.x, value.IntRect.y, value.IntRect.width, value.IntRect.height)); break;
							case VariableType.BoundsInt: definition.Initializer.SetStatement(string.Format("BoundsInt({0}, {1}, {2}, {3}, {4}, {5})", value.IntBounds.x, value.IntBounds.y, value.IntBounds.z, value.IntBounds.size.x, value.IntBounds.size.y, value.IntBounds.size.z)); break;
							case VariableType.Vector2: definition.Initializer.SetStatement(string.Format("Vector2({0}, {1})", value.Vector2.x, value.Vector2.y)); break;
							case VariableType.Vector3: definition.Initializer.SetStatement(string.Format("Vector3({0}, {1}, {2})", value.Vector3.x, value.Vector3.y, value.Vector3.z)); break;
							case VariableType.Vector4: definition.Initializer.SetStatement(string.Format("Vector4({0}, {1}, {2}, {3})", value.Vector4.x, value.Vector4.y, value.Vector4.z, value.Vector4.w)); break;
							case VariableType.Quaternion: var euler = value.Quaternion.eulerAngles; definition.Initializer.SetStatement(string.Format("Quaternion({0}, {1}, {2})", euler.x, euler.y, euler.z)); break;
							case VariableType.Rect: definition.Initializer.SetStatement(string.Format("Rect({0}, {1}, {2}, {3})", value.Rect.x, value.Rect.y, value.Rect.width, value.Rect.height)); break;
							case VariableType.Bounds: definition.Initializer.SetStatement(string.Format("Bounds({0}, {1})", value.Bounds.center, value.Bounds.extents)); break;
							case VariableType.Color: definition.Initializer.SetStatement(string.Format("#{0:X2}{1:X2}{2:X2}{3:X2}", Mathf.RoundToInt(value.Color.r * 255), Mathf.RoundToInt(value.Color.g * 255), Mathf.RoundToInt(value.Color.b * 255), Mathf.RoundToInt(value.Color.a * 255))); break;
							case VariableType.String: definition.Initializer.SetStatement("\"" + value.String + "\""); break;
						}

						SetDefinition(definition);
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

				var dropdown = new PopupField<string>();//(tags.List, tags.List, _definition.Tag, _owner, () => _definition.Tag, tag =>
				//{
				//	var definition = ValueDefinition.Create(_definition.Type, _definition.Constraint, tag, _definition.Initializer, _definition.IsTypeLocked, _definition.IsConstraintLocked);
				//	SetDefinition(definition);
				//});

				container.Add(dropdown);
			}

			ReplaceElement(ref _tagContainer, container);
		}

		#region Constraints

		private void SetupConstraint()
		{
			var container = new VisualElement();

			//if (_definition.Constraint != null)
			//	SetupConstraint(container, _definition.IsConstraintLocked, _definition.Constraint, _definition.Type, _showConstrantLabel);

			ReplaceElement(ref _constraintContainer, container);
		}

		private void SetupConstraint(VisualElement container, bool isLocked, VariableDefinition definition, VariableType type, bool showLabel)
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
					//container.Add(SetupIntConstraint(constraint as IntVariableConstraint));
					break;
				}
				case VariableType.Float:
				{
					label.tooltip = "The range of values allowed for the variable";
					//container.Add(SetupFloatConstraint(constraint as FloatVariableConstraint));
					break;
				}
				case VariableType.String:
				{
					label.tooltip = "The list of valid string values for the variable";
					//container.Add(SetupStringConstraint(constraint as StringVariableConstraint));
					break;
				}
				case VariableType.Object:
				{
					label.tooltip = "The Object type that the assigned object must be derived from or have an instance of";
					//container.Add(SetupObjectConstraint(constraint as ObjectVariableConstraint));
					break;
				}
				case VariableType.Enum:
				{
					label.tooltip = "The enum type of values added to the list";
					//container.Add(SetupEnumConstraint(constraint as EnumVariableConstraint));
					break;
				}
				case VariableType.Store:
				{
					label.tooltip = "The schema the store must use";
					//container.Add(SetupStoreConstraint(constraint as StoreVariableConstraint));
					break;
				}
				case VariableType.List:
				{
					label.tooltip = "The variable type of values added to the list";
					//container.Add(SetupListConstraint(constraint as ListVariableConstraint));
					break;
				}
			}
		}
		/*
		private VisualElement SetupIntConstraint(IntVariableConstraint constraint)
		{
			var container = new VisualElement();
			var toggle = new Toggle() { value = constraint.HasRange };
			var intContainer = new VisualElement();
			var intMin = new IntegerField() { value = constraint.Minimum, isDelayed = true };
			var intMax = new IntegerField() { value = constraint.Maximum, isDelayed = true };

			container.AddToClassList(_ussRowClass);
			container.AddToClassList(_ussConstraintClass);
			intContainer.AddToClassList(_ussRowClass);
			intContainer.AddToClassList(_ussNumberConstraintClass);
			intMin.AddToClassList(_ussConstraintInputClass);
			intMax.AddToClassList(_ussConstraintInputClass);

			intContainer.Add(new Label("Between"));
			intContainer.Add(intMin);
			intContainer.Add(new Label("and"));
			intContainer.Add(intMax);

			container.Add(toggle);
			container.Add(intContainer);

			ElementHelper.SetVisible(intContainer, constraint.HasRange);

			ElementHelper.Bind(this, toggle, _owner, () => constraint.HasRange, hasConstraint =>
			{
				constraint.HasRange = hasConstraint;
				ElementHelper.SetVisible(intContainer, hasConstraint);
				ElementHelper.SendChangeEvent(this, _definition, _definition);
			});

			ElementHelper.Bind(this, intMin, _owner, () => constraint.Minimum, minimum =>
			{
				constraint.Minimum = minimum;
				ElementHelper.SendChangeEvent(this, _definition, _definition);
			});

			ElementHelper.Bind(this, intMax, _owner, () => constraint.Maximum, maximum =>
			{
				constraint.Maximum = maximum;
				ElementHelper.SendChangeEvent(this, _definition, _definition);
			});

			return container;
		}

		private VisualElement SetupFloatConstraint(FloatVariableConstraint constraint)
		{
			var container = new VisualElement();
			var toggle = new Toggle() { value = constraint.HasRange };
			var floatContainer = new VisualElement();
			var floatMin = new FloatField() { value = constraint.Minimum, isDelayed = true };
			var floatMax = new FloatField() { value = constraint.Maximum, isDelayed = true };

			container.AddToClassList(_ussRowClass);
			container.AddToClassList(_ussConstraintClass);
			floatContainer.AddToClassList(_ussRowClass);
			floatContainer.AddToClassList(_ussNumberConstraintClass);
			floatMin.AddToClassList(_ussConstraintInputClass);
			floatMax.AddToClassList(_ussConstraintInputClass);

			floatContainer.Add(new Label("Between"));
			floatContainer.Add(floatMin);
			floatContainer.Add(new Label("and"));
			floatContainer.Add(floatMax);

			container.Add(toggle);
			container.Add(floatContainer);

			ElementHelper.SetVisible(floatContainer, constraint.HasRange);

			ElementHelper.Bind(this, toggle, _owner, () => constraint.HasRange, hasConstraint =>
			{
				constraint.HasRange = hasConstraint;
				ElementHelper.SetVisible(floatContainer, hasConstraint);
				ElementHelper.SendChangeEvent(this, _definition, _definition);
			});

			ElementHelper.Bind(this, floatMin, _owner, () => constraint.Minimum, minimum =>
			{
				constraint.Minimum = minimum;
				ElementHelper.SendChangeEvent(this, _definition, _definition);
			});

			ElementHelper.Bind(this, floatMax, _owner, () => constraint.Maximum, maximum =>
			{
				constraint.Maximum = maximum;
				ElementHelper.SendChangeEvent(this, _definition, _definition);
			});

			return container;
		}

		private VisualElement SetupStringConstraint(StringVariableConstraint constraint)
		{
			var container = new VisualElement();
			//var proxy = new ListProxy<string>(constraint.Values, (value, index) =>
			//{
			//	var field = new TextField() { value = value, isDelayed = true };
			//
			//	ElementHelper.Bind(field, field, _owner, () =>
			//	{
			//		return constraint.Values[index];
			//	},
			//	text =>
			//	{
			//		constraint.Values[index] = text;
			//		ElementHelper.SendChangeEvent(this, _definition, _definition);
			//	});
			//
			//	return field;
			//});
			//
			//var list = new ListElement(proxy, "Valid Strings", "The list of valid string values for the variable");
			//list.OnItemAdded += () => ElementHelper.SendChangeEvent(this, _definition, _definition);
			//list.OnItemRemoved += index => ElementHelper.SendChangeEvent(this, _definition, _definition);
			//list.OnItemMoved += (from, to) => ElementHelper.SendChangeEvent(this, _definition, _definition);
			//
			//container.Add(list);

			return container;
		}

		private VisualElement SetupObjectConstraint(ObjectVariableConstraint constraint)
		{
			var container = new VisualElement();
			//var picker = new TypePicker(_owner, () => constraint.Type?.AssemblyQualifiedName, type =>
			//{
			//	constraint.Type = Type.GetType(type ?? string.Empty);
			//	ElementHelper.SendChangeEvent(this, _definition, _definition);
			//});
			//
			//picker.Setup(typeof(Object), true, constraint.Type);
			//
			//container.Add(picker);

			return container;
		}

		private VisualElement SetupEnumConstraint(EnumVariableConstraint constraint)
		{
			var container = new VisualElement();
			//var picker = new TypePicker(_owner, () => constraint.Type?.AssemblyQualifiedName ?? string.Empty, type =>
			//{
			//	constraint.Type = Type.GetType(type ?? string.Empty);
			//	ElementHelper.SendChangeEvent(this, _definition, _definition);
			//});
			//
			//picker.Setup(typeof(Enum), true, constraint.Type);
			//
			//container.Add(picker);

			return container;
		}

		private VisualElement SetupStoreConstraint(StoreVariableConstraint constraint)
		{			
			var container = new VisualElement();
			//var picker = new ObjectPicker(_owner, () => constraint.Schema, schema =>
			//{
			//	constraint.Schema = schema as VariableSchema;
			//	ElementHelper.SendChangeEvent(this, _definition, _definition);
			//});
			//
			//picker.Setup(typeof(VariableSchema), constraint.Schema);
			//
			//container.Add(picker);

			return container;
		}

		private VisualElement SetupListConstraint(ListVariableConstraint constraint)
		{
			var container = new VisualElement();
			//var dropdown = new EnumDropdown<VariableType>(constraint.ItemType, _owner, () => (int)constraint.ItemType, type =>
			//{
			//	var variableType = (VariableType)type;
			//
			//	constraint.ItemType = variableType;
			//	constraint.ItemConstraint = VariableConstraint.Create(variableType);
			//
			//	ElementHelper.SendChangeEvent(this, _definition, _definition);
			//});
			//
			//container.Add(dropdown);

			if (constraint.ItemConstraint != null)
			{
				var constraintElement = new VisualElement();
				SetupConstraint(container, false, constraint.ItemConstraint, constraint.ItemType, false);
				container.Add(constraintElement);
			}

			return container;
		}
		*/
		#endregion
	}
}
