using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	public static class VisualElementExtensions
	{
		#region Events

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