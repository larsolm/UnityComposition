using PiRhoSoft.Utilities.Editor;
using System;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace PiRhoSoft.Composition.Editor
{
	public class ValueDefinitionControl : VisualElement
	{
		public VariableDefinition Value { get; private set; }
		public VariableInitializerType Initializer { get; private set; }
		public TagList Tags { get; private set; }

		private VisualElement _typeContainer;
		private VisualElement _constraintContainer;
		private VisualElement _initializerContainer;
		private VisualElement _tagContainer;

		private readonly bool _showConstraintLabel;

		public ValueDefinitionControl(VariableDefinition value, VariableInitializerType initializer, TagList tags, bool showConstraintLabel)
		{
			Value = Value;
			Initializer = initializer;
			Tags = tags;

			_showConstraintLabel = showConstraintLabel;

			Refresh();
		}

		public void SetValueWithoutNotify(VariableDefinition value)
		{
			Value = value;
			Refresh();
		}

		public void SetValue(VariableDefinition value)
		{
			var previous = Value;
			SetValueWithoutNotify(value);
			this.SendChangeEvent(previous, Value);
		}

		#region Flags

		//private bool HasInitializer
		//{
		//	get
		//	{
		//		if (Initializer == VariableInitializerType.None)
		//			return false;
		//
		//		switch (Value.Type)
		//		{
		//			case VariableType.Bool:
		//			case VariableType.Float:
		//			case VariableType.Int:
		//			case VariableType.Int2:
		//			case VariableType.Int3:
		//			case VariableType.IntRect:
		//			case VariableType.IntBounds:
		//			case VariableType.Vector2:
		//			case VariableType.Vector3:
		//			case VariableType.Vector4:
		//			case VariableType.Quaternion:
		//			case VariableType.Rect:
		//			case VariableType.Bounds:
		//			case VariableType.Color:
		//			case VariableType.String: return Value.Initializer != null;
		//			default: return false;
		//		}
		//	}
		//}

		#endregion

		private void Refresh()
		{
			Reset();

			SetupType();
			SetupConstraint();
			SetupInitializer();
			SetupTag();
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

		private void SetupType()
		{
			var container = new VisualElement() { tooltip = "The type of variable this defines" };

			//if (Value.IsTypeLocked)
			//	container.SetEnabled(false);

			//var dropdown = new EnumField(Value.Type);
			//dropdown.RegisterValueChangedCallback(evt =>
			//{
			//	var type = (VariableType)evt.newValue;
			//	var constraint = VariableConstraint.Create(type);
			//	var value = VariableDefinition.Create(type, constraint, Value.Tag, Value.Initializer, Value.IsTypeLocked, Value.IsConstraintLocked);
			//
			//	SetValue(value);
			//});

			//container.Add(dropdown);

			ReplaceElement(ref _typeContainer, container);
		}

		private void SetupInitializer()
		{
			var container = new VisualElement();

			//if (HasInitializer)
			//{
			//	if (Initializer == VariableInitializerType.Expression)
			//	{
			//		var expression = new ExpressionField("Initializer", Value.Initializer) { tooltip = "The Expression to execute and assign when initializing, resetting, or updating the variable" };
			//		container.Add(expression);
			//	}
			//	else if (Initializer == VariableInitializerType.DefaultValue)
			//	{
			//		var defaultValue = Value.Initializer.Execute(null, null); // context isn't necessary since the object that would be the context is currently drawing
			//		if (defaultValue.IsEmpty) // If the initializer hasn't been set, use the default value.
			//			defaultValue = VariableHandler.CreateDefault(Value.Type, Value.Constraint);
			//
			//		var field = new VariableValueField("Default", defaultValue, Value);
			//		field.RegisterValueChangedCallback(evt =>
			//		{
			//			switch (Value.Type)
			//			{
			//				case VariableType.Bool: Value.Initializer.SetStatement(evt.newValue.Bool ? "true" : "false"); break;
			//				case VariableType.Float: Value.Initializer.SetStatement(evt.newValue.Float.ToString()); break;
			//				case VariableType.Int: Value.Initializer.SetStatement(evt.newValue.Int.ToString()); break;
			//				case VariableType.Int2: Value.Initializer.SetStatement(string.Format("Vector2Int({0}, {1})", evt.newValue.Int2.x, evt.newValue.Int2.y)); break;
			//				case VariableType.Int3: Value.Initializer.SetStatement(string.Format("Vector3Int({0}, {1}, {2})", evt.newValue.Int3.x, evt.newValue.Int3.y, evt.newValue.Int3.z)); break;
			//				case VariableType.IntRect: Value.Initializer.SetStatement(string.Format("RectInt({0}, {1}, {2}, {3})", evt.newValue.IntRect.x, evt.newValue.IntRect.y, evt.newValue.IntRect.width, evt.newValue.IntRect.height)); break;
			//				case VariableType.IntBounds: Value.Initializer.SetStatement(string.Format("BoundsInt({0}, {1}, {2}, {3}, {4}, {5})", evt.newValue.IntBounds.x, evt.newValue.IntBounds.y, evt.newValue.IntBounds.z, evt.newValue.IntBounds.size.x, evt.newValue.IntBounds.size.y, evt.newValue.IntBounds.size.z)); break;
			//				case VariableType.Vector2: Value.Initializer.SetStatement(string.Format("Vector2({0}, {1})", evt.newValue.Vector2.x, evt.newValue.Vector2.y)); break;
			//				case VariableType.Vector3: Value.Initializer.SetStatement(string.Format("Vector3({0}, {1}, {2})", evt.newValue.Vector3.x, evt.newValue.Vector3.y, evt.newValue.Vector3.z)); break;
			//				case VariableType.Vector4: Value.Initializer.SetStatement(string.Format("Vector4({0}, {1}, {2}, {3})", evt.newValue.Vector4.x, evt.newValue.Vector4.y, evt.newValue.Vector4.z, evt.newValue.Vector4.w)); break;
			//				case VariableType.Quaternion: var euler = evt.newValue.Quaternion.eulerAngles; Value.Initializer.SetStatement(string.Format("Quaternion({0}, {1}, {2})", euler.x, euler.y, euler.z)); break;
			//				case VariableType.Rect: Value.Initializer.SetStatement(string.Format("Rect({0}, {1}, {2}, {3})", evt.newValue.Rect.x, evt.newValue.Rect.y, evt.newValue.Rect.width, evt.newValue.Rect.height)); break;
			//				case VariableType.Bounds: Value.Initializer.SetStatement(string.Format("Bounds({0}, {1})", evt.newValue.Bounds.center, evt.newValue.Bounds.extents)); break;
			//				case VariableType.Color: Value.Initializer.SetStatement(string.Format("#{0:X2}{1:X2}{2:X2}{3:X2}", Mathf.RoundToInt(evt.newValue.Color.r * 255), Mathf.RoundToInt(evt.newValue.Color.g * 255), Mathf.RoundToInt(evt.newValue.Color.b * 255), Mathf.RoundToInt(evt.newValue.Color.a * 255))); break;
			//				case VariableType.String: Value.Initializer.SetStatement("\"" + evt.newValue.String + "\""); break;
			//			}
			//		});
			//	}
			//}

			ReplaceElement(ref _initializerContainer, container);
		}

		private void SetupTag()
		{
			var container = new VisualElement();

			if (Tags != null && Tags.Count > 0)
			{
				//var dropdown = new PopupField<string>("Tag", Tags.List, Value.Tag) { tooltip = "An identifier that can be used to reset or persist this variable" };
				//dropdown.RegisterValueChangedCallback(evt =>
				//{
				//	var value  = VariableDefinition.Create(Value.Type, Value.Constraint, evt.newValue, Value.Initializer, Value.IsTypeLocked, Value.IsConstraintLocked);
				//	SetValue(value);
				//});

				//container.Add(dropdown);
			}

			ReplaceElement(ref _tagContainer, container);
		}

		#region Constraints

		private void SetupConstraint()
		{
			var container = new VisualElement();

			//if (Value.Constraint != null)
			//	SetupConstraint(container, Value.IsConstraintLocked, Value.Constraint, Value.Type, _showConstraintLabel);

			ReplaceElement(ref _constraintContainer, container);
		}

		private void SetupConstraint(VisualElement container, bool isLocked, VariableConstraint constraint, VariableType type, bool showLabel)
		{
			var label = new Label("Constraint");

			if (isLocked)
				container.SetEnabled(false);

			if (showLabel)
				container.Add(label);

			//switch (type)
			//{
			//	case VariableType.Int:
			//	{
			//		label.tooltip = "The range of values allowed for the variable";
			//		container.Add(SetupIntConstraint(constraint as IntVariableConstraint));
			//		break;
			//	}
			//	case VariableType.Float:
			//	{
			//		label.tooltip = "The range of values allowed for the variable";
			//		container.Add(SetupFloatConstraint(constraint as FloatVariableConstraint));
			//		break;
			//	}
			//	case VariableType.String:
			//	{
			//		label.tooltip = "The list of valid string values for the variable";
			//		container.Add(SetupStringConstraint(constraint as StringVariableConstraint));
			//		break;
			//	}
			//	case VariableType.Object:
			//	{
			//		label.tooltip = "The Object type that the assigned object must be derived from or have an instance of";
			//		container.Add(SetupObjectConstraint(constraint as ObjectVariableConstraint));
			//		break;
			//	}
			//	case VariableType.Enum:
			//	{
			//		label.tooltip = "The enum type of values added to the list";
			//		container.Add(SetupEnumConstraint(constraint as EnumVariableConstraint));
			//		break;
			//	}
			//	case VariableType.Store:
			//	{
			//		label.tooltip = "The schema the store must use";
			//		container.Add(SetupStoreConstraint(constraint as StoreVariableConstraint));
			//		break;
			//	}
			//	case VariableType.List:
			//	{
			//		label.tooltip = "The variable type of values added to the list";
			//		container.Add(SetupListConstraint(constraint as ListVariableConstraint));
			//		break;
			//	}
			//}
		}

		//private VisualElement SetupIntConstraint(IntVariableConstraint constraint)
		//{
		//	var container = new VisualElement();
		//	var toggle = new Toggle() { value = constraint.HasRange };
		//	var intContainer = new VisualElement();
		//	var intMin = new IntegerField() { value = constraint.Minimum, isDelayed = true };
		//	var intMax = new IntegerField() { value = constraint.Maximum, isDelayed = true };
		//
		//	intContainer.Add(new Label("Between"));
		//	intContainer.Add(intMin);
		//	intContainer.Add(new Label("and"));
		//	intContainer.Add(intMax);
		//
		//	container.Add(toggle);
		//	container.Add(intContainer);
		//
		//	intContainer.SetDisplayed(constraint.HasRange);
		//
		//	toggle.RegisterValueChangedCallback(evt =>
		//	{
		//		constraint.HasRange = evt.newValue;
		//		intContainer.SetDisplayed(constraint.HasRange);
		//	});
		//
		//	intMin.RegisterValueChangedCallback(evt =>
		//	{
		//		constraint.Minimum = evt.newValue;
		//	});
		//
		//	intMax.RegisterValueChangedCallback(evt =>
		//	{
		//		constraint.Maximum = evt.newValue;
		//	});
		//
		//	return container;
		//}
		//
		//private VisualElement SetupFloatConstraint(FloatVariableConstraint constraint)
		//{
		//	var container = new VisualElement();
		//	var toggle = new Toggle() { value = constraint.HasRange };
		//	var floatContainer = new VisualElement();
		//	var floatMin = new FloatField() { value = constraint.Minimum, isDelayed = true };
		//	var floatMax = new FloatField() { value = constraint.Maximum, isDelayed = true };
		//
		//	floatContainer.Add(new Label("Between"));
		//	floatContainer.Add(floatMin);
		//	floatContainer.Add(new Label("and"));
		//	floatContainer.Add(floatMax);
		//
		//	container.Add(toggle);
		//	container.Add(floatContainer);
		//
		//	floatContainer.SetDisplayed(constraint.HasRange);
		//
		//	toggle.RegisterValueChangedCallback(evt =>
		//	{
		//		constraint.HasRange = evt.newValue;
		//		floatContainer.SetDisplayed(constraint.HasRange);
		//	});
		//
		//	floatMin.RegisterValueChangedCallback(evt =>
		//	{
		//		constraint.Minimum = evt.newValue;
		//	});
		//
		//	floatMax.RegisterValueChangedCallback(evt =>
		//	{
		//		constraint.Maximum = evt.newValue;
		//	});
		//
		//	return container;
		//}
		//
		//private VisualElement SetupStringConstraint(StringVariableConstraint constraint)
		//{
		//	var container = new VisualElement();
		//	//var proxy = new ListProxy<string>(constraint.Values, (value, index) =>
		//	//{
		//	//	var field = new TextField() { value = value, isDelayed = true };
		//	//
		//	//	ElementHelper.Bind(field, field, _owner, () =>
		//	//	{
		//	//		return constraint.Values[index];
		//	//	},
		//	//	text =>
		//	//	{
		//	//		constraint.Values[index] = text;
		//	//		ElementHelper.SendChangeEvent(this, _definition, _definition);
		//	//	});
		//	//
		//	//	return field;
		//	//});
		//	//
		//	//var list = new ListElement(proxy, "Valid Strings", "The list of valid string values for the variable");
		//	//list.OnItemAdded += () => ElementHelper.SendChangeEvent(this, _definition, _definition);
		//	//list.OnItemRemoved += index => ElementHelper.SendChangeEvent(this, _definition, _definition);
		//	//list.OnItemMoved += (from, to) => ElementHelper.SendChangeEvent(this, _definition, _definition);
		//	//
		//	//container.Add(list);
		//
		//	return container;
		//}
		//
		//private VisualElement SetupObjectConstraint(ObjectVariableConstraint constraint)
		//{
		//	var picker = new TypePickerControl(constraint.Type?.AssemblyQualifiedName, typeof(Object), true);
		//	picker.RegisterCallback<ChangeEvent<string>>(evt =>
		//	{
		//		constraint.Type = Type.GetType(evt.newValue ?? string.Empty);
		//	});
		//	
		//	return picker;
		//}
		//
		//private VisualElement SetupEnumConstraint(EnumVariableConstraint constraint)
		//{
		//	var picker = new TypePickerControl(constraint.Type?.AssemblyQualifiedName, typeof(Enum), true);
		//	picker.RegisterCallback<ChangeEvent<string>>(evt =>
		//	{
		//		constraint.Type = Type.GetType(evt.newValue ?? string.Empty);
		//	});
		//
		//	return picker;
		//}
		//
		//private VisualElement SetupStoreConstraint(StoreVariableConstraint constraint)
		//{
		//	var picker = new ObjectPickerControl(constraint.Schema, typeof(VariableSchema));
		//	picker.RegisterCallback<ChangeEvent<Object>>(evt =>
		//	{
		//		constraint.Schema = evt.newValue as VariableSchema;
		//	});
		//
		//	return picker;
		//}
		//
		//private VisualElement SetupListConstraint(ListVariableConstraint constraint)
		//{
		//	var container = new VisualElement();
		//	var dropdown = new EnumField(constraint.ItemType);
		//	dropdown.RegisterValueChangedCallback(evt =>
		//	{
		//		var type = (VariableType)evt.newValue;
		//	
		//		constraint.ItemType = type;
		//		constraint.ItemConstraint = VariableConstraint.Create(type);
		//	});
		//	
		//	container.Add(dropdown);
		//
		//	if (constraint.ItemConstraint != null)
		//	{
		//		var constraintElement = new VisualElement();
		//		SetupConstraint(container, false, constraint.ItemConstraint, constraint.ItemType, false);
		//		container.Add(constraintElement);
		//	}
		//
		//	return container;
		//}

		#endregion
	}
}
