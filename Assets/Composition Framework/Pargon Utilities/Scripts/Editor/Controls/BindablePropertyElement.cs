using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace PiRhoSoft.PargonUtilities.Editor
{
	public class BindablePropertyElement : BindableElement
	{
		private const int _scheduleTime = 100;

		public VisualElement Element { get; private set; }

		public BindablePropertyElement(SerializedProperty property, string label)
		{
			Element = ElementHelper.CreateDefaultElement(property, label);

			switch (Element)
			{
				case IntegerField field: field.RegisterValueChangedCallback(e => { property.intValue = e.newValue; property.serializedObject.ApplyModifiedProperties(); }); break;
				case Toggle field: field.RegisterValueChangedCallback(e => { property.boolValue = e.newValue; property.serializedObject.ApplyModifiedProperties(); }); break;
				case FloatField field: field.RegisterValueChangedCallback(e => { property.floatValue = e.newValue; property.serializedObject.ApplyModifiedProperties(); }); break;
				case TextField field: field.RegisterValueChangedCallback(e => { property.stringValue = e.newValue; property.serializedObject.ApplyModifiedProperties(); }); break;
				case ColorField field: field.RegisterValueChangedCallback(e => { property.colorValue = e.newValue; property.serializedObject.ApplyModifiedProperties(); }); break;
				case ObjectField field: field.RegisterValueChangedCallback(e => { property.objectReferenceValue = e.newValue; property.serializedObject.ApplyModifiedProperties(); }); break;
				case Vector2Field field: field.RegisterValueChangedCallback(e => { property.vector2Value = e.newValue; property.serializedObject.ApplyModifiedProperties(); }); break;
				case Vector3Field field: field.RegisterValueChangedCallback(e => { property.vector3Value = e.newValue; property.serializedObject.ApplyModifiedProperties(); }); break;
				case Vector4Field field: field.RegisterValueChangedCallback(e => { property.vector4Value = e.newValue; property.serializedObject.ApplyModifiedProperties(); }); break;
				case RectField field: field.RegisterValueChangedCallback(e => { property.rectValue = e.newValue; property.serializedObject.ApplyModifiedProperties(); }); break;
				case BoundsField field: field.RegisterValueChangedCallback(e => { property.boundsValue = e.newValue; property.serializedObject.ApplyModifiedProperties(); }); break;
				case Vector2IntField field: field.RegisterValueChangedCallback(e => { property.vector2IntValue = e.newValue; property.serializedObject.ApplyModifiedProperties(); }); break;
				case Vector3IntField field: field.RegisterValueChangedCallback(e => { property.vector3IntValue = e.newValue; property.serializedObject.ApplyModifiedProperties(); }); break;
				case RectIntField field: field.RegisterValueChangedCallback(e => { property.rectIntValue = e.newValue; property.serializedObject.ApplyModifiedProperties(); }); break;
				case BoundsIntField field: field.RegisterValueChangedCallback(e => { property.boundsIntValue = e.newValue; property.serializedObject.ApplyModifiedProperties(); }); break;
			}

			schedule.Execute(() =>
			{
				switch (Element)
				{
					case IntegerField field: field.SetValueWithoutNotify(property.intValue); break;
					case Toggle field: field.SetValueWithoutNotify(property.boolValue); break;
					case FloatField field: field.SetValueWithoutNotify(property.floatValue); break;
					case TextField field: field.SetValueWithoutNotify(property.stringValue); break;
					case ColorField field: field.SetValueWithoutNotify(property.colorValue); break;
					case ObjectField field: field.SetValueWithoutNotify(property.objectReferenceValue); break;
					case Vector2Field field: field.SetValueWithoutNotify(property.vector2Value); break;
					case Vector3Field field: field.SetValueWithoutNotify(property.vector3Value); break;
					case Vector4Field field: field.SetValueWithoutNotify(property.vector4Value); break;
					case RectField field: field.SetValueWithoutNotify(property.rectValue); break;
					case BoundsField field: field.SetValueWithoutNotify(property.boundsValue); break;
					case Vector2IntField field: field.SetValueWithoutNotify(property.vector2IntValue); break;
					case Vector3IntField field: field.SetValueWithoutNotify(property.vector3IntValue); break;
					case RectIntField field: field.SetValueWithoutNotify(property.rectIntValue); break;
					case BoundsIntField field: field.SetValueWithoutNotify(property.boundsIntValue); break;
				}
			}).Every(_scheduleTime);

			Add(Element);
		}
	}
}
