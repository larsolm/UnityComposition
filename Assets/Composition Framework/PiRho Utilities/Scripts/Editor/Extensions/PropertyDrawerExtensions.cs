using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	public static class PropertyDrawerExtensions
	{
		private const string _changedInternalsError = "(PUPDECI) failed to setup PropertyDrawer: Unity internals have changed";
		private const string _scriptAttributeUtilityTypeName = "UnityEditor.ScriptAttributeUtility, UnityEditor";

		private static FieldInfo m_FieldInfo;
		private static FieldInfo m_Attribute;

		private static MethodInfo _getDrawerTypeForType;
		private static object[] _getDrawerTypeForTypeParameters = new object[1];

		static PropertyDrawerExtensions()
		{
			var propertyDrawer = typeof(PropertyDrawer);
			var fieldInfo = propertyDrawer.GetField(nameof(m_FieldInfo), BindingFlags.Instance | BindingFlags.NonPublic);
			var attribute = propertyDrawer.GetField(nameof(m_Attribute), BindingFlags.Instance | BindingFlags.NonPublic);

			var scriptAttributeUtility = Type.GetType(_scriptAttributeUtilityTypeName);
			var getDrawerTypeForType = scriptAttributeUtility?.GetMethod(nameof(GetDrawerTypeForType), BindingFlags.Static | BindingFlags.NonPublic);
			var getDrawerTypeForTypeParameters = getDrawerTypeForType?.GetParameters();

			if (fieldInfo != null && fieldInfo.FieldType == typeof(FieldInfo))
				m_FieldInfo = fieldInfo;

			if (attribute != null && attribute.FieldType == typeof(PropertyAttribute))
				m_Attribute = attribute;

			if (scriptAttributeUtility != null && getDrawerTypeForType != null && getDrawerTypeForType.ReturnType == typeof(Type) && getDrawerTypeForTypeParameters.Length == 1 && getDrawerTypeForTypeParameters[0].ParameterType == typeof(Type))
				_getDrawerTypeForType = getDrawerTypeForType;

			if (_getDrawerTypeForType == null || m_FieldInfo == null || m_Attribute == null)
				Debug.LogError(_changedInternalsError);
		}

		public static void SetFieldInfo(this PropertyDrawer drawer, FieldInfo value)
		{
			m_FieldInfo?.SetValue(drawer, value);
		}

		public static void SetAttribute(this PropertyDrawer drawer, PropertyAttribute value)
		{
			m_Attribute?.SetValue(drawer, value);
		}

		public static Type GetFieldType(this PropertyDrawer drawer)
		{
			if (drawer.fieldInfo.FieldType.IsArray)
				return drawer.fieldInfo.FieldType.GetElementType();

			var ilist = drawer.fieldInfo.FieldType.GetInterfaces().FirstOrDefault(type => type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IList<>));

			if (ilist != null)
				return ilist.GetGenericArguments()[0];

			return drawer.fieldInfo.FieldType;
		}

		public static string GetTooltip(this PropertyDrawer drawer)
		{
			return drawer.fieldInfo.GetCustomAttribute<TooltipAttribute>()?.tooltip ?? string.Empty;
		}

		#region Trait Helpers

		public static VisualElement CreateNextElement(this PropertyDrawer drawer, SerializedProperty property)
		{
			var nextDrawer = GetNextDrawer(drawer);

			if (nextDrawer != null)
			{
				var element = nextDrawer.CreatePropertyGUI(property);

				return element != null
					? element
					: new ImGuiDrawer(property, nextDrawer);
			}

			// creating an actual PropertyField will result in infinitely redrawing drawer as well as add another
			// wrapper element around the field
			return PropertyFieldExtensions.CreateFieldFromProperty(property);
		}

		public static PropertyDrawer GetNextDrawer(this PropertyDrawer drawer)
		{
			var nextAttribute = GetNextAttribute(drawer);
			var drawerType = GetDrawerTypeForType(nextAttribute?.GetType() ?? GetFieldType(drawer));

			if (drawerType != null)
			{
				var nextDrawer = drawerType.CreateInstance<PropertyDrawer>();
				nextDrawer.SetFieldInfo(drawer.fieldInfo);
				nextDrawer.SetAttribute(nextAttribute);
				return nextDrawer;
			}

			return null;
		}

		public static PropertyAttribute GetNextAttribute(this PropertyDrawer drawer)
		{
			return drawer.fieldInfo.GetCustomAttributes<PropertyAttribute>()
				.OrderByDescending(attribute => attribute.order) // stable sort so every execution will result in the same order
				.ThenBy(attribute => attribute.GetType().FullName) // but GetCustomAttributes might return a different order so an additional sort is needed
				.SkipWhile(attribute => attribute.GetType() != drawer.attribute.GetType())
				.Where(attribute =>
				{
					var drawerType = GetDrawerTypeForType(attribute.GetType());
					return drawerType != null && drawerType.IsCreatableAs<PropertyDrawer>();
				})
				.ElementAtOrDefault(1);
		}

		public static Type GetDrawerTypeForType(Type type)
		{
			_getDrawerTypeForTypeParameters[0] = type;
			return _getDrawerTypeForType?.Invoke(null, _getDrawerTypeForTypeParameters) as Type;
		}

		#endregion
	}
}