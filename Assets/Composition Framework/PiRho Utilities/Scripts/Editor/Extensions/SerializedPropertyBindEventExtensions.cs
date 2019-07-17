using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	public static class SerializedPropertyBindEventExtensions
	{
		private const string _changedInternalsError = "(PUSPBEECI) failed to setup SerializedPropertyBindEvent: Unity internals have changed";

		private const string _typeName = "UnityEditor.UIElements.SerializedPropertyBindEvent, UnityEditor";

		private static Type _type;
		private static PropertyInfo _bindProperty;

		static SerializedPropertyBindEventExtensions()
		{
			var type = Type.GetType(_typeName);
			var property = type?.GetProperty(nameof(bindProperty), BindingFlags.Instance | BindingFlags.Public);

			if (type != null && property != null && property.PropertyType == typeof(SerializedProperty))
			{
				_type = type;
				_bindProperty = property;
			}
			else
			{
				Debug.LogError(_changedInternalsError);
			}
		}

		public static bool Test(EventBase evt, out SerializedProperty property)
		{
			property = bindProperty(evt);
			return property != null;
		}

		public static SerializedProperty bindProperty(EventBase evt)
		{
			return evt.GetType() == _type
				? _bindProperty.GetValue(evt) as SerializedProperty
				: null;
		}
	}
}