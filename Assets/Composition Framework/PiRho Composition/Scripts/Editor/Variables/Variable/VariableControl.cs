using PiRhoSoft.Utilities.Editor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace PiRhoSoft.Composition.Editor
{
	public class VariableControl : VisualElement
	{
		public Variable Value { get; private set; }
		public VariableDefinition Definition { get; private set; }

		public VariableControl(Variable value, VariableDefinition definition)
		{
			Value = value;
			Definition = definition;
			Refresh();
		}

		public void SetValueWithoutNotify(Variable value)
		{
			Value = value;
			Refresh();
		}

		public void SetValue(Variable value)
		{
			var previous = Value;
			SetValueWithoutNotify(value);
			this.SendChangeEvent(previous, Value);
		}

		public void SetDefinition(VariableDefinition definition)
		{
			Definition = definition;

			if (Definition.Type != Value.Type)
				SetValue(Definition.Generate());
			else
				Refresh();
		}

		private void Refresh()
		{
			Clear();
			Add(CreateElement());
		}

		private VisualElement CreateElement()
		{
			switch (Value.Type)
			{
				case VariableType.Empty: return CreateEmpty();
				case VariableType.Bool: return CreateBool();
				case VariableType.Int: return CreateInt();
				case VariableType.Float: return CreateFloat();
				case VariableType.Vector2Int: return CreateVector2Int();
				case VariableType.Vector3Int: return CreateVector3Int();
				case VariableType.RectInt: return CreateRectInt();
				case VariableType.BoundsInt: return CreateBoundsInt();
				case VariableType.Vector2: return CreateVector2();
				case VariableType.Vector3: return CreateVector3();
				case VariableType.Vector4: return CreateVector4();
				case VariableType.Quaternion: return CreateQuaternion();
				case VariableType.Rect: return CreateRect();
				case VariableType.Bounds: return CreateBounds();
				case VariableType.Color: return CreateColor();
				case VariableType.Enum: return CreateEnum();
				case VariableType.String: return CreateString();
				case VariableType.List: return CreateList();
				case VariableType.Dictionary: return CreateDictionary();
				case VariableType.Object: return CreateObject();
				default: return null;
			}
		}

		private VisualElement CreateEmpty()
		{
			var dropdown = new EnumField(VariableType.Empty);
			dropdown.RegisterValueChangedCallback(evt =>
			{
				var type = (VariableType)evt.newValue;
				var value = Variable.Create(type);

				Definition.Type = type;
				SetValue(value);
			});

			return dropdown;
		}

		public VisualElement CreateBool()
		{
			var toggle = new Toggle() { value = Value.AsBool };
			toggle.RegisterValueChangedCallback(evt =>
			{
				var value = Variable.Bool(evt.newValue);
				SetValue(value);
			});

			return toggle;
		}

		public VisualElement CreateInt()
		{
			var container = new VisualElement();
			container.style.flexDirection = FlexDirection.Row;

			if (Definition.Constraint is IntConstraint constraint)
			{
				var field = new IntegerField() { value = Value.AsInt, isDelayed = true };
				field.RegisterValueChangedCallback(evt =>
				{
					var clamped = Mathf.Clamp(evt.newValue, constraint.Minimum ?? evt.newValue, constraint.Maximum ?? evt.newValue);
					var value = Variable.Int(clamped);

					SetValue(value);
				});

				container.Add(field);

				if (constraint.Minimum.HasValue && constraint.Maximum.HasValue)
				{
					var slider = new SliderInt(constraint.Minimum.Value, constraint.Maximum.Value) { value = Value.AsInt };
					slider.RegisterValueChangedCallback(evt =>
					{
						var value = Variable.Int(evt.newValue);
						SetValue(value);
					});

					container.Add(slider);
				}
			}

			return container;
		}

		private VisualElement CreateFloat()
		{
			var container = new VisualElement();
			container.style.flexDirection = FlexDirection.Row;

			if (Definition.Constraint is FloatConstraint constraint)
			{
				var field = new FloatField() { value = Value.AsFloat, isDelayed = true };
				field.RegisterValueChangedCallback(evt =>
				{
					var clamped = Mathf.Clamp(evt.newValue, constraint.Minimum ?? evt.newValue, constraint.Maximum ?? evt.newValue);
					var value = Variable.Float(clamped);

					SetValue(value);
				});

				container.Add(field);

				if (constraint.Minimum.HasValue && constraint.Maximum.HasValue)
				{
					var slider = new Slider(constraint.Minimum.Value, constraint.Maximum.Value) { value = Value.AsFloat };
					slider.RegisterValueChangedCallback(evt =>
					{
						var value = Variable.Float(evt.newValue);
						SetValue(value);
					});

					container.Add(slider);
				}
			}

			return container;
		}

		private VisualElement CreateVector2Int()
		{
			var field = new Vector2IntField() { value = Value.AsVector2Int };
			field.RegisterValueChangedCallback(evt =>
			{
				var value = Variable.Vector2Int(evt.newValue);
				SetValue(value);
			});

			return field;
		}

		private VisualElement CreateVector3Int()
		{
			var field = new Vector3IntField() { value = Value.AsVector3Int };
			field.RegisterValueChangedCallback(evt =>
			{
				var value = Variable.Vector3Int(evt.newValue);
				SetValue(value);
			});

			return field;
		}

		private VisualElement CreateRectInt()
		{
			var field = new RectIntField() { value = Value.AsRectInt };
			field.RegisterValueChangedCallback(evt =>
			{
				var value = Variable.RectInt(evt.newValue);
				SetValue(value);
			});

			return field;
		}

		private VisualElement CreateBoundsInt()
		{
			var field = new BoundsIntField() { value = Value.AsBoundsInt };
			field.RegisterValueChangedCallback(evt =>
			{
				var value = Variable.BoundsInt(evt.newValue);
				SetValue(value);
			});

			return field;
		}

		private VisualElement CreateVector2()
		{
			var field = new Vector2Field() { value = Value.AsVector2 };
			field.RegisterValueChangedCallback(evt =>
			{
				var value = Variable.Vector2(evt.newValue);
				SetValue(value);
			});

			return field;
		}

		private VisualElement CreateVector3()
		{
			var field = new Vector3Field() { value = Value.AsVector3 };
			field.RegisterValueChangedCallback(evt =>
			{
				var value = Variable.Vector3(evt.newValue);
				SetValue(value);
			});

			return field;
		}

		private VisualElement CreateVector4()
		{
			var field = new Vector4Field() { value = Value.AsVector4 };
			field.RegisterValueChangedCallback(evt =>
			{
				var value = Variable.Vector4(evt.newValue);
				SetValue(value);
			});

			return field;
		}

		private VisualElement CreateQuaternion()
		{
			var field = new EulerControl(Value.AsQuaternion);
			field.RegisterCallback<ChangeEvent<Quaternion>>(evt =>
			{
				var value = Variable.Quaternion(evt.newValue);
				SetValue(value);
			});

			return field;
		}

		private VisualElement CreateRect()
		{
			var field = new RectField() { value = Value.AsRect };
			field.RegisterValueChangedCallback(evt =>
			{
				var value = Variable.Rect(evt.newValue);
				SetValue(value);
			});

			return field;
		}

		private VisualElement CreateBounds()
		{
			var field = new BoundsField() { value = Value.AsBounds };
			field.RegisterValueChangedCallback(evt =>
			{
				var value = Variable.Bounds(evt.newValue);
				SetValue(value);
			});

			return field;
		}

		private VisualElement CreateColor()
		{
			var field = new ColorField() { value = Value.AsColor };
			field.RegisterValueChangedCallback(evt =>
			{
				var value = Variable.Color(evt.newValue);
				SetValue(value);
			});

			return field;
		}

		private VisualElement CreateEnum()
		{
			var field = new EnumField(Value.AsEnum);
			field.RegisterValueChangedCallback(evt =>
			{
				var value = Variable.Enum(evt.newValue);
				SetValue(value);
			});

			return field;
		}

		private VisualElement CreateString()
		{
			if (Definition.Constraint is StringConstraint constraint && constraint.Values.Count > 0)
			{
				var popup = new PopupField<string>(constraint.Values, Value.AsString);
				popup.RegisterValueChangedCallback(evt =>
				{
					var value = Variable.String(evt.newValue);
					SetValue(value);
				});

				return popup;
			}
			else
			{
				var field = new TextField() { value = Value.AsString, isDelayed = true };
				field.RegisterValueChangedCallback(evt =>
				{
					var value = Variable.String(evt.newValue);
					SetValue(value);
				});

				return field;
			}
		}

		private VisualElement CreateList()
		{
			var list = Value.AsList;
			var constraint = (Definition.Constraint as ListConstraint).ItemConstraint;
			var definition = new VariableDefinition(string.Empty, constraint);
			var proxy = new VariableListProxy(list, definition)
			{
				Label = "Variables",
				EmptyLabel = "This List has no Variables",
				AddTooltip = "Add a Variable to this List",
				RemoveTooltip = "Remove this Variables"
			};

			var control = new ListControl(proxy);

			return control;
		}

		private VisualElement CreateDictionary()
		{
			return new VisualElement();
		}

		private VisualElement CreateObject()
		{
			var objectType = (Definition.Constraint as ObjectConstraint)?.ObjectType ?? typeof(Object);

			var picker = new ObjectPickerControl(Value.GetObject<Object>(), objectType);
			picker.RegisterCallback<ChangeEvent<Object>>(evt =>
			{
				var value = Variable.Object(evt.newValue);
				SetValue(value);
			});

			return picker;
		}

		private class VariableListProxy : ListProxy
		{
			public IVariableList Variables;
			public VariableDefinition Definition;

			public override int ItemCount => Variables.VariableCount;

			public VariableListProxy(IVariableList variables, VariableDefinition definition)
			{
				Variables = variables;
				Definition = definition;
			}

			public override VisualElement CreateElement(int index)
			{
				var value = Variables.GetVariable(index);
				var control = new VariableControl(value, Definition) { userData = index };
				return control;
			}

			public override bool NeedsUpdate(VisualElement item, int index)
			{
				return !(item.userData is int i) || i != index;
			}

			public override void AddItem()
			{
				Variables.AddVariable(Definition.Generate());
			}

			public override void RemoveItem(int index)
			{
				Variables.RemoveVariable(index);
			}

			public override void ReorderItem(int from, int to)
			{
				var previous = Variables.GetVariable(to);
				Variables.SetVariable(to, Variables.GetVariable(from));
				Variables.SetVariable(from, previous);
			}
		}
	}
}
