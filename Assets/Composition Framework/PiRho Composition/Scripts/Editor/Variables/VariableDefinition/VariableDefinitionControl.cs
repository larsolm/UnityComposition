using PiRhoSoft.Utilities.Editor;
using System;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace PiRhoSoft.Composition.Editor
{
	public class VariableDefinitionControl : VisualElement
	{
		public VariableDefinition Value { get; private set; }

		private VisualElement _typeContainer;
		private VisualElement _constraintContainer;

		public VariableDefinitionControl(VariableDefinition value)
		{
			Value = value;

			_typeContainer = new VisualElement { tooltip = "The type of variable this definition defines" };
			_constraintContainer = new VisualElement();

			Refresh();
		}

		public void SetValueWithoutNotify(VariableDefinition value)
		{
			Value = value;
			Refresh();
		}

		private void Refresh()
		{
			_typeContainer.Clear();
			_constraintContainer.Clear();

			SetupType();
			SetupConstraint(_constraintContainer, Value.Constraint);
		}

		private void SetupType()
		{
			var dropdown = new EnumField(Value.Type);
			dropdown.RegisterValueChangedCallback(evt =>
			{
				Value.Type = (VariableType)evt.newValue;
			});

			_typeContainer.Add(dropdown);
		}

		private void SendChangeEvent()
		{
			this.SendChangeEvent(Value, Value);
		}

		#region Constraints

		private void SetupConstraint(VisualElement container, VariableConstraint constraint)
		{
			if (constraint != null)
			{
				var constraintContainer = CreateConstraint(constraint, out var tooltip);
				var label = new Label("Constraint") { tooltip = tooltip };

				container.Add(label);
				container.Add(constraintContainer);
			}
		}

		private VisualElement CreateConstraint(VariableConstraint constraint, out string tooltip)
		{
			switch (constraint)
			{
				case IntConstraint intConstraint:
				{
					tooltip = "The range of values allowed for the variable";
					return CreateIntConstraint(intConstraint);
				}
				case FloatConstraint floatConstraint:
				{
					tooltip = "The range of values allowed for the variable";
					return CreateFloatConstraint(floatConstraint);
				}
				case StringConstraint stringConstraint:
				{
					tooltip = "The list of valid string values for the variable";
					return CreateStringConstraint(stringConstraint);
				}
				case ObjectConstraint objectConstraint:
				{
					tooltip = "The Object type that the assigned object must be derived from or have an instance of";
					return CreateObjectConstraint(objectConstraint);
				}
				case EnumConstraint enumConstraint:
				{
					tooltip = "The enum type of values added to the list";
					return CreateEnumConstraint(enumConstraint);
				}
				case DictionaryConstraint dictionaryConstraint:
				{
					tooltip = "The schema the store must use";
					return CreateDictionaryConstraint(dictionaryConstraint);
				}
				case ListConstraint listConstraint:
				{
					tooltip = "The variable type of values added to the list";
					return CreateListConstraint(listConstraint);
				}
				default:
				{
					tooltip = null;
					return null;
				}
			}
		}

		private VisualElement CreateIntConstraint(IntConstraint constraint)
		{
			var container = new VisualElement();
			var hasMin = new Toggle("Minimum:") { value = constraint.Minimum.HasValue };
			var hasMax = new Toggle("Maximum:") { value = constraint.Maximum.HasValue };

			var min = new IntegerField() { value = constraint.Minimum.GetValueOrDefault(0), isDelayed = true };
			var max = new IntegerField() { value = constraint.Maximum.GetValueOrDefault(100), isDelayed = true };
		
			container.Add(hasMin);
			container.Add(min);
			container.Add(hasMax);
			container.Add(max);
		
			hasMin.SetDisplayed(constraint.Minimum.HasValue);
			hasMax.SetDisplayed(constraint.Maximum.HasValue);

			hasMin.RegisterValueChangedCallback(evt =>
			{
				if (evt.newValue)
					constraint.Minimum = 0;
				else
					constraint.Minimum = null;

				min.SetDisplayed(constraint.Minimum.HasValue);
				SendChangeEvent();
			});

			hasMax.RegisterValueChangedCallback(evt =>
			{
				if (evt.newValue)
					constraint.Maximum = 0;
				else
					constraint.Maximum = null;

				min.SetDisplayed(constraint.Maximum.HasValue);
				SendChangeEvent();
			});

			min.RegisterValueChangedCallback(evt =>
			{
				constraint.Minimum = evt.newValue;
				SendChangeEvent();
			});
		
			max.RegisterValueChangedCallback(evt =>
			{
				constraint.Maximum = evt.newValue;
				SendChangeEvent();
			});
		
			return container;
		}
		
		private VisualElement CreateFloatConstraint(FloatConstraint constraint)
		{
			var container = new VisualElement();
			var hasMin = new Toggle("Minimum:") { value = constraint.Minimum.HasValue };
			var hasMax = new Toggle("Maximum:") { value = constraint.Maximum.HasValue };

			var min = new FloatField() { value = constraint.Minimum.GetValueOrDefault(0.0f), isDelayed = true };
			var max = new FloatField() { value = constraint.Maximum.GetValueOrDefault(1.0f), isDelayed = true };

			container.Add(hasMin);
			container.Add(min);
			container.Add(hasMax);
			container.Add(max);

			hasMin.SetDisplayed(constraint.Minimum.HasValue);
			hasMax.SetDisplayed(constraint.Maximum.HasValue);

			hasMin.RegisterValueChangedCallback(evt =>
			{
				if (evt.newValue)
					constraint.Minimum = 0;
				else
					constraint.Minimum = null;

				min.SetDisplayed(constraint.Minimum.HasValue);
				SendChangeEvent();
			});

			hasMax.RegisterValueChangedCallback(evt =>
			{
				if (evt.newValue)
					constraint.Maximum = 0;
				else
					constraint.Maximum = null;

				min.SetDisplayed(constraint.Maximum.HasValue);
				SendChangeEvent();
			});

			min.RegisterValueChangedCallback(evt =>
			{
				constraint.Minimum = evt.newValue;
				SendChangeEvent();
			});

			max.RegisterValueChangedCallback(evt =>
			{
				constraint.Maximum = evt.newValue;
				SendChangeEvent();
			});

			return container;
		}
		
		private VisualElement CreateStringConstraint(StringConstraint constraint)
		{
			var proxy = new ListProxy<string>(constraint.Values, index =>
			{
				var field = new TextField() { value = constraint.Values[index], isDelayed = true };
				field.RegisterValueChangedCallback(evt => 
				{
					constraint.Values[index] = evt.newValue;
					SendChangeEvent();
				});

				return field;
			})
			{
				Label = "Valid Strings",
				Tooltip = "A list of valid strings that this variable can contain"
			};

			return new ListControl(proxy);
		}
		
		private VisualElement CreateObjectConstraint(ObjectConstraint constraint)
		{
			var picker = new TypePickerControl(constraint.ObjectType.AssemblyQualifiedName, typeof(Object), true);
			picker.RegisterCallback<ChangeEvent<string>>(evt =>
			{
				constraint.ObjectType = Type.GetType(evt.newValue ?? string.Empty);
				SendChangeEvent();
			});
			
			return picker;
		}
		
		private VisualElement CreateEnumConstraint(EnumConstraint constraint)
		{
			var picker = new TypePickerControl(constraint.EnumType.AssemblyQualifiedName, typeof(Enum), true);
			picker.RegisterCallback<ChangeEvent<string>>(evt =>
			{
				constraint.EnumType = Type.GetType(evt.newValue ?? string.Empty);
				SendChangeEvent();
			});
		
			return picker;
		}
		
		private VisualElement CreateDictionaryConstraint(DictionaryConstraint constraint)
		{
			var picker = new ObjectPickerControl(constraint.Schema, typeof(VariableSchema));
			picker.RegisterCallback<ChangeEvent<Object>>(evt =>
			{
				constraint.Schema = evt.newValue as VariableSchema;
				SendChangeEvent();
			});
		
			return picker;
		}
		
		private VisualElement CreateListConstraint(ListConstraint constraint)
		{
			var container = new VisualElement();
			var dropdown = new EnumField(constraint.ItemType);
			dropdown.RegisterValueChangedCallback(evt =>
			{
				constraint.ItemType = (VariableType)evt.newValue;
				SendChangeEvent();
			});
			
			container.Add(dropdown);
		
			if (constraint.ItemConstraint != null)
			{
				var constraintElement = new VisualElement();
				SetupConstraint(container, constraint.ItemConstraint);
				container.Add(constraintElement);
			}
		
			return container;
		}

		#endregion
	}
}
