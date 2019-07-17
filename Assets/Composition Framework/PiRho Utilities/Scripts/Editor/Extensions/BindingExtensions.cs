using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	public static class BindingExtensions
	{
		private const string _changedInternalsError = "(PUBFECI) failed to setup BindingExtensions: Unity internals have changed";
		private const string _typeName = "UnityEditor.UIElements.BindingExtensions, UnityEditor";
		private const string _defaultBindMethodName = "DefaultBind";
		private const string _serializedObjectUpdateWrapperName = "SerializedObjectUpdateWrapper";

		private static Type _serializedObjectUpdateWrapperType;
		private static MethodInfo _defaultBindEnumMethod;

		static BindingExtensions()
		{
			var type = Type.GetType(_typeName);
			var defaultBindGeneric = type?.GetMethod(_defaultBindMethodName, BindingFlags.Static | BindingFlags.NonPublic);
			var defaultBind = defaultBindGeneric?.MakeGenericMethod(typeof(Enum));
			var defaultBindParameters = defaultBind?.GetParameters();
			var serializedObjectUpdateWrapper = type?.GetNestedType(_serializedObjectUpdateWrapperName, BindingFlags.NonPublic);
			var constructor = serializedObjectUpdateWrapper.GetConstructor(new Type[] { typeof(SerializedObject) });

			if (defaultBind != null && serializedObjectUpdateWrapper != null && constructor != null
				&& defaultBindParameters.Length == 6
				&& defaultBindParameters[0].ParameterType == typeof(VisualElement)
				&& defaultBindParameters[1].ParameterType == serializedObjectUpdateWrapper
				&& defaultBindParameters[2].ParameterType == typeof(SerializedProperty)
				&& defaultBindParameters[3].ParameterType == typeof(Func<SerializedProperty, Enum>)
				&& defaultBindParameters[4].ParameterType == typeof(Action<SerializedProperty, Enum>)
				&& defaultBindParameters[5].ParameterType == typeof(Func<Enum, SerializedProperty, Func<SerializedProperty, Enum>, bool>))
			{
				_serializedObjectUpdateWrapperType = serializedObjectUpdateWrapper;
				_defaultBindEnumMethod = defaultBind;
			}
			else
			{
				Debug.LogError(_changedInternalsError);
			}
		}

		public static void DefaultEnumBind(BaseField<Enum> field, SerializedProperty property)
		{
			// 2019.2 and earlier don't support flags in enum bindings (since they use enumValueIndex) and 2019.3 only
			// supports flags on EnumFlagsField specifically

			var type = field.value.GetType();
			var wrapper = Activator.CreateInstance(_serializedObjectUpdateWrapperType, property.serializedObject);

			Func<SerializedProperty, Enum> getter = p => Enum.ToObject(type, p.intValue) as Enum;
			Action<SerializedProperty, Enum> setter = (p, v) => p.intValue = (int)Enum.Parse(type, v.ToString());
			Func<Enum, SerializedProperty, Func<SerializedProperty, Enum>, bool> comparer = (v, p, g) => g(p).Equals(v);

			_defaultBindEnumMethod.Invoke(null, new object[] { field, wrapper, property, getter, setter, comparer });
		}
	}
}