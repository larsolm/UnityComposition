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

		private EnumField _emptyField;
		private Toggle _boolToggle;
		private VisualElement _intContainer;
		private VisualElement _floatContainer;
		private Vector2IntField _vector2IntField;
		private Vector3IntField _vector3IntField;
		private RectIntField _rectIntField;
		private BoundsIntField _boundsIntField;
		private Vector2Field _vector2Field;
		private Vector3Field _vector3Field;
		private Vector4Field _vector4Field;
		private EulerControl _quaternionContainer;
		private RectField _rectField;
		private BoundsField _boundsField;
		private ColorField _colorField;
		private EnumField _enumField;
		private VisualElement _stringContainer;
		private ListControl _listControl;
		private VisualElement _dictionaryContainer;
		private VisualElement _objectContainer;

		private IntegerField _intField;
		private SliderInt _intSlider;
		private FloatField _floatField;
		private Slider _floatSlider;
		private PopupField<string> _stringPopup;
		private TextField _stringField;
		private VariableListProxy _listProxy;

		public VariableControl(Variable value, VariableDefinition definition)
		{
			style.flexGrow = 1;

			Value = value;
			Definition = definition;

			CreateElements();
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

		private void CreateElements()
		{
			CreateEmpty();
			CreateBool();
			CreateInt();
			CreateFloat();
			CreateVector2Int();
			CreateVector3Int();
			CreateRectInt();
			CreateBoundsInt();
			CreateVector2();
			CreateVector3();
			CreateVector4();
			CreateQuaternion();
			CreateRect();
			CreateBounds();
			CreateColor();
			CreateEnum();
			CreateString();
			CreateList();
			CreateDictionary();
			CreateObject();
		}

		public void Refresh()
		{
			switch (Value.Type)
			{
				case VariableType.Empty: RefreshEmpty(); break;
				case VariableType.Bool: RefreshBool(); break;
				case VariableType.Int: RefreshInt();  break;
				case VariableType.Float: RefreshFloat(); break;
				case VariableType.Vector2Int: RefreshVector2Int();  break;
				case VariableType.Vector3Int: RefreshVector3Int();  break;
				case VariableType.RectInt: RefreshRectInt();  break;
				case VariableType.BoundsInt: RefreshBoundsInt();  break;
				case VariableType.Vector2: RefreshVector2(); break;
				case VariableType.Vector3: RefreshVector3();  break;
				case VariableType.Vector4: RefreshVector3();  break;
				case VariableType.Quaternion: RefreshQuaternion();  break;
				case VariableType.Rect: RefreshRect();  break;
				case VariableType.Bounds: RefreshBounds();  break;
				case VariableType.Color: RefreshColor();  break;
				case VariableType.Enum: RefreshEnum();  break;
				case VariableType.String: RefreshString(); break;
				case VariableType.List: RefreshList(); break;
				case VariableType.Store: RefreshDictionary();  break;
				case VariableType.Object: RefreshObject(); break;
			}

			_emptyField.SetDisplayed(Value.Type == VariableType.Empty);
			_boolToggle.SetDisplayed(Value.Type == VariableType.Bool);
			_intContainer.SetDisplayed(Value.Type == VariableType.Int);
			_floatContainer.SetDisplayed(Value.Type == VariableType.Float);
			_vector2IntField.SetDisplayed(Value.Type == VariableType.Vector2Int);
			_vector3IntField.SetDisplayed(Value.Type == VariableType.Vector3Int);
			_rectIntField.SetDisplayed(Value.Type == VariableType.RectInt);
			_boundsIntField.SetDisplayed(Value.Type == VariableType.BoundsInt);
			_vector2Field.SetDisplayed(Value.Type == VariableType.Vector2);
			_vector3Field.SetDisplayed(Value.Type == VariableType.Vector3);
			_vector4Field.SetDisplayed(Value.Type == VariableType.Vector4);
			_quaternionContainer.SetDisplayed(Value.Type == VariableType.Quaternion);
			_rectField.SetDisplayed(Value.Type == VariableType.Rect);
			_boundsField.SetDisplayed(Value.Type == VariableType.Bounds);
			_colorField.SetDisplayed(Value.Type == VariableType.Color);
			_enumField.SetDisplayed(Value.Type == VariableType.Enum);
			_stringContainer.SetDisplayed(Value.Type == VariableType.String);
			_listControl.SetDisplayed(Value.Type == VariableType.List);
			_dictionaryContainer.SetDisplayed(Value.Type == VariableType.Store);
			_objectContainer.SetDisplayed(Value.Type == VariableType.Object);
		}

		private void CreateEmpty()
		{
			_emptyField = new EnumField(VariableType.Empty);
			_emptyField.RegisterValueChangedCallback(evt =>
			{
				var type = (VariableType)evt.newValue;
				var value = Variable.Create(type);

				Definition.Type = type;
				SetValue(value);
			});

			Add(_emptyField);
		}

		private void RefreshEmpty()
		{
			_emptyField.SetValueWithoutNotify(VariableType.Empty);
		}

		private void CreateBool()
		{
			_boolToggle = new Toggle();
			_boolToggle.RegisterValueChangedCallback(evt =>
			{
				var value = Variable.Bool(evt.newValue);
				SetValue(value);
			});

			Add(_boolToggle);
		}

		private void RefreshBool()
		{
			_boolToggle.SetValueWithoutNotify(Value.AsBool);
		}

		private void CreateInt()
		{
			_intContainer = new VisualElement();
			
			_intSlider = new SliderInt();
			_intSlider.RegisterValueChangedCallback(evt =>
			{
				var value = Variable.Int(evt.newValue);
				SetValue(value);
			});

			_intField = new IntegerField();
			_intField.RegisterValueChangedCallback(evt =>
			{
				if (Definition.Constraint is IntConstraint constraint)
				{
					var clamped = Mathf.Clamp(evt.newValue, constraint.Minimum ?? evt.newValue, constraint.Maximum ?? evt.newValue);
					var value = Variable.Int(clamped);

					SetValue(value);
				}
			});

			_intContainer.style.flexDirection = FlexDirection.Row;

			_intSlider.style.flexGrow = 1;
			_intSlider.style.paddingLeft = 5;

			_intField.style.minWidth = 50;
			_intField.style.paddingLeft = 5;

			_intContainer.Add(_intSlider);
			_intContainer.Add(_intField);

			Add(_intContainer);
		}

		private void RefreshInt()
		{
			if (Definition.Constraint is IntConstraint constraint)
			{
				var hasRange = constraint.Minimum.HasValue && constraint.Maximum.HasValue;

				_intSlider.SetDisplayed(hasRange);
				_intSlider.SetValueWithoutNotify(Value.AsInt);
				_intSlider.lowValue = constraint.Minimum ?? 0;
				_intSlider.highValue = constraint.Maximum ?? 10;

				_intField.SetValueWithoutNotify(Value.AsInt);
				_intField.style.flexGrow = hasRange ? 0 : 1;
			}
		}

		private void CreateFloat()
		{
			_floatContainer = new VisualElement();

			_floatSlider = new Slider();
			_floatSlider.RegisterValueChangedCallback(evt =>
			{
				var value = Variable.Float(evt.newValue);
				SetValue(value);
			});

			_floatField = new FloatField();
			_floatField.RegisterValueChangedCallback(evt =>
			{
				if (Definition.Constraint is FloatConstraint constraint)
				{
					var clamped = Mathf.Clamp(evt.newValue, constraint.Minimum ?? evt.newValue, constraint.Maximum ?? evt.newValue);
					var value = Variable.Float(clamped);

					SetValue(value);
				}
			});

			_floatContainer.style.flexDirection = FlexDirection.Row;

			_floatSlider.style.flexGrow = 1;
			_floatSlider.style.paddingLeft = 5;

			_floatField.style.minWidth = 50;
			_floatField.style.paddingLeft = 5;

			_floatContainer.Add(_floatSlider);
			_floatContainer.Add(_floatField);

			Add(_floatContainer);
		}

		private void RefreshFloat()
		{
			if (Definition.Constraint is FloatConstraint constraint)
			{
				var hasRange = constraint.Minimum.HasValue && constraint.Maximum.HasValue;

				_floatSlider.SetDisplayed(hasRange);
				_floatSlider.SetValueWithoutNotify(Value.AsFloat);
				_floatSlider.lowValue = constraint.Minimum ?? 0;
				_floatSlider.highValue = constraint.Maximum ?? 10;

				_floatField.SetValueWithoutNotify(Value.AsFloat);
				_floatField.style.flexGrow = hasRange ? 0 : 1;
			}
		}

		private void CreateVector2Int()
		{
			_vector2IntField = new Vector2IntField();
			_vector2IntField.RegisterValueChangedCallback(evt =>
			{
				var value = Variable.Vector2Int(evt.newValue);
				SetValue(value);
			});

			Add(_vector2IntField);
		}

		private void RefreshVector2Int()
		{
			_vector2IntField.SetValueWithoutNotify(Value.AsVector2Int);
		}

		private void CreateVector3Int()
		{
			_vector3IntField = new Vector3IntField();
			_vector3IntField.RegisterValueChangedCallback(evt =>
			{
				var value = Variable.Vector3Int(evt.newValue);
				SetValue(value);
			});

			Add(_vector3IntField);
		}

		private void RefreshVector3Int()
		{
			_vector3IntField.SetValueWithoutNotify(Value.AsVector3Int);
		}

		private void CreateRectInt()
		{
			_rectIntField = new RectIntField();
			_rectIntField.RegisterValueChangedCallback(evt =>
			{
				var value = Variable.RectInt(evt.newValue);
				SetValue(value);
			});

			Add(_rectIntField);
		}

		private void RefreshRectInt()
		{
			_rectIntField.SetValueWithoutNotify(Value.AsRectInt);
		}

		private void CreateBoundsInt()
		{
			_boundsIntField = new BoundsIntField();
			_boundsIntField.RegisterValueChangedCallback(evt =>
			{
				var value = Variable.BoundsInt(evt.newValue);
				SetValue(value);
			});

			Add(_boundsIntField);
		}

		private void RefreshBoundsInt()
		{
			_boundsIntField.SetValueWithoutNotify(Value.AsBoundsInt);
		}

		private void CreateVector2()
		{
			_vector2Field = new Vector2Field();
			_vector2Field.RegisterValueChangedCallback(evt =>
			{
				var value = Variable.Vector2(evt.newValue);
				SetValue(value);
			});

			Add(_vector2Field);
		}

		private void RefreshVector2()
		{
			_vector2Field.SetValueWithoutNotify(Value.AsVector2);
		}

		private void CreateVector3()
		{
			_vector3Field = new Vector3Field();
			_vector3Field.RegisterValueChangedCallback(evt =>
			{
				var value = Variable.Vector3(evt.newValue);
				SetValue(value);
			});

			Add(_vector3Field);
		}

		private void RefreshVector3()
		{
			_vector3Field.SetValueWithoutNotify(Value.AsVector3);
		}

		private void CreateVector4()
		{
			_vector4Field = new Vector4Field();
			_vector4Field.RegisterValueChangedCallback(evt =>
			{
				var value = Variable.Vector4(evt.newValue);
				SetValue(value);
			});

			Add(_vector4Field);
		}

		private void RefreshVector4()
		{
			_vector4Field.SetValueWithoutNotify(Value.AsVector4);
		}

		private void CreateQuaternion()
		{
			_quaternionContainer = new EulerControl(Quaternion.identity);
			_quaternionContainer.RegisterCallback<ChangeEvent<Quaternion>>(evt =>
			{
				var value = Variable.Quaternion(evt.newValue);
				SetValue(value);
			});

			Add(_quaternionContainer);
		}

		private void RefreshQuaternion()
		{
			_quaternionContainer.SetValueWithoutNotify(Value.AsQuaternion);
		}

		private void CreateRect()
		{
			_rectField = new RectField();
			_rectField.RegisterValueChangedCallback(evt =>
			{
				var value = Variable.Rect(evt.newValue);
				SetValue(value);
			});

			Add(_rectField);
		}

		private void RefreshRect()
		{
			_rectField.SetValueWithoutNotify(Value.AsRect);
		}

		private void CreateBounds()
		{
			_boundsField = new BoundsField();
			_boundsField.RegisterValueChangedCallback(evt =>
			{
				var value = Variable.Bounds(evt.newValue);
				SetValue(value);
			});

			Add(_boundsField);
		}

		private void RefreshBounds()
		{
			_boundsField.SetValueWithoutNotify(Value.AsBounds);
		}

		private void CreateColor()
		{
			_colorField = new ColorField();
			_colorField.RegisterValueChangedCallback(evt =>
			{
				var value = Variable.Color(evt.newValue);
				SetValue(value);
			});

			Add(_colorField);
		}

		private void RefreshColor()
		{
			_colorField.SetValueWithoutNotify(Value.AsColor);
		}

		private void CreateEnum()
		{
			_enumField = new EnumField();
			_enumField.RegisterValueChangedCallback(evt =>
			{
				var value = Variable.Enum(evt.newValue);
				SetValue(value);
			});

			Add(_enumField);
		}

		private void RefreshEnum()
		{
			_enumField.Init(Value.AsEnum);
		}

		private void CreateString()
		{
			_stringContainer = new VisualElement();

			_stringField = new TextField { isDelayed = true };
			_stringField.RegisterValueChangedCallback(evt =>
			{
				var value = Variable.String(evt.newValue);
				SetValue(value);
			});

			_stringContainer.Add(_stringField);

			Add(_stringContainer);
		}

		private void RefreshString()
		{
			if (Definition.Constraint is StringConstraint constraint && constraint.Values.Count > 0)
			{
				if (_stringPopup != null)
					_stringPopup.RemoveFromHierarchy();

				// This has to be recreated because PopupField must be initialized with it's options
				_stringPopup = new PopupField<string>(constraint.Values, Value.AsString);
				_stringPopup.RegisterValueChangedCallback(evt =>
				{
					var value = Variable.String(evt.newValue);
					SetValue(value);
				});

				_stringField.SetDisplayed(false);
				_stringContainer.Add(_stringPopup);
			}
			else
			{
				_stringPopup?.SetDisplayed(false);
				_stringField.SetDisplayed(true);
				_stringField.SetValueWithoutNotify(Value.AsString);
			}
		}

		private void CreateList()
		{
			_listProxy = new VariableListProxy(null, null)
			{
				Label = "Variables",
				EmptyLabel = "This List has no Variables",
				AddTooltip = "Add a Variable to this List",
				RemoveTooltip = "Remove this Variables"
			};

			_listControl = new ListControl(_listProxy);

			Add(_listControl);
		}

		private void RefreshList()
		{
			var constraint = (Definition.Constraint as ListConstraint).ItemConstraint;

			_listProxy.Definition = new VariableDefinition(string.Empty, constraint);
			_listProxy.Variables = Value.AsList;
			_listControl.Refresh();
		}

		private void CreateDictionary()
		{
			_dictionaryContainer = new VisualElement();

			Add(_dictionaryContainer);
		}

		private void RefreshDictionary()
		{
		}

		private void CreateObject()
		{
			_objectContainer = new VisualElement();

			Add(_objectContainer);
		}

		private void RefreshObject()
		{
			_objectContainer.Clear();

			var objectType = (Definition.Constraint as ObjectConstraint)?.ObjectType ?? typeof(Object);

			var objectPicker = new ObjectPickerControl(Value.AsObject, objectType);
			objectPicker.RegisterCallback<ChangeEvent<Object>>(evt =>
			{
				var value = Variable.Object(evt.newValue);
				SetValue(value);
			});

			_objectContainer.Add(objectPicker);
		}

		private class VariableListProxy : ListProxy
		{
			public IVariableList Variables;
			public VariableDefinition Definition;

			public override int ItemCount => Variables?.Count ?? 0;

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
