using PiRhoSoft.Utilities.Editor;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace PiRhoSoft.Composition.Editor
{
	public class VariableControl : VisualElement
	{
		public const string Stylesheet = "Variables/Variable/VariableStyle.uss";
		public const string UssClassName = "pirho-variable";
		public const string EmptyUssClassName = UssClassName + "__empty";
		public const string BoolUssClassName = UssClassName + "__bool";
		public const string NumberUssClassName = UssClassName + "__number";
		public const string NumberHasRangeUssClassName = NumberUssClassName + "--has-range";
		public const string NumberSliderUssClassName = NumberUssClassName + "__slider";
		public const string NumberFieldUssClassName = NumberUssClassName + "__field";
		public const string FieldUssClassName = UssClassName + "__field";

		public Variable Value { get; private set; }
		public VariableDefinition Definition { get; private set; }

		private readonly Object _owner;

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
		private DictionaryControl _dictionaryControl;
		private VisualElement _assetContainer;
		private VisualElement _objectContainer;

		private IntegerField _intField;
		private SliderInt _intSlider;
		private FloatField _floatField;
		private Slider _floatSlider;
		private PopupField<string> _stringPopup;
		private TextField _stringField;
		private VariableListProxy _listProxy;
		private VariableDictionaryProxy _dictionaryProxy;

		public VariableControl(Variable value, VariableDefinition definition, Object owner)
		{
			Value = value;
			Definition = definition;
			_owner = owner;

			CreateElements();
			Refresh();

			this.AddStyleSheet(Configuration.EditorPath, Stylesheet);
			AddToClassList(UssClassName);
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
			CreateAsset();
			CreateObject();
		}

		public void Refresh()
		{
			if (Definition.Type != Value.Type)
			{
				SetValue(Definition.Generate());
			}
			else
			{
				switch (Value.Type)
				{
					case VariableType.Empty: RefreshEmpty(); break;
					case VariableType.Bool: RefreshBool(); break;
					case VariableType.Int: RefreshInt(); break;
					case VariableType.Float: RefreshFloat(); break;
					case VariableType.Vector2Int: RefreshVector2Int(); break;
					case VariableType.Vector3Int: RefreshVector3Int(); break;
					case VariableType.RectInt: RefreshRectInt(); break;
					case VariableType.BoundsInt: RefreshBoundsInt(); break;
					case VariableType.Vector2: RefreshVector2(); break;
					case VariableType.Vector3: RefreshVector3(); break;
					case VariableType.Vector4: RefreshVector4(); break;
					case VariableType.Quaternion: RefreshQuaternion(); break;
					case VariableType.Rect: RefreshRect(); break;
					case VariableType.Bounds: RefreshBounds(); break;
					case VariableType.Color: RefreshColor(); break;
					case VariableType.Enum: RefreshEnum(); break;
					case VariableType.String: RefreshString(); break;
					case VariableType.List: RefreshList(); break;
					case VariableType.Dictionary: RefreshDictionary(); break;
					case VariableType.Asset: RefreshAsset(); break;
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
				_dictionaryControl.SetDisplayed(Value.Type == VariableType.Dictionary);
				_assetContainer.SetDisplayed(Value.Type == VariableType.Asset);
				_objectContainer.SetDisplayed(Value.Type == VariableType.Object);
			}
		}

		private void CreateEmpty()
		{
			_emptyField = new EnumField(VariableType.Empty);
			_emptyField.AddToClassList(EmptyUssClassName);
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
			_boolToggle.AddToClassList(BoolUssClassName);
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
			_intContainer.AddToClassList(NumberUssClassName);

			_intSlider = new SliderInt();
			_intSlider.AddToClassList(NumberSliderUssClassName);
			_intSlider.RegisterValueChangedCallback(evt =>
			{
				var value = Variable.Int(evt.newValue);
				SetValue(value);
			});

			_intField = new IntegerField();
			_intField.AddToClassList(NumberFieldUssClassName);
			_intField.RegisterValueChangedCallback(evt =>
			{
				if (Definition.Constraint is IntConstraint constraint)
				{
					var clamped = Mathf.Clamp(evt.newValue, constraint.Minimum ?? evt.newValue, constraint.Maximum ?? evt.newValue);
					var value = Variable.Int(clamped);

					SetValue(value);
				}
			});

			_intContainer.Add(_intSlider);
			_intContainer.Add(_intField);

			Add(_intContainer);
		}

		private void RefreshInt()
		{
			if (Definition.Constraint is IntConstraint constraint)
			{
				_intContainer.EnableInClassList(NumberHasRangeUssClassName, constraint.Minimum.HasValue && constraint.Maximum.HasValue);
				_intSlider.SetValueWithoutNotify(Value.AsInt);
				_intSlider.lowValue = constraint.Minimum ?? IntConstraint.DefaultMinimum;
				_intSlider.highValue = constraint.Maximum ?? IntConstraint.DefaultMaximum;
				_intField.SetValueWithoutNotify(Value.AsInt);
			}
		}

		private void CreateFloat()
		{
			_floatContainer = new VisualElement();
			_floatContainer.AddToClassList(NumberUssClassName);

			_floatSlider = new Slider();
			_floatSlider.AddToClassList(NumberSliderUssClassName);
			_floatSlider.RegisterValueChangedCallback(evt =>
			{
				var value = Variable.Float(evt.newValue);
				SetValue(value);
			});

			_floatField = new FloatField();
			_floatField.AddToClassList(NumberFieldUssClassName);
			_floatField.RegisterValueChangedCallback(evt =>
			{
				if (Definition.Constraint is FloatConstraint constraint)
				{
					var clamped = Mathf.Clamp(evt.newValue, constraint.Minimum ?? evt.newValue, constraint.Maximum ?? evt.newValue);
					var value = Variable.Float(clamped);

					SetValue(value);
				}
			});

			_floatContainer.Add(_floatSlider);
			_floatContainer.Add(_floatField);

			Add(_floatContainer);
		}

		private void RefreshFloat()
		{
			if (Definition.Constraint is FloatConstraint constraint)
			{
				_floatContainer.EnableInClassList(NumberHasRangeUssClassName, constraint.Minimum.HasValue && constraint.Maximum.HasValue);
				_floatSlider.SetValueWithoutNotify(Value.AsFloat);
				_floatSlider.lowValue = constraint.Minimum ?? 0;
				_floatSlider.highValue = constraint.Maximum ?? 10;
				_floatField.SetValueWithoutNotify(Value.AsFloat);
			}
		}

		private void CreateVector2Int()
		{
			_vector2IntField = new Vector2IntField();
			_vector2IntField.AddToClassList(FieldUssClassName);
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
			_vector3IntField.AddToClassList(FieldUssClassName);
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
			_rectIntField.AddToClassList(FieldUssClassName);
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
			_boundsIntField.AddToClassList(FieldUssClassName);
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
			_vector2Field.AddToClassList(FieldUssClassName);
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
			_vector3Field.AddToClassList(FieldUssClassName);
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
			_vector4Field.AddToClassList(FieldUssClassName);
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
			_rectField.AddToClassList(FieldUssClassName);
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
			_boundsField.AddToClassList(FieldUssClassName);
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
			_colorField.AddToClassList(FieldUssClassName);
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
			_enumField.AddToClassList(FieldUssClassName);
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
			_stringContainer.AddToClassList(FieldUssClassName);

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

				if (!constraint.IsValid(Value))
				{
					var value = constraint.Generate();
					SetValue(value);
					return;
				}

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
			_listProxy = new VariableListProxy(_owner);
			_listControl = new ListControl(_listProxy);

			Add(_listControl);
		}

		private void RefreshList()
		{
			if (Definition.Constraint is ListConstraint constraint)
			{
				_listProxy.Definition = new VariableDefinition(string.Empty, constraint.ItemConstraint);
				_listProxy.Variables = Value.AsList;
				_listControl.Refresh();
			}
		}

		private void CreateDictionary()
		{
			_dictionaryProxy = new VariableDictionaryProxy(null, _owner);
			_dictionaryControl = new DictionaryControl(_dictionaryProxy);

			Add(_dictionaryControl);
		}

		private void RefreshDictionary()
		{
			if (Definition.Constraint is DictionaryConstraint constraint)
			{
				_dictionaryProxy.Variables = Value.AsDictionary;
				_dictionaryProxy.Schema = constraint.Schema;
				_dictionaryControl.Refresh();
			}
		}

		private void CreateAsset()
		{
			_assetContainer = new VisualElement();

			Add(_objectContainer);
		}

		private void RefreshAsset()
		{
			_assetContainer.Clear();

			var assetPicker = new ObjectPickerControl(Value.GetObject<Object>(), null, typeof(ScriptableObject));
			assetPicker.RegisterCallback<ChangeEvent<Object>>(evt =>
			{
				var value = Variable.Asset(new AssetReference(AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(evt.newValue))));
				SetValue(value);
			});

			_objectContainer.Add(assetPicker);
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

			var objectPicker = new ObjectPickerControl(Value.GetObject<Object>(), _owner, objectType);
			objectPicker.RegisterCallback<ChangeEvent<Object>>(evt =>
			{
				var value = Variable.Object(evt.newValue);
				SetValue(value);
			});

			_objectContainer.Add(objectPicker);
		}

		private class VariableListProxy : IListProxy
		{
			public string Label => "Variables";
			public string Tooltip => "The variables in this list";
			public string EmptyLabel => "This list has no variables";
			public string EmptyTooltip => "There are no variables in this list";
			public string AddTooltip => "Add a variable to this list";
			public string RemoveTooltip => "Remove this variable from the list";
			public string ReorderTooltip => "Move this variable in the list";

			public bool AllowAdd => true;
			public bool AllowRemove => true;
			public bool AllowReorder => true;

			public bool CanAdd() => true;
			public bool CanRemove(int index) => true;
			public bool CanReorder(int from, int to) => true;

			public IVariableList Variables;
			public VariableDefinition Definition;

			private readonly Object _owner;

			public int ItemCount => Variables?.VariableCount ?? 0;

			public VariableListProxy(Object owner)
			{
				_owner = owner;
			}

			public VisualElement CreateElement(int index)
			{
				var value = Variables.GetVariable(index);
				var control = new VariableControl(value, Definition, _owner) { userData = index };
				control.RegisterCallback<ChangeEvent<Variable>>(evt => Variables.SetVariable(index, evt.newValue));
				return control;
			}

			public bool NeedsUpdate(VisualElement item, int index)
			{
				return !(item.userData is int i) || i != index;
			}

			public void AddItem()
			{
				Variables.AddVariable(Definition.Generate());
			}

			public void RemoveItem(int index)
			{
				Variables.RemoveVariable(index);
			}

			public void ReorderItem(int from, int to)
			{
				var variable = Variables.GetVariable(from);
				Variables.RemoveVariable(from);
				Variables.InsertVariable(to, variable);
			}
		}

		private class VariableDictionaryProxy : IDictionaryProxy
		{
			public IVariableDictionary Variables;
			public VariableSchema Schema;

			public int KeyCount => Variables?.VariableNames.Count ?? 0;

			public string Label => "Variables";
			public string Tooltip => "The Variables in this dictionary";
			public string EmptyLabel => "This Dictionary has no Variables";
			public string EmptyTooltip => "There are no Variables in this dictionary";
			public string AddPlaceholder => "New Variable";
			public string AddTooltip => "Add a Variable to this dictionary";
			public string RemoveTooltip => "Remove this Variable";
			public string ReorderTooltip => "Move this Variable";

			public bool AllowAdd => !Schema;
			public bool AllowRemove => !Schema;
			public bool AllowReorder => false;

			private readonly Object _owner;

			public VariableDictionaryProxy(IVariableDictionary variables, Object owner)
			{
				Variables = variables;

				_owner = owner;
			}

			public VisualElement CreateField(int index)
			{
				var name = GetName(index);
				var value = Variables.GetVariable(name);
				var definition = GetDefinition(name);
				var control = new VariableControl(value, definition, _owner) { userData = index };
				return control;
			}

			public bool NeedsUpdate(VisualElement item, int index)
			{
				return !(item.userData is int i) || i != index;
			}

			public bool CanAdd(string key)
			{
				return AllowAdd && !Variables.VariableNames.Contains(key);
			}

			public bool CanRemove(int index)
			{
				return AllowRemove;
			}

			public bool CanReorder(int from, int to)
			{
				return AllowReorder;
			}

			public void AddItem(string key)
			{
				var definition = GetDefinition(key);
				Variables.SetVariable(key, definition.Generate());
			}

			public void RemoveItem(int index)
			{
				var name = GetName(index);
				Variables.SetVariable(name, Variable.Empty);
			}

			public void ReorderItem(int from, int to)
			{
			}

			private string GetName(int index)
			{
				return Variables.VariableNames[index];
			}

			private VariableDefinition GetDefinition(string name)
			{
				return Schema ? Schema.GetEntry(name).Definition : new VariableDefinition(name);
			}
		}
	}
}
