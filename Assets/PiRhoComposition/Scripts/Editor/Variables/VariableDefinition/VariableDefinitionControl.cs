using PiRhoSoft.Utilities.Editor;
using System;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace PiRhoSoft.Composition.Editor
{
	public class VariableDefinitionControl : VisualElement
	{
		#region Style Strings

		public const string Stylesheet = "Variables/VariableDefinition/VariableDefinitionStyle.uss";
		public const string UssClassName = "pirho-variable-definition";
		public const string NameUssClassName = UssClassName + "__name";
		public const string NoNameUssClassName = NameUssClassName + "--none";
		public const string TypeUssClassName = UssClassName + "__type";
		public const string ConstraintUssClassName = UssClassName + "__constraint";
		public const string NumberConstraintUssClassName = ConstraintUssClassName + "__number";
		public const string NumberConstraintContainerUssClassName = NumberConstraintUssClassName + "__container";
		public const string NumberConstraintContainerMinUssClassName = NumberConstraintContainerUssClassName + "--min";
		public const string NumberConstraintContainerMaxUssClassName = NumberConstraintContainerUssClassName + "--max";
		public const string NumberConstraintLabelUssClassName = NumberConstraintUssClassName + "__label";
		public const string NumberConstraintToggleUssClassName = NumberConstraintUssClassName + "__toggle";
		public const string NumberConstraintMinToggleUssClassName = NumberConstraintToggleUssClassName + "--min";
		public const string NumberConstraintMaxToggleUssClassName = NumberConstraintToggleUssClassName + "--max";
		public const string NumberConstraintFieldUssClassName = NumberConstraintUssClassName + "__field";
		public const string NumberConstraintHasValueUssClassName = NumberConstraintFieldUssClassName + "--has-value";
		public const string NumberConstraintMinUssClassName = NumberConstraintFieldUssClassName + "--min";
		public const string NumberConstraintMaxUssClassName = NumberConstraintFieldUssClassName + "--max";
		public const string StringConstraintUssClassName = ConstraintUssClassName + "__string";
		public const string ObjectConstraintUssClassName = ConstraintUssClassName + "__object";
		public const string EnumConstraintUssClassName = ConstraintUssClassName + "__enum";
		public const string SchemaConstraintUssClassName = ConstraintUssClassName + "__schema";
		public const string ListConstraintUssClassName = ConstraintUssClassName + "__list";
		public const string ListItemConstraintUssClassName = ListConstraintUssClassName + "__item";

		#endregion

		public VariableDefinition Value { get; private set; }

		private readonly Object _owner;

		private Label _label;
		private EnumField _typeDropdown;
		private VisualElement _constraintContainer;

		public VariableDefinitionControl(VariableDefinition value, Object owner)
		{
			Value = value;

			_owner = owner;

			_label = new Label();
			_label.AddToClassList(NameUssClassName);
			SetLabel(Value.Name);
			Add(_label);

			CreateType();
			CreateConstraint();
			Refresh();

			this.AddStyleSheet(Configuration.EditorPath, Stylesheet);
			AddToClassList(UssClassName);
		}

		public void SetLabel(string label)
		{
			_label.text = label;
			_label.EnableInClassList(NoNameUssClassName, string.IsNullOrEmpty(label));
		}

		public void Refresh()
		{
			RefreshType();
			RefreshConstraint(_constraintContainer, Value.Constraint);
		}

		private void CreateType()
		{
			_typeDropdown = new EnumField(Value.Type) { tooltip = "The type of variable this definition defines" };
			_typeDropdown.AddToClassList(TypeUssClassName);
			_typeDropdown.RegisterValueChangedCallback(evt =>
			{
				Value.Type = (VariableType)evt.newValue;

				_constraintContainer.Clear();
				RefreshConstraint(_constraintContainer, Value.Constraint);
				SendChangeEvent();
			});

			Add(_typeDropdown);
		}

		private void RefreshType()
		{
			_typeDropdown.SetValueWithoutNotify(Value.Type);
		}

		private void SendChangeEvent()
		{
			this.SendChangeEvent(Value, Value);
		}

		#region Constraints

		private void CreateConstraint()
		{
			_constraintContainer = new VisualElement();
			_constraintContainer.AddToClassList(ConstraintUssClassName);

			CreateConstraint(_constraintContainer, Value.Constraint);

			Add(_constraintContainer);
		}

		private void CreateConstraint(VisualElement container, VariableConstraint constraint)
		{
			container.Clear();

			if (constraint != null)
			{
				switch (constraint)
				{
					case IntConstraint intConstraint: container.Add(CreateIntConstraint(intConstraint)); break;
					case FloatConstraint floatConstraint: container.Add(CreateFloatConstraint(floatConstraint)); break;
					case StringConstraint stringConstraint: container.Add(CreateStringConstraint(stringConstraint)); break;
					case ObjectConstraint objectConstraint: container.Add(CreateObjectConstraint(objectConstraint)); break;
					case EnumConstraint enumConstraint: container.Add(CreateEnumConstraint(enumConstraint)); break;
					case DictionaryConstraint dictionaryConstraint: container.Add(CreateDictionaryConstraint(dictionaryConstraint)); break;
					case ListConstraint listConstraint: container.Add(CreateListConstraint(listConstraint)); break;
				}
			}
		}

		private void RefreshConstraint(VisualElement container, VariableConstraint constraint)
		{
			if (constraint != null)
			{
				if (container.childCount == 0)
					CreateConstraint(container, constraint);

				switch (constraint)
				{
					case IntConstraint intConstraint: RefreshIntConstraint(container, intConstraint); break;
					case FloatConstraint floatConstraint: RefreshFloatConstraint(container, floatConstraint); break;
					case StringConstraint stringConstraint: RefreshStringConstraint(container, stringConstraint); break;
					case ObjectConstraint objectConstraint: RefreshObjectConstraint(container, objectConstraint); break;
					case EnumConstraint enumConstraint: RefreshEnumConstraint(container, enumConstraint); break;
					case DictionaryConstraint dictionaryConstraint: RefreshDictionaryConstraint(container, dictionaryConstraint); break;
					case ListConstraint listConstraint: RefreshListConstraint(container, listConstraint); break;
				}
			}
		}

		private VisualElement CreateIntConstraint(IntConstraint constraint)
		{
			var container = new VisualElement();
			var minContainer = new VisualElement();
			var maxContainer = new VisualElement();
			var minLabel = new TextElement { text = "Minimum:", tooltip = "Whether to constrain this int to a minimum value" };
			var maxLabel = new TextElement { text = "Maximum:", tooltip = "Whether to constrain this int to a maximum value" };
			var minToggle = new Toggle();
			var maxToggle = new Toggle();
			var min = new IntegerField();
			var max = new IntegerField();

			container.AddToClassList(NumberConstraintUssClassName);
			minContainer.AddToClassList(NumberConstraintContainerUssClassName);
			minContainer.AddToClassList(NumberConstraintContainerMinUssClassName);
			maxContainer.AddToClassList(NumberConstraintContainerUssClassName);
			maxContainer.AddToClassList(NumberConstraintContainerMaxUssClassName);
			minLabel.AddToClassList(NumberConstraintLabelUssClassName);
			maxLabel.AddToClassList(NumberConstraintLabelUssClassName);
			minToggle.AddToClassList(NumberConstraintToggleUssClassName);
			minToggle.AddToClassList(NumberConstraintMinToggleUssClassName);
			maxToggle.AddToClassList(NumberConstraintToggleUssClassName);
			maxToggle.AddToClassList(NumberConstraintMaxToggleUssClassName);
			min.AddToClassList(NumberConstraintFieldUssClassName);
			min.AddToClassList(NumberConstraintMinUssClassName);
			max.AddToClassList(NumberConstraintFieldUssClassName);
			max.AddToClassList(NumberConstraintMaxUssClassName);

			minContainer.Add(minLabel);
			minContainer.Add(minToggle);
			minContainer.Add(min);
			maxContainer.Add(maxLabel);
			maxContainer.Add(maxToggle);
			maxContainer.Add(max);

			container.Add(minContainer);
			container.Add(maxContainer);

			minToggle.RegisterValueChangedCallback(evt =>
			{
				if (evt.newValue)
					constraint.Minimum = IntConstraint.DefaultMinimum;
				else
					constraint.Minimum = null;

				RefreshIntConstraint(container, constraint);
				SendChangeEvent();
			});

			maxToggle.RegisterValueChangedCallback(evt =>
			{
				if (evt.newValue)
					constraint.Maximum = IntConstraint.DefaultMaximum;
				else
					constraint.Maximum = null;

				RefreshIntConstraint(container, constraint);
				SendChangeEvent();
			});

			min.RegisterValueChangedCallback(evt =>
			{
				constraint.Minimum = evt.newValue;
				RefreshIntConstraint(container, constraint);
				SendChangeEvent();
			});

			max.RegisterValueChangedCallback(evt =>
			{
				constraint.Maximum = evt.newValue;
				RefreshIntConstraint(container, constraint);
				SendChangeEvent();
			});

			return container;
		}

		private void RefreshIntConstraint(VisualElement container, IntConstraint constraint)
		{
			var minToggle = container.Q<Toggle>(className: NumberConstraintMinToggleUssClassName);
			var maxToggle = container.Q<Toggle>(className: NumberConstraintMaxToggleUssClassName);
			var min = container.Q<IntegerField>(className: NumberConstraintMinUssClassName);
			var max = container.Q<IntegerField>(className: NumberConstraintMaxUssClassName);

			minToggle.SetValueWithoutNotify(constraint.Minimum.HasValue);
			maxToggle.SetValueWithoutNotify(constraint.Maximum.HasValue);
			min.SetValueWithoutNotify(constraint.Minimum ?? 0);
			max.SetValueWithoutNotify(constraint.Maximum ?? 10);

			min.EnableInClassList(NumberConstraintHasValueUssClassName, constraint.Minimum.HasValue);
			max.EnableInClassList(NumberConstraintHasValueUssClassName, constraint.Maximum.HasValue);
		}

		private VisualElement CreateFloatConstraint(FloatConstraint constraint)
		{
			var container = new VisualElement();
			var minContainer = new VisualElement();
			var maxContainer = new VisualElement();
			var minLabel = new TextElement { text = "Minimum:", tooltip = "Whether to constrain this int to a minimum value" };
			var maxLabel = new TextElement { text = "Maximum:", tooltip = "Whether to constrain this int to a maximum value" };
			var minToggle = new Toggle();
			var maxToggle = new Toggle();
			var min = new FloatField();
			var max = new FloatField();

			container.AddToClassList(NumberConstraintUssClassName);
			minContainer.AddToClassList(NumberConstraintContainerUssClassName);
			minContainer.AddToClassList(NumberConstraintContainerMinUssClassName);
			maxContainer.AddToClassList(NumberConstraintContainerUssClassName);
			maxContainer.AddToClassList(NumberConstraintContainerMaxUssClassName);
			minLabel.AddToClassList(NumberConstraintLabelUssClassName);
			maxLabel.AddToClassList(NumberConstraintLabelUssClassName);
			minToggle.AddToClassList(NumberConstraintToggleUssClassName);
			minToggle.AddToClassList(NumberConstraintMinToggleUssClassName);
			maxToggle.AddToClassList(NumberConstraintToggleUssClassName);
			maxToggle.AddToClassList(NumberConstraintMaxToggleUssClassName);
			min.AddToClassList(NumberConstraintFieldUssClassName);
			min.AddToClassList(NumberConstraintMinUssClassName);
			max.AddToClassList(NumberConstraintFieldUssClassName);
			max.AddToClassList(NumberConstraintMaxUssClassName);

			minContainer.Add(minLabel);
			minContainer.Add(minToggle);
			minContainer.Add(min);
			maxContainer.Add(maxLabel);
			maxContainer.Add(maxToggle);
			maxContainer.Add(max);

			container.Add(minContainer);
			container.Add(maxContainer);

			minToggle.RegisterValueChangedCallback(evt =>
			{
				if (evt.newValue)
					constraint.Minimum = FloatConstraint.DefaultMinimum;
				else
					constraint.Minimum = null;

				SendChangeEvent();
			});

			maxToggle.RegisterValueChangedCallback(evt =>
			{
				if (evt.newValue)
					constraint.Maximum = FloatConstraint.DefaultMaximum;
				else
					constraint.Maximum = null;

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

		private void RefreshFloatConstraint(VisualElement container, FloatConstraint constraint)
		{
			var minToggle = container.Q<Toggle>(className: NumberConstraintMinToggleUssClassName);
			var maxToggle = container.Q<Toggle>(className: NumberConstraintMaxToggleUssClassName);
			var min = container.Q<FloatField>(className: NumberConstraintMinUssClassName);
			var max = container.Q<FloatField>(className: NumberConstraintMaxUssClassName);

			minToggle.SetValueWithoutNotify(constraint.Minimum.HasValue);
			maxToggle.SetValueWithoutNotify(constraint.Maximum.HasValue);
			min.SetValueWithoutNotify(constraint.Minimum ?? 0.0f);
			max.SetValueWithoutNotify(constraint.Maximum ?? 10.0f);

			min.EnableInClassList(NumberConstraintHasValueUssClassName, constraint.Minimum.HasValue);
			max.EnableInClassList(NumberConstraintHasValueUssClassName, constraint.Maximum.HasValue);
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

			var list = new ListControl(proxy);
			list.AddToClassList(StringConstraintUssClassName);

			return list;
		}

		private void RefreshStringConstraint(VisualElement container, StringConstraint constraint)
		{
			var list = container.Q<ListControl>(className: StringConstraintUssClassName);
			var proxy = list.Proxy as ListProxy<string>;
			proxy.Items = constraint.Values;
			list.Refresh();
		}

		private VisualElement CreateObjectConstraint(ObjectConstraint constraint)
		{
			var picker = new TypePickerControl(constraint.ObjectType.AssemblyQualifiedName, typeof(Object), true);
			picker.AddToClassList(ObjectConstraintUssClassName);
			picker.RegisterCallback<ChangeEvent<string>>(evt =>
			{
				constraint.ObjectType = Type.GetType(evt.newValue ?? string.Empty);
				SendChangeEvent();
			});

			return picker;
		}

		private void RefreshObjectConstraint(VisualElement container, ObjectConstraint constraint)
		{
			var picker = container.Q<TypePickerControl>(className: ObjectConstraintUssClassName);
			picker.SetValueWithoutNotify(constraint.ObjectType.AssemblyQualifiedName);
		}

		private VisualElement CreateEnumConstraint(EnumConstraint constraint)
		{
			var picker = new TypePickerControl(constraint.EnumType.AssemblyQualifiedName, typeof(Enum), true);
			picker.AddToClassList(EnumConstraintUssClassName);
			picker.RegisterCallback<ChangeEvent<string>>(evt =>
			{
				constraint.EnumType = Type.GetType(evt.newValue ?? string.Empty);
				SendChangeEvent();
			});

			return picker;
		}

		private void RefreshEnumConstraint(VisualElement container, EnumConstraint constraint)
		{
			var picker = container.Q<TypePickerControl>(className: EnumConstraintUssClassName);
			picker.SetValueWithoutNotify(constraint.EnumType.AssemblyQualifiedName);
		}

		private VisualElement CreateDictionaryConstraint(DictionaryConstraint constraint)
		{
			var picker = new ObjectPickerControl(constraint.Schema, _owner, typeof(VariableSchema));
			picker.AddToClassList(SchemaConstraintUssClassName);
			picker.RegisterCallback<ChangeEvent<Object>>(evt =>
			{
				constraint.Schema = evt.newValue as VariableSchema;
				SendChangeEvent();
			});

			return picker;
		}

		private void RefreshDictionaryConstraint(VisualElement container, DictionaryConstraint constraint)
		{
			var picker = container.Q<ObjectPickerControl>(className: SchemaConstraintUssClassName);
			picker.SetValueWithoutNotify(constraint.Schema);
		}

		private VisualElement CreateListConstraint(ListConstraint constraint)
		{
			var container = new VisualElement();
			var listContainer = new VisualElement();

			var dropdown = new EnumField(constraint.ItemType);
			dropdown.AddToClassList(ListItemConstraintUssClassName);
			dropdown.RegisterValueChangedCallback(evt =>
			{
				constraint.ItemType = (VariableType)evt.newValue;

				listContainer.Clear();
				CreateListConstraint(listContainer, constraint);

				SendChangeEvent();
			});

			container.Add(dropdown);
			container.Add(listContainer);

			if (constraint.ItemConstraint != null)
				CreateListConstraint(listContainer, constraint);

			return container;
		}

		private void CreateListConstraint(VisualElement container, ListConstraint constraint)
		{
			var constraintElement = new VisualElement();
			constraintElement.AddToClassList(ListConstraintUssClassName);
			CreateConstraint(constraintElement, constraint.ItemConstraint);
			container.Add(constraintElement);
		}

		private void RefreshListConstraint(VisualElement container, ListConstraint constraint)
		{
			var constraintElement = container.Q<VisualElement>(className: ListConstraintUssClassName);
			RefreshConstraint(constraintElement, constraint.ItemConstraint);
		}

		#endregion
	}
}
