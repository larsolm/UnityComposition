using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace PiRhoSoft.Utilities.Editor
{
	public static class PropertyHelper
	{
		public static Type GetFieldType(FieldInfo fieldInfo)
		{
			if (fieldInfo.FieldType.IsArray)
				return fieldInfo.FieldType.GetElementType();

			var ilist = fieldInfo.FieldType.GetInterfaces().FirstOrDefault(type => type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IList<>));

			if (ilist != null)
				return ilist.GetGenericArguments()[0];

			return fieldInfo.FieldType;
		}

		public static string GetTooltip(FieldInfo fieldInfo)
		{
			return fieldInfo?.GetCustomAttribute<TooltipAttribute>()?.tooltip ?? string.Empty;
		}

		#region Property Attributes and Drawers

		public static PropertyDrawer CreateNextDrawer(PropertyAttribute attribute, FieldInfo fieldInfo)
		{
			var nextAttribute = GetNextPropertyAttribute(attribute, fieldInfo);
			var drawerType = ScriptAttributeUtility.GetDrawerTypeForType(nextAttribute?.GetType() ?? fieldInfo.FieldType);

			if (drawerType != null)
			{
				var drawer = Activator.CreateInstance(drawerType) as PropertyDrawer;
				drawer.SetFieldInfo(fieldInfo);
				drawer.SetAttribute(attribute);
				return drawer;
			}

			return null;
		}

		public static PropertyAttribute GetNextPropertyAttribute(PropertyAttribute attribute, FieldInfo fieldInfo)
		{
			return fieldInfo.GetCustomAttributes<PropertyAttribute>()
				.Where(a => a.order <= attribute.order) // include equal so none get skipped even though order is undefined
				.Where(a => a.GetType() != attribute.GetType()) // necessary since order comparison includes equal
				.Where(a =>
				{
					var drawerType = ScriptAttributeUtility.GetDrawerTypeForType(attribute.GetType());
					return drawerType != null && TypeHelper.IsCreatableAs<PropertyDrawer>(drawerType);
				})
				.OrderByDescending(a => a.order)
				.FirstOrDefault();
		}

		#endregion
	}
}