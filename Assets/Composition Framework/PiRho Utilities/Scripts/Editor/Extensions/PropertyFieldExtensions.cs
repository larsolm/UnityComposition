using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	public static class PropertyFieldExtensions
	{
		private const string _changedInternalsError = "(PUPFECI) failed to setup PropertyField: Unity internals have changed";

		private static PropertyField _instance;

		private static MethodInfo _createFieldFromProperty;
		private static object[] _createFieldFromPropertyParameters = new object[1];

		private static MethodInfo _configureField;
		private static object[] _configureFieldParameters = new object[2];

		static PropertyFieldExtensions()
		{
			var createFieldFromProperty = typeof(PropertyField).GetMethod(nameof(CreateFieldFromProperty), BindingFlags.Instance | BindingFlags.NonPublic);
			var createFieldFromPropertyParameters = createFieldFromProperty?.GetParameters();

			if (createFieldFromProperty != null && createFieldFromProperty.ReturnType == typeof(VisualElement) && createFieldFromPropertyParameters.Length == 1 && createFieldFromPropertyParameters[0].ParameterType == typeof(SerializedProperty))
				_createFieldFromProperty = createFieldFromProperty;

			var configureField = typeof(PropertyField).GetMethod(nameof(ConfigureField), BindingFlags.Instance | BindingFlags.NonPublic);
			var configureFieldParameters = configureField?.GetParameters();

			if (configureField != null && configureField.ReturnType == typeof(VisualElement) && configureFieldParameters.Length == 2 && configureFieldParameters[1].ParameterType == typeof(SerializedProperty)) // TODO: check parameter[0]
				_configureField = configureField;

			if (_createFieldFromProperty == null || _configureField == null)
				Debug.LogError(_changedInternalsError);

			_instance = new PropertyField(); // the two exposed methods are effectively static so they can be called with a dummy instance
		}

		public static VisualElement CreateFieldFromProperty(SerializedProperty property)
		{
			// TODO: two situations where this doesn't work correctly
			//  - for enums with a binding there seems to be an internal bug (at least in 2019.2)
			//  - for arrays updates depend on state of the PropertyField

			_createFieldFromPropertyParameters[0] = property;
			return _createFieldFromProperty?.Invoke(_instance, _createFieldFromPropertyParameters) as VisualElement;
		}

		public static VisualElement ConfigureField<TField, TValue>(TField field, SerializedProperty property) where TField : BaseField<TValue>
		{
			var method = _configureField?.MakeGenericMethod(typeof(TField), typeof(TValue));

			_configureFieldParameters[0] = field;
			_configureFieldParameters[1] = property;

			return method?.Invoke(_instance, _configureFieldParameters) as VisualElement;
		}
	}
}