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
			var container = ElementHelper.CreatePropertyContainer(property.displayName, ElementHelper.GetTooltip(fieldInfo));

			if (property.propertyType == SerializedPropertyType.Integer || property.propertyType == SerializedPropertyType.String || fieldInfo.FieldType == typeof(SceneReference))
			{
				var picker = new ScenePicker(property);
				var scenePicker = attribute as ScenePickerAttribute;
				
				var method = fieldInfo.DeclaringType.GetMethod(scenePicker.CreateMethod, BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				void onCreate() => method?.Invoke(method.IsStatic ? null : property.serializedObject.targetObject, null);

				if (property.propertyType == SerializedPropertyType.String)
					picker.Setup(property.stringValue, onCreate);
				else if (property.propertyType == SerializedPropertyType.Integer)
					picker.Setup(property.intValue, onCreate);
				else // SceneReference
					picker.Setup(property.FindPropertyRelative(nameof(SceneReference.Path)).stringValue, onCreate);

				container.Add(picker);

				//if (DragAndDrop.objectReferences.Length > 0 && rect.Contains(Event.current.mousePosition))
				//{
				//	var obj = DragAndDrop.objectReferences[0];
				//
				//	if (obj is SceneAsset asset)
				//	{
				//		if (Event.current.type == EventType.DragUpdated)
				//		{
				//			DragAndDrop.visualMode = DragAndDropVisualMode.Link;
				//			Event.current.Use();
				//		}
				//
				//		if (Event.current.type == EventType.DragPerform)
				//		{
				//			scene.Path = AssetDatabase.GetAssetPath(asset);
				//			DragAndDrop.AcceptDrag();
				//		}
				//	}
				//}
			}
			else
			{
				Debug.LogWarningFormat(_invalidTypeWarning, property.propertyPath);
			}

			return container;
		}
	}
}
