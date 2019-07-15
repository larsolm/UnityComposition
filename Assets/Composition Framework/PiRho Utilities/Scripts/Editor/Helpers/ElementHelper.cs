using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	public static class ElementHelper
	{
		#region Paths
		
		private static string _elementsPath = null;
		private static string _elementsFolder = "Elements/";
		private static string _editorFolder = "PiRho Utilities/Scripts/Editor/";
		private const string _missingUtilitiesPathError = "(PUEHMUP) failed to determine editor path";

		public static string ElementsPath
		{
			get
			{
				if (_elementsPath == null)
				{
					// PiRho Utilites might be added as a subfolder of a different project so this determines the
					// actual path to the editor scripts by finding the asset representing this script file

					var ids = AssetDatabase.FindAssets(nameof(ElementHelper));

					foreach (var id in ids)
					{
						var path = AssetDatabase.GUIDToAssetPath(id);
						var index = path.IndexOf(_editorFolder);

						if (index >= 0)
						{
							_elementsPath = path.Substring(0, index) + _editorFolder + _elementsFolder;
							break;
						}
					}

					if (string.IsNullOrEmpty(_elementsPath))
					{
						Debug.LogError(_missingUtilitiesPathError);
						_elementsPath = "Assets/" + _editorFolder;
					}
				}

				return _elementsPath;
			}
			set
			{
				// settable so the utilities can be moved or renamed by end users
				_elementsPath = value;
			}
		}

		#endregion

		#region Drawer Management

		public static VisualElement CreateNextDrawer(SerializedProperty property, string label, FieldInfo fieldInfo, PropertyAttribute attribute)
		{
			var drawer = PropertyHelper.CreateNextDrawer(attribute, fieldInfo);

			if (drawer == null)
				return new PropertyField(property, label);

			var element = drawer.CreatePropertyGUI(property);

			if (element != null)
				return element;

			return CreateIMGUIDrawer(drawer, property, label);
		}

		public static VisualElement CreateIMGUIDrawer(PropertyDrawer drawer, SerializedProperty property, string label)
		{
			return new IMGUIContainer(() =>
			{
				EditorGUI.BeginChangeCheck();
				property.serializedObject.Update();

				var content = new GUIContent(label);
				var height = drawer.GetPropertyHeight(property, content);
				var rect = EditorGUILayout.GetControlRect(true, height);

				drawer.OnGUI(rect, property, content);

				property.serializedObject.ApplyModifiedProperties();
				EditorGUI.EndChangeCheck();
			});
		}

		#endregion

		#region Property Fields

		public static VisualElement SetupPropertyField<T>(BaseField<T> field, FieldInfo fieldInfo)
		{
			field.labelElement.tooltip = PropertyHelper.GetTooltip(fieldInfo);
			field.labelElement.AddToClassList(PropertyField.labelUssClassName);
			field.GetVisualInput().AddToClassList(PropertyField.inputUssClassName);

			return field;
		}

		public static VisualElement CreateEmptyPropertyField(string label)
		{
			var container = new VisualElement();
			var labelElement = new Label(label);

			labelElement.AddToClassList(PropertyField.labelUssClassName);
			container.AddToClassList(BaseFieldExtensions.UssClassName);
			container.Add(labelElement);

			return container;
		}

		#endregion

		#region Stylesheets

		private const string _missingStylesheetError = "(PUEHMS) failed to load stylesheet: the asset '{0}' could not be found";

		public static void AddStyleSheet(VisualElement element, string path)
		{
			var fullPath = ElementsPath + path;
			var stylesheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(fullPath);

			if (stylesheet != null)
				element.styleSheets.Add(stylesheet);
			else
				Debug.LogErrorFormat(_missingStylesheetError, fullPath);
		}

		#endregion
	}
}