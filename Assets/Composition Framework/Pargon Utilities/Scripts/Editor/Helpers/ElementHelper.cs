using System;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace PiRhoSoft.PargonUtilities.Editor
{
	public static class ElementHelper
	{
		private const string _missingUxmlError = "(EHMX) failed to load uxml for {0}: asset '{1}' could not be found";
		private const string _missingUssError = "(EHMU) failed to load uss for {0}: asset '{1}' could not be found";

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

		public static VisualElement CreatePropertyContainer(string label)
		{
			var container = new VisualElement();
			container.AddToClassList(BaseField<string>.ussClassName);

			if (!string.IsNullOrEmpty(label))
				container.Add(CreatePropertyLabel(label));

			return container;
		}

		public static VisualElement CreatePropertyLabel(string text)
		{
			var label = new Label(text);
			label.AddToClassList(BaseField<string>.labelUssClassName);
			label.AddToClassList(PropertyField.labelUssClassName);

			return label;
		}

		public static VisualElement GetPropertyContainer(SerializedProperty property, string label, FieldInfo fieldInfo, PropertyAttribute attribute)
		{
			var drawer = PropertyHelper.GetNextDrawer(fieldInfo, attribute);
			return drawer == null ? new BindablePropertyElement(property, label) : drawer.CreatePropertyGUI(property) ?? CreateFallbackContainer(property);
		}

		public static VisualElement CreateDefaultElement(SerializedProperty property, string label)
		{
			switch (property.propertyType)
			{
				case SerializedPropertyType.Integer: return new IntegerField(label);
				case SerializedPropertyType.Boolean: return new Toggle(label);
				case SerializedPropertyType.Float: return new FloatField(label);
				case SerializedPropertyType.String: return new TextField(label);
				case SerializedPropertyType.Color: return new ColorField(label);
				case SerializedPropertyType.ObjectReference: return new ObjectField(label);
				case SerializedPropertyType.Vector2: return new Vector2Field(label);
				case SerializedPropertyType.Vector3: return new Vector3Field(label);
				case SerializedPropertyType.Vector4: return new Vector4Field(label);
				case SerializedPropertyType.Rect: return new RectField(label);
				case SerializedPropertyType.Bounds: return new BoundsField(label);
				case SerializedPropertyType.Vector2Int: return new Vector2IntField(label);
				case SerializedPropertyType.Vector3Int: return new Vector3IntField(label);
				case SerializedPropertyType.RectInt: return new RectIntField(label);
				case SerializedPropertyType.BoundsInt: return new BoundsIntField(label);
				default: return null;
			}
		}

		public static bool RegisterChangeEvent(VisualElement element, Action action)
		{
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
				default: return false;
			}
		}

		public static void ToggleClass(VisualElement element, string className, bool isValid)
		{
			if (isValid && !element.ClassListContains(className))
				element.AddToClassList(className);
			else if (!isValid && element.ClassListContains(className))
				element.RemoveFromClassList(className);
		}

		public static void AlternateClass(VisualElement element, string validClass, string invalidClass, bool isValid)
		{
			if (isValid)
			{
				if (!element.ClassListContains(validClass))
					element.AddToClassList(validClass);

				if (element.ClassListContains(invalidClass))
					element.RemoveFromClassList(invalidClass);
			}
			else
			{
				if (!element.ClassListContains(invalidClass))
					element.AddToClassList(invalidClass);

				if (element.ClassListContains(validClass))
					element.RemoveFromClassList(validClass);
			}
		}

		public static void SetVisible(VisualElement element, bool visible)
		{
			element.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
		}

		public static VisualElement CreateFallbackContainer(SerializedProperty property)
		{
			return new IMGUIContainer(() =>
			{
				EditorGUILayout.PropertyField(property);
			});
		}
	}
}
