using PiRhoSoft.CompositionEngine;
using PiRhoSoft.PargonUtilities.Editor;
using System;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace PiRhoSoft.CompositionEditor
{
	public class VariableValueElement : VisualElement, IBindableObject<VariableValue>
	{
		private readonly Object _owner;
		private readonly Func<VariableValue> _getValue;
		private readonly Action<VariableValue> _setValue;
		private readonly Func<ValueDefinition> _getDefinition;

		private VariableValue _value;
		private ValueDefinition _definition;

		public VariableValueElement(Object owner, Func<VariableValue> getValue, Action<VariableValue> setValue, Func<ValueDefinition> getDefinition)
		{
			_owner = owner;
			_getValue = getValue;
			_setValue = setValue;
			_getDefinition = getDefinition;

			schedule.Execute(() =>
			{
				var definition = _getDefinition();
				if (definition.Constraint != _definition.Constraint)
					Setup(_value, definition);
			}).Every(100);

			ElementHelper.Bind(this, this, _owner);

			Setup(_getValue(), _getDefinition());
		}

		public VariableValue GetValueFromElement(VisualElement element) => _value;
		public VariableValue GetValueFromObject(Object owner) => _getValue();
		public void UpdateElement(VariableValue value, VisualElement element, Object owner) => Setup(value, _definition);
		public void UpdateObject(VariableValue value, VisualElement element, Object owner) => _setValue(value);

		public void Setup(VariableValue value, ValueDefinition definition)
		{
			Clear();

			_value = value;
			_definition = definition;

			var element = CreateElement();

			Add(element);
		}

		private void SetValue(VariableValue value)
		{
			ElementHelper.SendChangeEvent(this, _value, value);
			_value = value;
		}

		private VisualElement CreateElement()
		{
			switch (_value.Type)
			{
				case VariableType.Empty: return CreateEmpty();
				case VariableType.Bool: return CreateBool();
				case VariableType.Int: return CreateInt();
				case VariableType.Float: return CreateFloat();
				case VariableType.Int2: return CreateInt2();
				case VariableType.Int3: return CreateInt3();
				case VariableType.IntRect: return CreateIntRect();
				case VariableType.IntBounds: return CreateIntBounds();
				case VariableType.Vector2: return CreateVector2();
				case VariableType.Vector3: return CreateVector3();
				case VariableType.Vector4: return CreateVector4();
				case VariableType.Quaternion: return CreateQuaternion();
				case VariableType.Rect: return CreateRect();
				case VariableType.Bounds: return CreateBounds();
				case VariableType.Color: return CreateColor();
				case VariableType.String: return CreateString();
				case VariableType.Enum: return CreateEnum();
				case VariableType.Object: return CreateObject();
				case VariableType.Store: return CreateStore();
				case VariableType.List: return CreateList();
				default: return null;
			}
		}

		private VisualElement CreateEmpty()
		{
			var dropdown = new EnumDropdown<VariableType>(VariableType.Empty, _owner, () => (int)VariableType.Empty, type =>
			{
				var variableType = (VariableType)type;
				var constraint = VariableConstraint.Create(variableType);
				var value = VariableHandler.CreateDefault((VariableType)type, constraint);
				var definition = ValueDefinition.Create(variableType, constraint, null, null, false, false);

				SetValue(value);
				Setup(value, definition);
			});
			return dropdown;
		}

		public VisualElement CreateBool()
		{
			var toggle = new Toggle() { value = _value.Bool };
			ElementHelper.Bind(this, toggle, _owner, () => _value.Bool, value => SetValue(VariableValue.Create(value)));
			return toggle;
		}

		public VisualElement CreateInt()
		{
			var container = new VisualElement();

			if (_definition.Constraint is IntVariableConstraint constraint)
			{
				var field = new IntegerField() { value = _value.Int, isDelayed = true };
				container.Add(field);

				ElementHelper.Bind(this, field, _owner, () => _value.Int, value =>
				{
					var clamped = constraint.HasRange ? Mathf.Clamp(value, constraint.Minimum, constraint.Maximum) : value;
					SetValue(VariableValue.Create(clamped));
				});

				if (constraint.HasRange)
				{
					var slider = new SliderInt(constraint.Minimum, constraint.Maximum) { value = _value.Int };
					container.Add(slider);

					ElementHelper.Bind(this, slider, _owner, () => _value.Int, value => SetValue(VariableValue.Create(value)));
				}
			}

			return container;
		}

		private VisualElement CreateFloat()
		{
			var container = new VisualElement();

			if (_definition.Constraint is FloatVariableConstraint constraint)
			{
				var field = new FloatField() { value = _value.Float, isDelayed = true };
				container.Add(field);

				ElementHelper.Bind(this, field, _owner, () => _value.Float, value =>
				{
					var clamped = constraint.HasRange ? Mathf.Clamp(value, constraint.Minimum, constraint.Maximum) : value;
					SetValue(VariableValue.Create(clamped));
				});

				if (constraint.HasRange)
				{
					var slider = new Slider(constraint.Minimum, constraint.Maximum) { value = _value.Float };
					container.Add(slider);

					ElementHelper.Bind(this, slider, _owner, () => _value.Float, value => SetValue(VariableValue.Create(value)));
				}
			}

			return container;
		}

		private VisualElement CreateInt2()
		{
			var field = new Vector2IntField() { value = _value.Int2 };
			ElementHelper.Bind(this, field, _owner, () => _value.Int2, value => SetValue(VariableValue.Create(value)));
			return field;
		}

		private VisualElement CreateInt3()
		{
			var field = new Vector3IntField() { value = _value.Int3 };
			ElementHelper.Bind(this, field, _owner, () => _value.Int3, value => SetValue(VariableValue.Create(value)));
			return field;
		}

		private VisualElement CreateIntRect()
		{
			var field = new RectIntField() { value = _value.IntRect };
			ElementHelper.Bind(this, field, _owner, () => _value.IntRect, value => SetValue(VariableValue.Create(value)));
			return field;
		}

		private VisualElement CreateIntBounds()
		{
			var field = new BoundsIntField() { value = _value.IntBounds };
			ElementHelper.Bind(this, field, _owner, () => _value.IntBounds, value => SetValue(VariableValue.Create(value)));
			return field;
		}

		private VisualElement CreateVector2()
		{
			var field = new Vector2Field() { value = _value.Vector2 };
			ElementHelper.Bind(this, field, _owner, () => _value.Vector2, value => SetValue(VariableValue.Create(value)));
			return field;
		}

		private VisualElement CreateVector3()
		{
			var field = new Vector3Field() { value = _value.Vector3 };
			ElementHelper.Bind(this, field, _owner, () => _value.Vector3, value => SetValue(VariableValue.Create(value)));
			return field;
		}

		private VisualElement CreateVector4()
		{
			var field = new Vector4Field() { value = _value.Vector4 };
			ElementHelper.Bind(this, field, _owner, () => _value.Vector4, value => SetValue(VariableValue.Create(value)));
			return field;
		}

		private VisualElement CreateQuaternion()
		{
			return null;//new Euler(_owner, () => _value.Quaternion, value => SetValue(VariableValue.Create(value)));
		}

		private VisualElement CreateRect()
		{
			var field = new RectField() { value = _value.Rect };
			ElementHelper.Bind(this, field, _owner, () => _value.Rect, value => SetValue(VariableValue.Create(value)));
			return field;
		}

		private VisualElement CreateBounds()
		{
			var field = new BoundsField() { value = _value.Bounds };
			ElementHelper.Bind(this, field, _owner, () => _value.Bounds, value => SetValue(VariableValue.Create(value)));
			return field;
		}

		private VisualElement CreateColor()
		{
			var field = new ColorField() { value = _value.Color };
			ElementHelper.Bind(this, field, _owner, () => _value.Color, value => SetValue(VariableValue.Create(value)));
			return field;
		}

		private VisualElement CreateString()
		{
			if (_definition.Constraint is StringVariableConstraint constraint && constraint.Values.Count > 0)
			{
				var dropdown = new StringDropdown(constraint.Values, constraint.Values, _value.String, _owner, () => _value.String, value => SetValue(VariableValue.Create(value)));
				return dropdown;
			}
			else
			{
				var field = new TextField() { value = _value.String, isDelayed = true };
				ElementHelper.Bind(this, field, _owner, () => _value.String, value => SetValue(VariableValue.Create(value)));
				return field;
			}
		}

		private VisualElement CreateEnum()
		{
			//var buttons = new EnumButtons(_owner, () => _value.Enum, value => SetValue(VariableValue.Create(value)));
			//buttons.Setup(_value.EnumType, false, _value.Enum);
			//return buttons;
			return null;
		}

		private VisualElement CreateObject()
		{
			var objectType = (_definition.Constraint as ObjectVariableConstraint)?.Type ?? _value.ReferenceType ?? typeof(Object);

			var picker = new ObjectPicker(_owner, () => _value.Object, value => SetValue(VariableValue.CreateReference(value)));
			picker.Setup(objectType, _value.Object);
			return picker;
		}

		private VisualElement CreateStore()
		{
			var container = new VisualElement();
			//
			//var names = _value.Store.GetVariableNames();
			//var remove = string.Empty;
			//
			//var storeConstraint = _definition.Constraint as StoreVariableConstraint;
			//
			//foreach (var name in names)
			//{			
			//	var index = storeConstraint?.Schema != null ? storeConstraint.Schema.GetIndex(name) : -1;
			//	var definition = index >= 0 ? storeConstraint.Schema[index].Definition : ValueDefinition.Empty);
			//
			//	var item = _value.Store.GetVariable(name);
			//	
			//	container.Add(new Label(name));
			//
			//	if (storeConstraint?.Schema == null && _value.Store is VariableStore store)
			//	{
			//		container.Add(ElementHelper.CreateIconButton(Icon.Remove.Content, "Remove this value from the variable store", () =>
			//		{
			//
			//		}));
			//	}
			//
			//	item = Draw(itemRect, GUIContent.none, item, definition, true);
			//	value.Store.SetVariable(name, item);
			//}
			//
			//if (constraint?.Schema == null && value.Store is VariableStore store)
			//{
			//	var addRect = RectHelper.TakeTrailingIcon(ref rect);
			//
			//	if (GUI.Button(addRect, _addStoreButton.Content, GUIStyle.none))
			//	{
			//		AddToStorePopup.Store = store;
			//		AddToStorePopup.Name = "";
			//		PopupWindow.Show(addRect, AddToStorePopup.Instance);
			//	}
			//
			//	if (!string.IsNullOrEmpty(remove))
			//		(value.Store as VariableStore).RemoveVariable(remove);
			//}
			//
			return container;
		}

		//private class AddToStorePopup : PopupWindowContent
		//{
		//	public static AddToStorePopup Instance = new AddToStorePopup();
		//	public static VariableStore Store;
		//	public static string Name;
		//
		//	public override Vector2 GetWindowSize()
		//	{
		//		var size = base.GetWindowSize();
		//
		//		size.y = 10.0f; // padding
		//		size.y += RectHelper.LineHeight; // name
		//		size.y += RectHelper.LineHeight; // button
		//
		//		return size;
		//	}
		//
		//	public override void OnGUI(Rect rect)
		//	{
		//		rect = RectHelper.Inset(rect, 5.0f);
		//		var nameRect = RectHelper.TakeLine(ref rect);
		//		var buttonRect = RectHelper.TakeTrailingHeight(ref rect, EditorGUIUtility.singleLineHeight);
		//		RectHelper.TakeTrailingHeight(ref rect, RectHelper.VerticalSpace);
		//
		//		Name = EditorGUI.TextField(nameRect, Name);
		//		var valid = !string.IsNullOrEmpty(Name) && !Store.Map.ContainsKey(Name);
		//
		//		using (new EditorGUI.DisabledScope(!valid))
		//		{
		//			if (GUI.Button(buttonRect, "Add"))
		//			{
		//				Store.AddVariable(Name, VariableValue.Empty);
		//				editorWindow.Close();
		//			}
		//		}
		//	}
		//}

		private VisualElement CreateList()
		{
			var container = new VisualElement();

			//var itemDefinition = constraint != null
			//	? ValueDefinition.Create(constraint.ItemType, constraint.ItemConstraint)
			//	: ValueDefinition.Create(VariableType.Empty);
			//
			//var remove = -1;
			//
			//for (var i = 0; i < value.List.Count; i++)
			//{
			//	if (i != 0)
			//		RectHelper.TakeVerticalSpace(ref rect);
			//
			//	var item = value.List.GetVariable(i);
			//	var height = GetHeight(item, itemDefinition, drawStores);
			//	var itemRect = RectHelper.TakeHeight(ref rect, height);
			//	var removeRect = RectHelper.TakeTrailingIcon(ref itemRect);
			//
			//	item = Draw(itemRect, GUIContent.none, item, itemDefinition);
			//	value.List.SetVariable(i, item);
			//
			//	if (GUI.Button(removeRect, _removeListButton.Content, GUIStyle.none))
			//		remove = i;
			//}
			//
			//var addRect = RectHelper.TakeTrailingIcon(ref rect);
			//
			//if (GUI.Button(addRect, _addListButton.Content, GUIStyle.none))
			//	value.List.AddVariable(itemDefinition.Generate(null));
			//
			//if (remove >= 0)
			//	value.List.RemoveVariable(remove);

			return container;
		}
	}
}
