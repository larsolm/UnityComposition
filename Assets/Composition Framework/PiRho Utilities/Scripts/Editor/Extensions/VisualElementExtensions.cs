using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	public static class VisualElementExtensions
	{
		#region Internal Lookups

		private const string _changedInternalsError = "(PUVEECI) failed to setup VisualElement: Unity internals have changed";

		private const string _serializedPropertyBindEventName = "UnityEditor.UIElements.SerializedPropertyBindEvent, UnityEditor";
		private static Type _serializedPropertyBindEventType;
		private static string _bindPropertyName = "bindProperty";
		private static PropertyInfo _bindPropertyProperty;

		static VisualElementExtensions()
		{
			var serializedPropertyBindEventType = Type.GetType(_serializedPropertyBindEventName);
			var bindPropertyProperty = serializedPropertyBindEventType?.GetProperty(_bindPropertyName, BindingFlags.Instance | BindingFlags.Public);

			if (serializedPropertyBindEventType != null && bindPropertyProperty != null && bindPropertyProperty.PropertyType == typeof(SerializedProperty))
			{
				_serializedPropertyBindEventType = serializedPropertyBindEventType;
				_bindPropertyProperty = bindPropertyProperty;
			}

			if (_serializedPropertyBindEventType == null || _bindPropertyProperty == null)
				Debug.LogError(_changedInternalsError);
		}

		#endregion

		#region Events

		public static bool TryGetPropertyBindEvent(this VisualElement element, EventBase evt, out SerializedProperty property)
		{
			property = evt.GetType() == _serializedPropertyBindEventType
				? _bindPropertyProperty?.GetValue(evt) as SerializedProperty
				: null;

			return property != null;
		}

		public static void SendChangeEvent<T>(this VisualElement element, T previous, T current)
		{
			using (var changeEvent = ChangeEvent<T>.GetPooled(previous, current))
			{
				changeEvent.target = element;
				element.SendEvent(changeEvent);
			}
		}

		#endregion

		#region Stylesheets

		private const string _missingUtilitiesPathError = "(PUEHMUP) failed to determine editor path";
		private const string _missingStylesheetError = "(PUEHMS) failed to load stylesheet: the asset '{0}' could not be found";

		private static string _elementsPath = null;
		private static string _elementsFolder = "Elements/";
		private static string _editorFolder = "PiRho Utilities/Scripts/Editor/";

		public static string ElementsPath
		{
			get
			{
				if (_elementsPath == null)
					_elementsPath = FindElementsPath();

				return _elementsPath;
			}
			set
			{
				_elementsPath = value; // settable so PiRho Utilities can be moved or renamed by end users
			}
		}

		public static void AddStyleSheet(this VisualElement element, string path)
		{
			var fullPath = ElementsPath + path;
			var stylesheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(fullPath);

			if (stylesheet != null)
				element.styleSheets.Add(stylesheet);
			else
				Debug.LogErrorFormat(_missingStylesheetError, fullPath);
		}

		private static string FindElementsPath()
		{
			// PiRho Utilites might be added as a subfolder of a different project so this determines the
			// actual path to the editor scripts by finding the asset representing this script file

			var ids = AssetDatabase.FindAssets(nameof(VisualElementExtensions));

			foreach (var id in ids)
			{
				var path = AssetDatabase.GUIDToAssetPath(id);
				var index = path.IndexOf(_editorFolder);

				if (index >= 0)
					return path.Substring(0, index) + _editorFolder + _elementsFolder;
			}

			Debug.LogError(_missingUtilitiesPathError);
			return "Assets/" + _editorFolder;
		}

		#endregion
	}
}