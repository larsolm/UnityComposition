using UnityEditor;

namespace PiRhoSoft.Utilities.Editor
{
	public static class SerializedPropertyExtensions
	{
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

		#region Arrays

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