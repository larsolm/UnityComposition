using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace PiRhoSoft.PargonUtilities.Editor
{
	public static class PropertyHelper
	{
		public static void SetToDefault(SerializedProperty property)
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

		public static SerializedProperty GetSibling(SerializedProperty property, string siblingName)
		{
			var path = property.propertyPath;
			var index = property.propertyPath.LastIndexOf('.');
			var siblingPath = index > 0 ? path.Substring(0, index) + "." + siblingName : siblingName;

			return property.serializedObject.FindProperty(siblingPath);
		}

		public static T GetObject<T>(SerializedProperty property) where T : class
		{
			return GetAncestor<T>(property, 0);
		}

		public static T GetParent<T>(SerializedProperty property) where T : class
		{
			return GetAncestor<T>(property, 1);
		}

		public static T GetAncestor<T>(SerializedProperty property, int generations) where T : class
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

		public static PropertyDrawer GetNextDrawer(FieldInfo fieldInfo, PropertyAttribute attribute)
		{
			SetupDrawerDictionary();

			var nextAttribute = attribute;

			while (true)
			{
				var attributes = fieldInfo.GetCustomAttributes<PropertyAttribute>(true).OrderByDescending(a => a.order);
				var drawerAttribute = attributes.FirstOrDefault(next => next.order < nextAttribute.order && !_ignoredTypes.Contains(next.GetType()));
				var typeToDraw = drawerAttribute == null ? fieldInfo.FieldType : drawerAttribute.GetType();

				if (_drawerLookup.TryGetValue(typeToDraw, out var drawer))
				{
					if (TypeHelper.IsCreatableAs<DecoratorDrawer>(drawer))
					{
						nextAttribute = drawerAttribute;
						continue;
					}

					var nextDrawer = Activator.CreateInstance(drawer) as PropertyDrawer;
					_fieldInfoField.SetValue(nextDrawer, fieldInfo);
					_attributeField.SetValue(nextDrawer, drawerAttribute);

					return nextDrawer;
				}

				return null;
			}
		}

		#region Drawer Management

		private static readonly List<Type> _ignoredTypes = new List<Type>()
		{
			typeof(TooltipAttribute),
			typeof(ContextMenuItemAttribute)
		};

		private static Dictionary<Type, Type> _drawerLookup;
		private static FieldInfo _attributeField;
		private static FieldInfo _fieldInfoField;

		private static void SetupDrawerDictionary()
		{
			if (_drawerLookup == null)
			{
				_drawerLookup = new Dictionary<Type, Type>();

				var assembly = Assembly.GetAssembly(typeof(UnityEditor.Editor));
				var internalType = assembly.GetType("UnityEditor.ScriptAttributeUtility", false);
				var drawerType = assembly.GetType("UnityEditor.ScriptAttributeUtility+DrawerKeySet", false);
				var dictionaryField = internalType.GetField("s_DrawerTypeForType", BindingFlags.Static | BindingFlags.NonPublic);
				var drawerField = drawerType.GetField("drawer", BindingFlags.Public | BindingFlags.Instance);
				var typeField = drawerType.GetField("type", BindingFlags.Public | BindingFlags.Instance);

				var dictionary = dictionaryField.GetValue(null) as IDictionary;

				foreach (var value in dictionary.Values)
				{
					var type = typeField.GetValue(value) as Type;
					var drawer = drawerField.GetValue(value) as Type;

					if (!_drawerLookup.ContainsKey(type))
						_drawerLookup.Add(type, drawer);
				}

				var propertyType = assembly.GetType("UnityEditor.PropertyDrawer", false);
				_fieldInfoField = propertyType.GetField("m_FieldInfo", BindingFlags.NonPublic | BindingFlags.Instance);
				_attributeField = propertyType.GetField("m_Attribute", BindingFlags.NonPublic | BindingFlags.Instance);
			}
		}

		#endregion
	}
}
