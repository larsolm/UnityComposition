using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

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

		private const string _gradientValueName = "gradientValue";
		private static PropertyInfo _gradientValueProperty;

		private const string _hasVisibleChildFieldsName = "HasVisibleChildFields";
		private static MethodInfo _hasVisibleChildFieldsMethod;
		private static object[] _hasVisibleChildFieldsParameters = new object[1];

		static SerializedPropertyExtensions()
		{
			var createFieldFromPropertyMethod = typeof(PropertyField).GetMethod(_createFieldFromPropertyName, BindingFlags.Instance | BindingFlags.NonPublic);
			var createFieldFromPropertyParameters = createFieldFromPropertyMethod?.GetParameters();

			if (createFieldFromPropertyMethod != null && createFieldFromPropertyMethod.HasSignature(typeof(VisualElement), typeof(SerializedProperty)))
			{
				_createFieldFromPropertyMethod = createFieldFromPropertyMethod;
				_createFieldFromPropertyInstance = new PropertyField();
			}

			var gradientValueProperty = typeof(SerializedProperty).GetProperty(_gradientValueName, BindingFlags.Instance | BindingFlags.NonPublic);

			if (gradientValueProperty != null && gradientValueProperty.PropertyType == typeof(Gradient) && gradientValueProperty.CanRead && gradientValueProperty.CanWrite)
				_gradientValueProperty = gradientValueProperty;

			var hasVisibleChildFieldsMethod = typeof(EditorGUI).GetMethod(_hasVisibleChildFieldsName, BindingFlags.Static| BindingFlags.NonPublic);

			if (hasVisibleChildFieldsMethod != null && hasVisibleChildFieldsMethod.HasSignature(typeof(bool), typeof(SerializedProperty)))
				_hasVisibleChildFieldsMethod = hasVisibleChildFieldsMethod;

			if (_createFieldFromPropertyMethod == null || _gradientValueProperty == null || _hasVisibleChildFieldsMethod == null)
				Debug.LogError(_changedInternalsError);
		}

		#endregion

		#region Extension Methods

		public static IEnumerable<SerializedProperty> Children(this SerializedProperty property)
		{
			if (property.isArray)
			{
				for (int i = 0; i < property.arraySize; i++)
					yield return property.GetArrayElementAtIndex(i);
			}
			else
			{
				// TODO: doesn't work right when property is the root

				var iterator = property.Copy();
				var end = iterator.GetEndProperty();
				var valid = iterator.NextVisible(true);

				while (valid && !SerializedProperty.EqualContents(iterator, end))
				{
					yield return iterator.Copy();
					valid = iterator.NextVisible(false);
				}
			}
		}

		// this property is internal for some reason
		public static Gradient GetGradientValue(this SerializedProperty property) => _gradientValueProperty?.GetValue(property) as Gradient;
		public static void SetGradientValue(this SerializedProperty property, Gradient gradient) => _gradientValueProperty?.SetValue(property, gradient);

		public static VisualElement CreateField(this SerializedProperty property)
		{
			// ArraySize properties return true for HasVisibleChildFields rely on PropertyField state in
			// CreateFieldFromProperty to maintain bindings. TODO: determine if this is relevant

			_createFieldFromPropertyParameters[0] = property;
			return _createFieldFromPropertyMethod?.Invoke(_createFieldFromPropertyInstance, _createFieldFromPropertyParameters) as VisualElement;
		}

		public static bool HasVisibleChildFields(this SerializedProperty property)
		{
			_hasVisibleChildFieldsParameters[0] = property;
			return (bool)_hasVisibleChildFieldsMethod?.Invoke(null, _hasVisibleChildFieldsParameters);
		}

		public static void SetToDefault(this SerializedProperty property)
		{
			if (property.isArray)
			{
				property.ClearArray();
			}
			else if (property.hasChildren)
			{
				var end = property.GetEndProperty();
				property.NextVisible(true);

				while (!SerializedProperty.EqualContents(property, end))
				{
					property.SetToDefault();
					property.NextVisible(false);
				}
			}
			else
			{
				switch (property.propertyType)
				{
					case SerializedPropertyType.Generic: break;
					case SerializedPropertyType.Integer: property.intValue = default; break;
					case SerializedPropertyType.Boolean: property.boolValue = default; break;
					case SerializedPropertyType.Float: property.floatValue = default; break;
					case SerializedPropertyType.String: property.stringValue = string.Empty; break;
					case SerializedPropertyType.Color: property.colorValue = default; break;
					case SerializedPropertyType.ObjectReference: property.objectReferenceValue = default; break;
					case SerializedPropertyType.LayerMask: property.intValue = 0; break;
					case SerializedPropertyType.Enum: property.enumValueIndex = 0; break;
					case SerializedPropertyType.Vector2: property.vector2Value = default; break;
					case SerializedPropertyType.Vector3: property.vector3Value = default; break;
					case SerializedPropertyType.Vector4: property.vector4Value = default; break;
					case SerializedPropertyType.Rect: property.rectValue = default; break;
					case SerializedPropertyType.ArraySize: property.intValue = 0; break;
					case SerializedPropertyType.Character: property.intValue = default; break;
					case SerializedPropertyType.AnimationCurve: property.animationCurveValue = default; break;
					case SerializedPropertyType.Bounds: property.boundsValue = default; break;
					case SerializedPropertyType.Gradient: property.SetGradientValue(default); break;
					case SerializedPropertyType.Quaternion: property.quaternionValue = default; break;
					case SerializedPropertyType.ExposedReference: property.exposedReferenceValue = default; break;
					case SerializedPropertyType.FixedBufferSize: property.intValue = 0; break;
					case SerializedPropertyType.Vector2Int: property.vector2IntValue = default; break;
					case SerializedPropertyType.Vector3Int: property.vector3IntValue = default; break;
					case SerializedPropertyType.RectInt: property.rectIntValue = default; break;
					case SerializedPropertyType.BoundsInt: property.boundsIntValue = default; break;
				}
			}
		}

		public static T GetValue<T>(this SerializedProperty property)
		{
			// TODO: probably should validate the property is holding the expected type (i.e TryGetValue)
			// this boxes for value types but I don't think there's a way around that without dynamic code generation

			if (typeof(T) == typeof(int)) return (T)(object)property.intValue;
			if (typeof(T) == typeof(bool)) return (T)(object)property.boolValue;
			if (typeof(T) == typeof(float)) return (T)(object)property.floatValue;
			if (typeof(T) == typeof(string)) return (T)(object)property.stringValue;
			if (typeof(T) == typeof(Color)) return (T)(object)property.colorValue;
			if (typeof(T) == typeof(LayerMask)) return (T)(object)property.intValue;
			if (typeof(T) == typeof(Enum)) return (T)(object)property.enumValueIndex;
			if (typeof(T) == typeof(Vector2)) return (T)(object)property.vector2Value;
			if (typeof(T) == typeof(Vector3)) return (T)(object)property.vector3Value;
			if (typeof(T) == typeof(Vector4)) return (T)(object)property.vector4Value;
			if (typeof(T) == typeof(Rect)) return (T)(object)property.rectValue;
			if (typeof(T) == typeof(AnimationCurve)) return (T)(object)property.animationCurveValue;
			if (typeof(T) == typeof(Bounds)) return (T)(object)property.boundsValue;
			if (typeof(T) == typeof(Gradient)) return (T)(object)property.GetGradientValue();
			if (typeof(T) == typeof(Quaternion)) return (T)(object)property.quaternionValue;
			if (typeof(T) == typeof(Vector2Int)) return (T)(object)property.vector2IntValue;
			if (typeof(T) == typeof(Vector3Int)) return (T)(object)property.vector3IntValue;
			if (typeof(T) == typeof(RectInt)) return (T)(object)property.rectIntValue;
			if (typeof(T) == typeof(BoundsInt)) return (T)(object)property.boundsIntValue;
			if (typeof(Object).IsAssignableFrom(typeof(T))) return (T)(object)property.objectReferenceValue;

			return default(T);
		}

		public static void ResizeArray(this SerializedProperty arrayProperty, int newSize)
		{
			var size = arrayProperty.arraySize;
			arrayProperty.arraySize = newSize;

			// new items will be a copy of the previous last item so this resets them to their default value
			for (var i = size; i < newSize; i++)
				SetToDefault(arrayProperty.GetArrayElementAtIndex(i));
		}

		public static void RemoveFromArray(this SerializedProperty arrayProperty, int index)
		{
			// If an object is removed from an array of ObjectReference, DeleteArrayElementAtIndex will set the item
			// to null instead of removing it. If the entry is already null it will be removed as expected.
			var item = arrayProperty.GetArrayElementAtIndex(index);

			if (item.propertyType == SerializedPropertyType.ObjectReference && item.objectReferenceValue != null)
				item.objectReferenceValue = null;

			arrayProperty.DeleteArrayElementAtIndex(index);
		}

		public static SerializedProperty GetSibling(this SerializedProperty property, string siblingName)
		{
			var path = property.propertyPath;
			var index = property.propertyPath.LastIndexOf('.');
			var siblingPath = index > 0 ? path.Substring(0, index) + "." + siblingName : siblingName;

			return property.serializedObject.FindProperty(siblingPath);
		}

		public static SerializedProperty GetParent(this SerializedProperty property)
		{
			var path = property.propertyPath;
			var index = property.propertyPath.LastIndexOf('.');

			if (index < 0)
				return property.serializedObject.GetIterator();

			var parentPath = path.Substring(0, index);
			return property.serializedObject.FindProperty(parentPath);
		}

		public static T GetObject<T>(this SerializedProperty property) where T : class
		{
			return GetAncestorObject<T>(property, 0);
		}

		public static T GetParentObject<T>(this SerializedProperty property) where T : class
		{
			return GetAncestorObject<T>(property, 1);
		}

		public static T GetAncestorObject<T>(this SerializedProperty property, int generations) where T : class
		{
			var obj = (object)property.serializedObject.targetObject;
			var elements = property.propertyPath.Replace("Array.data[", "[").Split('.');
			var count = elements.Length - generations;

			for (var i = 0; i < count; i++)
			{
				var element = elements[i];

				if (element.StartsWith("["))
				{
					var indexString = element.Substring(1, element.Length - 2);
					var index = Convert.ToInt32(indexString);

					obj = GetIndexed(obj, index);
				}
				else
				{
					obj = obj.GetType().GetField(element, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).GetValue(obj);
				}
			}

			return obj as T;
		}

		private static object GetIndexed(object obj, int index)
		{
			if (obj is Array array)
				return array.GetValue(index);

			if (obj is IList list)
				return list[index];

			return null;
		}

		#endregion
	}
}