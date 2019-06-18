using PiRhoSoft.PargonUtilities.Engine;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.PargonUtilities.Editor
{
	[CustomPropertyDrawer(typeof(ScenePickerAttribute))]
	public class ScenePickerDrawer : PropertyDrawer
	{
		private const string _invalidTypeWarning = "(PISPDIT) Invalid type for ScenePickerAttribute on field {0}: ScenePicker can only be applied to strings, ints, or SceneReferences";

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var container = ElementHelper.CreatePropertyContainer(property.displayName);

			if (property.propertyType == SerializedPropertyType.Integer || property.propertyType == SerializedPropertyType.String || fieldInfo.FieldType == typeof(SceneReference))
			{
				var picker = new ScenePickerButton();

				if (attribute is ScenePickerAttribute pickerAttribute)
				{
					var method = fieldInfo.DeclaringType.GetMethod(pickerAttribute.CreateMethod, BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
					void onCreate() => method?.Invoke(method.IsStatic ? null : property.serializedObject.targetObject, null);

					picker.Setup(property, onCreate);
				}
				else
				{
					picker.Setup(property, null);
				}

				container.Add(picker);
			}
			else
			{
				Debug.LogWarningFormat(_invalidTypeWarning, property.propertyPath);
			}

			return container;
		}
	}
}
