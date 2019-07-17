using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	[CustomPropertyDrawer(typeof(ScenePickerAttribute))]
	public class ScenePickerDrawer : PropertyDrawer
	{
		private const string _invalidTypeWarning = "(PUSPDIT) invalid type for ScenePickerAttribute on field {0}: ScenePicker can only be applied to strings or SceneReferences";

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var method = fieldInfo.DeclaringType.GetMethod((attribute as ScenePickerAttribute)?.CreateMethod, BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			void onCreate() => method?.Invoke(method.IsStatic ? null : property.serializedObject.targetObject, null);

			if (property.propertyType == SerializedPropertyType.String)
			{
				var picker = new ScenePickerField(property.displayName, property.stringValue, onCreate);
				return picker.ConfigureProperty(property);
			}
			else if (this.GetFieldType() == typeof(SceneReference))
			{
				var pathProperty = property.FindPropertyRelative(nameof(SceneReference.Path));
				var picker = new ScenePickerField(property.displayName, pathProperty.stringValue, onCreate);
				return picker.ConfigureProperty(pathProperty);
			}
			else
			{
				Debug.LogWarningFormat(_invalidTypeWarning, property.propertyPath);
			}

			return new FieldContainer(property.displayName);
		}
	}
}
