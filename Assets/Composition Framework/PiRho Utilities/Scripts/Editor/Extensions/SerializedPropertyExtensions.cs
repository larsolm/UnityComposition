using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	public static class SerializedPropertyExtensions
	{
		#region Internal Lookups

		private const string _changedInternalsError = "(PUPFECI) failed to setup SerializedProperty: Unity internals have changed";

		private const string _createFieldFromPropertyName = "CreateFieldFromProperty";
		private static MethodInfo _createFieldFromPropertyMethod;
		private static object[] _createFieldFromPropertyParameters = new object[1];
		private static PropertyField _createFieldFromPropertyInstance;

		static SerializedPropertyExtensions()
		{
			var createFieldFromPropertyMethod = typeof(PropertyField).GetMethod(_createFieldFromPropertyName, BindingFlags.Instance | BindingFlags.NonPublic);
			var createFieldFromPropertyParameters = createFieldFromPropertyMethod?.GetParameters();

			if (createFieldFromPropertyMethod != null && createFieldFromPropertyMethod.HasSignature(typeof(VisualElement), typeof(SerializedProperty)))
			{
				_createFieldFromPropertyMethod = createFieldFromPropertyMethod;
				_createFieldFromPropertyInstance = new PropertyField();
			}

			if (_createFieldFromPropertyMethod == null)
				Debug.LogError(_changedInternalsError);
		}

		#endregion

		#region Extension Methods

		public static VisualElement CreateField(this SerializedProperty property)
		{
			// TODO: two situations where this doesn't work correctly
			//  - for enums with a binding there seems to be an internal bug (at least in 2019.2)
			//  - for arrays updates depend on state of the PropertyField

			_createFieldFromPropertyParameters[0] = property;
			return _createFieldFromPropertyMethod?.Invoke(_createFieldFromPropertyInstance, _createFieldFromPropertyParameters) as VisualElement;
		}

		public static void SetToDefault(this SerializedProperty property)
		{
			// TODO: struct

			if (property.isArray)
			{
				property.arraySize = 0;
			}
			else
			{
				switch (property.propertyType)
				{
					case SerializedPropertyType.Generic: break; // TODO: ?
					case SerializedPropertyType.Integer: property.intValue = default; break;
					case SerializedPropertyType.Boolean: property.boolValue = default; break;
					case SerializedPropertyType.Float: property.floatValue = default; break;
					case SerializedPropertyType.String: property.stringValue = string.Empty; break;
					case SerializedPropertyType.Color: property.colorValue = default; break;
					case SerializedPropertyType.ObjectReference: property.objectReferenceValue = default; break;
					case SerializedPropertyType.LayerMask: break; // TODO: ?
					case SerializedPropertyType.Enum: property.enumValueIndex = 0; break;
					case SerializedPropertyType.Vector2: property.vector2Value = default; break;
					case SerializedPropertyType.Vector3: property.vector3Value = default; break;
					case SerializedPropertyType.Vector4: property.vector4Value = default; break;
					case SerializedPropertyType.Rect: property.rectValue = default; break;
					case SerializedPropertyType.ArraySize: break; // TODO: ?
					case SerializedPropertyType.Character: break; // TODO: ?
					case SerializedPropertyType.AnimationCurve: property.animationCurveValue = default; break;
					case SerializedPropertyType.Bounds: property.boundsValue = default; break;
					case SerializedPropertyType.Gradient: break; // TODO: ?
					case SerializedPropertyType.Quaternion: property.quaternionValue = default; break;
					case SerializedPropertyType.ExposedReference: break; // TODO: ?
					case SerializedPropertyType.FixedBufferSize: break; // TODO: ?
					case SerializedPropertyType.Vector2Int: property.vector2IntValue = default; break;
					case SerializedPropertyType.Vector3Int: property.vector3IntValue = default; break;
					case SerializedPropertyType.RectInt: property.rectIntValue = default; break;
					case SerializedPropertyType.BoundsInt: property.boundsIntValue = default; break;
				}
			}
		}

		public static void ResizeArray(this SerializedProperty arrayProperty, int newSize)
		{
			var size = arrayProperty.arraySize;
			arrayProperty.arraySize = newSize;

			// new items will be a copy of the previous last item so this resets them to their default value
			for (var i = size; i < newSize; i++)
				SetToDefault(arrayProperty.GetArrayElementAtIndex(i));

			arrayProperty.serializedObject.ApplyModifiedProperties();
		}

		public static void RemoveFromArray(this SerializedProperty arrayProperty, int index)
		{
			// If an object is removed from an array of ObjectReference, DeleteArrayElementAtIndex will set the item
			// to null instead of removing it. If the entry is already null it will be removed as expected.
			var item = arrayProperty.GetArrayElementAtIndex(index);

			if (item.propertyType == SerializedPropertyType.ObjectReference && item.objectReferenceValue != null)
				item.objectReferenceValue = null;

			arrayProperty.DeleteArrayElementAtIndex(index);
			arrayProperty.serializedObject.ApplyModifiedProperties();
		}

		#endregion
	}
}