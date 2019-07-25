using PiRhoSoft.Utilities.Editor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace PiRhoSoft.Composition.Editor
{
	public class VariableValueControl : VisualElement
	{
		public VariableValue Value { get; private set; }
		public ValueDefinition Definition { get; private set; }

		public VariableValueControl(VariableValue value, ValueDefinition definition)
		{
			Value = value;
			Definition = definition;

			Refresh();
		}

		public void SetValueWithoutNotify(VariableValue value)
		{
			Value = value;
			Refresh();
		}

		public void SetValue(VariableValue value)
		{
			var previous = Value;
			SetValueWithoutNotify(value);
			this.SendChangeEvent(previous, Value);
		}

		public void SetDefinition(ValueDefinition definition, VariableValue value)
		{
			Definition = definition;
			SetValueWithoutNotify(value);
		}

		private void Refresh()
		{
			Clear();
			Add(CreateElement());
		}

		private void RegisterChange<T>(BaseField<T> field)
		{
			field.RegisterValueChangedCallback(evt =>
			{
				var value = VariableValue.CreateValue(evt.newValue);
				SetValue(value);
			});
		}

		private VisualElement CreateElement()
		{
			switch (Value.Type)
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
			var dropdown = new EnumField(VariableType.Empty);
			dropdown.RegisterValueChangedCallback(evt =>
			{
				var type = (VariableType)evt.newValue;
				var constraint = VariableConstraint.Create(type);
				var value = VariableHandler.CreateDefault(type, constraint);

				Definition = ValueDefinition.Create(type, constraint, null, null, false, false);
				SetValue(value);
			});

			return dropdown;
		}

		public VisualElement CreateBool()
		{
			var toggle = new Toggle() { value = Value.Bool };
			toggle.RegisterValueChangedCallback(evt =>
			{
				var value = VariableValue.Create(evt.newValue);
				SetValue(value);
			});

			return toggle;
		}

		public VisualElement CreateInt()
		{
			var container = new VisualElement();
			container.style.flexDirection = FlexDirection.Row;

			if (Definition.Constraint is IntVariableConstraint constraint)
			{
				var field = new IntegerField() { value = Value.Int, isDelayed = true };
				field.RegisterValueChangedCallback(evt =>
				{
					var clamped = constraint.HasRange ? Mathf.Clamp(evt.newValue, constraint.Minimum, constraint.Maximum) : evt.newValue;
					var value = VariableValue.Create(clamped);

					SetValue(value);
				});

				container.Add(field);

				if (constraint.HasRange)
				{
					var slider = new SliderInt(constraint.Minimum, constraint.Maximum) { value = Value.Int };
					slider.RegisterValueChangedCallback(evt =>
					{
						var value = VariableValue.Create(evt.newValue);
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

			if (Definition.Constraint is FloatVariableConstraint constraint)
			{
				var field = new FloatField() { value = Value.Float, isDelayed = true };
				field.RegisterValueChangedCallback(evt =>
				{
					var clamped = constraint.HasRange ? Mathf.Clamp(evt.newValue, constraint.Minimum, constraint.Maximum) : evt.newValue;
					var value = VariableValue.Create(clamped);

					SetValue(value);
				});

				container.Add(field);

				if (constraint.HasRange)
				{
					var slider = new Slider(constraint.Minimum, constraint.Maximum) { value = Value.Float };
					RegisterChange(slider);
					container.Add(slider);
				}
			}

			return container;
		}

		private VisualElement CreateInt2()
		{
			var field = new Vector2IntField() { value = Value.Int2 };
			RegisterChange(field);
			return field;
		}

		private VisualElement CreateInt3()
		{
			var field = new Vector3IntField() { value = Value.Int3 };
			RegisterChange(field);
			return field;
		}

		private VisualElement CreateIntRect()
		{
			var field = new RectIntField() { value = Value.IntRect };
			RegisterChange(field);
			return field;
		}

		private VisualElement CreateIntBounds()
		{
			var field = new BoundsIntField() { value = Value.IntBounds };
			RegisterChange(field);
			return field;
		}

		private VisualElement CreateVector2()
		{
			var field = new Vector2Field() { value = Value.Vector2 };
			RegisterChange(field);
			return field;
		}

		private VisualElement CreateVector3()
		{
			var field = new Vector3Field() { value = Value.Vector3 };
			RegisterChange(field);
			return field;
		}

		private VisualElement CreateVector4()
		{
			var field = new Vector4Field() { value = Value.Vector4 };
			RegisterChange(field);
			return field;
		}

		private VisualElement CreateQuaternion()
		{
			var field = new EulerControl(Value.Quaternion);
			field.RegisterCallback<ChangeEvent<Quaternion>>(evt =>
			{
				var value = VariableValue.Create(evt.newValue);
				SetValue(value);
			});

			return field;
		}

		private VisualElement CreateRect()
		{
			var field = new RectField() { value = Value.Rect };
			RegisterChange(field);
			return field;
		}

		private VisualElement CreateBounds()
		{
			var field = new BoundsField() { value = Value.Bounds };
			RegisterChange(field);
			return field;
		}

		private VisualElement CreateColor()
		{
			var field = new ColorField() { value = Value.Color };
			RegisterChange(field);
			return field;
		}

		private VisualElement CreateString()
		{
			if (Definition.Constraint is StringVariableConstraint constraint && constraint.Values.Count > 0)
			{
				var popup = new PopupField<string>(constraint.Values, Value.String);
				RegisterChange(popup);
				return popup;
			}
			else
			{
				var field = new TextField() { value = Value.String, isDelayed = true };
				RegisterChange(field);
				return field;
			}
		}

		private VisualElement CreateEnum()
		{
			var field = new EnumField(Value.Enum);
			RegisterChange(field);
			return field;
		}

		private VisualElement CreateObject()
		{
			var objectType = (Definition.Constraint as ObjectVariableConstraint)?.Type ?? Value.ReferenceType ?? typeof(Object);

			var picker = new ObjectPickerControl(Value.Object, objectType);
			picker.RegisterCallback<ChangeEvent<Object>>(evt =>
			{
				var value = VariableValue.Create(evt.newValue);
				SetValue(value);
			});

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
