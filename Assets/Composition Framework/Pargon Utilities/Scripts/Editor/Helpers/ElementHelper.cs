﻿using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace PiRhoSoft.PargonUtilities.Editor
{
	public interface IBindableObject<T>
	{
		T GetValueFromElement(VisualElement element);
		T GetValueFromObject(Object owner);

		void UpdateElement(T value, VisualElement element, Object owner);
		void UpdateObject(T value, VisualElement element, Object owner);
	}

	public interface IBindableProperty<T>
	{
		T GetValueFromElement(VisualElement element);
		T GetValueFromProperty(SerializedProperty property);

		void UpdateElement(T value, VisualElement element, SerializedProperty property);
		void UpdateProperty(T value, VisualElement element, SerializedProperty property);
	}

	public static class ElementHelper
	{
		#region UXML and USS

		private const string _missingUxmlError = "(PUEHMX) failed to load uxml for {0}: asset '{1}' could not be found";
		private const string _missingUssError = "(PUEHMU) failed to load uss for {0}: asset '{1}' could not be found";

		public static void AddVisualTree<ElementType>(ElementType root, string path) where ElementType : VisualElement
		{
			var uxml = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(path);

			if (uxml != null)
				uxml.CloneTree(root);
			else
				Debug.LogErrorFormat(_missingUxmlError, typeof(ElementType).Name, path);
		}

		public static void AddStyleSheet<ElementType>(ElementType root, string path) where ElementType : VisualElement
		{
			var uss = AssetDatabase.LoadAssetAtPath<StyleSheet>(path);

			if (uss != null)
				root.styleSheets.Add(uss);
			else
				Debug.LogErrorFormat(_missingUssError, typeof(ElementType).Name, path);
		}

		#endregion

		#region Style Helpers

		public static void SetVisible(this VisualElement element, bool visible)
		{
			element.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
		}

		#endregion

		#region BaseField Extensions

		private const string _visualInputProperty = "visualInput";

		public static readonly string BaseFieldUssClassName = BaseField<int>.ussClassName;

		public static VisualElement GetVisualInput<T>(this BaseField<T> field)
		{
			return GetVisualInputProperty<T>().GetValue(field) as VisualElement;
		}

		public static void SetVisualInput<T>(this BaseField<T> field, VisualElement element)
		{
			GetVisualInputProperty<T>().SetValue(field, element);
		}

		public static void SetBaseFieldLabel(VisualElement baseField, string label)
		{
			// BaseField is generic without a non-generic base even though label is not dependent on the generic type
			// so it has to be accessed through reflection. This is a public api, though, so it shouldn't change.

			baseField.GetType().GetProperty("label")?.SetValue(baseField, label);
		}

		private static PropertyInfo GetVisualInputProperty<T>()
		{
			return typeof(BaseField<T>).GetProperty(_visualInputProperty, BindingFlags.Instance | BindingFlags.NonPublic);
		}

		#endregion

		#region Event Helpers

		private const string _serializedPropertyBindEventChangedError = "(PUEEHSPBEC) binding event internals have changed - custom editor bindings may not function correctly";
		private const string _serializedPropertyBindEventTypeName = "UnityEditor.UIElements.SerializedPropertyBindEvent, UnityEditor";
		private const string _serializedPropertyBindEventPropertyName = "bindProperty";

		private static Type _serializedPropertyBindEventType = null;
		private static PropertyInfo _serializedPropertyBindEventProperty = null;

		public static Type SerializedPropertyBindEventType
		{
			get
			{
				if (_serializedPropertyBindEventType == null)
					_serializedPropertyBindEventType = Type.GetType(_serializedPropertyBindEventTypeName);

				if (_serializedPropertyBindEventType == null)
					Debug.LogWarning(_serializedPropertyBindEventChangedError);

				return _serializedPropertyBindEventType;
			}
		}

		public static PropertyInfo SerializedPropertyBindEventProperty
		{
			get
			{
				if (_serializedPropertyBindEventProperty == null)
					_serializedPropertyBindEventProperty = SerializedPropertyBindEventType?.GetProperty(_serializedPropertyBindEventPropertyName, typeof(SerializedProperty));

				if (_serializedPropertyBindEventType != null && _serializedPropertyBindEventProperty == null)
					Debug.LogWarning(_serializedPropertyBindEventChangedError);

				return _serializedPropertyBindEventProperty;
			}
		}

		public static bool IsSerializedPropertyBindEvent(EventBase evt, out SerializedProperty property)
		{
			property = evt.GetType() == SerializedPropertyBindEventType
				? SerializedPropertyBindEventProperty?.GetValue(evt) as SerializedProperty
				: null;

			return property != null;
		}

		#endregion

		#region Element Helpers

		public static VisualElement SetupPropertyField<T>(BaseField<T> field, FieldInfo fieldInfo)
		{
			// TODO: look for stretch attribute and set up accordingly

			field.labelElement.tooltip = GetTooltip(fieldInfo);
			field.labelElement.AddToClassList(PropertyField.labelUssClassName);
			field.GetVisualInput().AddToClassList(PropertyField.inputUssClassName);

			return field;
		}

		public static VisualElement CreateEmptyPropertyField(string label)
		{
			var container = new VisualElement();
			var labelElement = new Label(label);

			labelElement.AddToClassList(PropertyField.labelUssClassName);
			container.AddToClassList(BaseFieldUssClassName);
			container.Add(labelElement);

			return container;
		}

		public static VisualElement CreatePropertyContainer(string label = null, string tooltip = null)
		{
			var container = new VisualElement();
			container.AddToClassList(BaseField<string>.ussClassName);

			if (!string.IsNullOrEmpty(label))
				container.Add(CreatePropertyLabel(label, tooltip));

			return container;
		}

		public static VisualElement CreatePropertyLabel(string text, string tooltip)
		{
			var label = new Label(text) { tooltip = tooltip };
			label.AddToClassList(PropertyField.labelUssClassName);
			label.AddToClassList(BaseField<string>.labelUssClassName); // This can be any generic type string just made sense as a default

			return label;
		}

		public static Image CreateIconButton(Texture image, string tooltip, Action action)
		{
			var button = new Image { image = image, tooltip = tooltip };
			button.AddManipulator(new Clickable(action));
			button.style.minWidth = new StyleLength(image.width);
			button.style.minHeight = new StyleLength(image.height);
			button.style.maxWidth = new StyleLength(image.width);
			button.style.maxHeight = new StyleLength(image.height);
			button.style.alignSelf = new StyleEnum<Align>(Align.Center);
			return button;
		}

		public static VisualElement GetPropertyContainer(SerializedProperty property, string label, FieldInfo fieldInfo, PropertyAttribute attribute)
		{
			var drawer = PropertyHelper.GetNextDrawer(fieldInfo, attribute);
			return drawer == null ? new BindablePropertyElement(property, label, GetTooltip(fieldInfo)) : drawer.CreatePropertyGUI(property) ?? CreateFallbackContainer(property);
		}

		public static VisualElement CreateFallbackContainer(SerializedProperty property)
		{
			return new IMGUIContainer(() =>
			{
				// this is a copy of the implementation from PropertyField.Reset without the wide mode handling since
				// that is internal and seemingly unimportant

				EditorGUI.BeginChangeCheck();
				property.serializedObject.Update();

				EditorGUILayout.PropertyField(property, true);

				property.serializedObject.ApplyModifiedProperties();
				EditorGUI.EndChangeCheck();
			});
		}

		public static VisualElement CreateDefaultElement(SerializedProperty property)
		{
			switch (property.propertyType)
			{
				case SerializedPropertyType.Integer: return new IntegerField();
				case SerializedPropertyType.Boolean: return new Toggle();
				case SerializedPropertyType.Float: return new FloatField();
				case SerializedPropertyType.String: return new TextField();
				case SerializedPropertyType.Color: return new ColorField();
				case SerializedPropertyType.Enum: return new PopupField<string>(property.enumDisplayNames.ToList(), property.enumValueIndex) { index = property.enumValueIndex };
				case SerializedPropertyType.ObjectReference: return new ObjectField();
				case SerializedPropertyType.Vector2: return new Vector2Field();
				case SerializedPropertyType.Vector3: return new Vector3Field();
				case SerializedPropertyType.Vector4: return new Vector4Field();
				case SerializedPropertyType.Quaternion: return new EulerField(property, property.displayName);
				case SerializedPropertyType.Rect: return new RectField();
				case SerializedPropertyType.Bounds: return new BoundsField();
				case SerializedPropertyType.Vector2Int: return new Vector2IntField();
				case SerializedPropertyType.Vector3Int: return new Vector3IntField();
				case SerializedPropertyType.RectInt: return new RectIntField();
				case SerializedPropertyType.BoundsInt: return new BoundsIntField();
				default: return null;
			}
		}

		public static void SetPropertyToElementValue(SerializedProperty property, VisualElement element)
		{
			switch (element)
			{
				case IntegerField field: property.intValue = field.value; break;
				case Toggle field: property.boolValue = field.value; break;
				case FloatField field: property.floatValue = field.value; break;
				case TextField field: property.stringValue = field.value; break;
				case ColorField field: property.colorValue = field.value; break;
				case PopupField<string> field: property.enumValueIndex = field.index; break;
				case ObjectField field: property.objectReferenceValue = field.value;  break;
				case Vector2Field field: property.vector2Value = field.value; break;
				case Vector3Field field: property.vector3Value = field.value; break;
				case Vector4Field field: property.vector4Value = field.value; break;
				case EulerField field: property.quaternionValue = field.value; break;
				case RectField field: property.rectValue = field.value; break;
				case BoundsField field: property.boundsValue = field.value; break;
				case Vector2IntField field: property.vector2IntValue = field.value; break;
				case Vector3IntField field: property.vector3IntValue = field.value; break;
				case RectIntField field: property.rectIntValue = field.value; break;
				case BoundsIntField field: property.boundsIntValue = field.value; break;
			}
		}

		#endregion

		#region Event Helpers

		public static void SendChangeEvent<T>(VisualElement element, T previous, T current)
		{
			using (var changeEvent = ChangeEvent<T>.GetPooled(previous, current))
			{
				changeEvent.target = element;
				element.SendEvent(changeEvent);
			}
		}

		public static bool RegisterChangeEvent(VisualElement element, Action action)
		{
			// TODO: Somehow make this respond to all element types

			if (element is BindablePropertyElement bindable)
				element = bindable.Element;

			switch (element)
			{
				case INotifyValueChanged<int> field: field.RegisterValueChangedCallback(e => action()); return true;
				case INotifyValueChanged<bool> field: field.RegisterValueChangedCallback(e => action()); return true;
				case INotifyValueChanged<float> field: field.RegisterValueChangedCallback(e => action()); return true;
				case INotifyValueChanged<string> field: field.RegisterValueChangedCallback(e => action()); return true;
				case INotifyValueChanged<Color> field: field.RegisterValueChangedCallback(e => action()); return true;
				case INotifyValueChanged<Object> field: field.RegisterValueChangedCallback(e => action()); return true;
				case INotifyValueChanged<Vector2> field: field.RegisterValueChangedCallback(e => action()); return true;
				case INotifyValueChanged<Vector3> field: field.RegisterValueChangedCallback(e => action()); return true;
				case INotifyValueChanged<Vector4> field: field.RegisterValueChangedCallback(e => action()); return true;
				case INotifyValueChanged<Rect> field: field.RegisterValueChangedCallback(e => action()); return true;
				case INotifyValueChanged<Bounds> field: field.RegisterValueChangedCallback(e => action()); return true;
				case INotifyValueChanged<Vector2Int> field: field.RegisterValueChangedCallback(e => action()); return true;
				case INotifyValueChanged<Vector3Int> field: field.RegisterValueChangedCallback(e => action()); return true;
				case INotifyValueChanged<RectInt> field: field.RegisterValueChangedCallback(e => action()); return true;
				case INotifyValueChanged<BoundsInt> field: field.RegisterValueChangedCallback(e => action()); return true;
				//case EnumDropdown field: field.RegisterCallback<ChangeEvent<int>>(e => action()); return true;
				default: return false;
			}
		}

		#endregion

		#region Tooltips

		private const string _missingPropertyWarning = "(PUEHMP) unable to find property '{0}' on type '{1}' for Tooltip";

		public static string GetTooltip(Type type, string propertyName)
		{
			var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
			var field = type.GetField(propertyName, flags);

			if (field == null)
			{
				Debug.LogWarningFormat(_missingPropertyWarning, propertyName, type.Name);
				return string.Empty;
			}
			else
			{
				return GetTooltip(field);
			}
		}

		public static string GetTooltip(FieldInfo field)
		{
			return field?.GetCustomAttribute<TooltipAttribute>()?.tooltip ?? string.Empty;
		}

		#endregion

		#region Bindings

		private const long _bindingUpdateRate = 100;

		public static void Bind<T>(VisualElement element, IBindableProperty<T> bindable, SerializedProperty property)
		{
			element.RegisterCallback<ChangeEvent<T>>(evt =>
			{
				using (new ChangeScope(property.serializedObject))
					bindable.UpdateProperty(evt.newValue, element, property);
			});

			element.schedule.Execute(() =>
			{
				var fromElement = bindable.GetValueFromElement(element);
				var fromProperty = bindable.GetValueFromProperty(property);

				if ((fromElement == null && fromProperty != null) || (fromElement != null && !fromElement.Equals(fromProperty)))
					bindable.UpdateElement(fromProperty, element, property);

			}).Every(_bindingUpdateRate);
		}

		public static void Bind<T>(VisualElement element, IBindableObject<T> bindable, Object owner)
		{
			element.RegisterCallback<ChangeEvent<T>>(evt =>
			{
				using (new ChangeScope(owner))
					bindable.UpdateObject(evt.newValue, element, owner);
			});

			element.schedule.Execute(() =>
			{
				var fromElement = bindable.GetValueFromElement(element);
				var fromObject = bindable.GetValueFromObject(owner);

				if ((fromElement == null && fromObject != null) || (fromElement != null && !fromElement.Equals(fromObject)))
					bindable.UpdateElement(fromObject, element, owner);

			}).Every(_bindingUpdateRate);
		}

		public static void Bind<T>(VisualElement element, INotifyValueChanged<T> field, Object owner, Func<T> getValue, Action<T> setValue)
		{
			field.RegisterValueChangedCallback(evt =>
			{
				using (new ChangeScope(owner))
					setValue(field.value);
			});

			element.schedule.Execute(() =>
			{
				var value = getValue();
				if ((value == null && field.value != null) || (value != null && !value.Equals(field.value)))
					field.SetValueWithoutNotify(value);

			}).Every(_bindingUpdateRate);
		}

		public class BindablePropertyElement : BindableElement
		{
			public VisualElement Element { get; private set; }

			public BindablePropertyElement(SerializedProperty property, string label, string tooltip)
			{
				AddToClassList(BaseField<string>.ussClassName);

				if (!string.IsNullOrEmpty(label))
					Add(CreatePropertyLabel(label, tooltip));

				Element = CreateDefaultElement(property);

				switch (Element)
				{
					case IBindable field: field.BindProperty(property); break;
				}

				Add(Element);
			}
		}

		#endregion
	}

	public class SimpleBinding<T> : BindableElement, INotifyValueChanged<T>
	{
		private T _value;
		private Action<T> _onChanged;

		public T value
		{
			get => _value;
			set
			{
				SetValueWithoutNotify(value);
				_onChanged(value);
			}
		}

		public SimpleBinding(T value, Action<T> onChanged)
		{
			_value = value;
			_onChanged = onChanged;

			style.display = DisplayStyle.None;
		}

		public void Reset(SerializedProperty property)
		{
			if (this.IsBound())
				binding.Release();

			this.BindProperty(property);
		}

		public void SetValueWithoutNotify(T newValue)
		{
			_value = newValue;
		}
	}
}
