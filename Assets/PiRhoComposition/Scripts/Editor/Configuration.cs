using PiRhoSoft.Utilities.Editor;

namespace PiRhoSoft.Composition.Editor
{
	public static class Configuration
	{
		private static string _editorPath = null;
		internal static string EditorPath
		{
			get
			{
				if (_editorPath == null)
					_editorPath = AssetHelper.FindEditorPath(nameof(Configuration), "PiRhoComposition/Scripts/Editor/", "Packages/com.pirho.composition/Scripts/Editor/");

				return _editorPath;
			}
		}

		public enum EditingMode
		{
			Simple,
			Advanced
		}

		public static EditingMode DefaultVariableReferenceEditing = EditingMode.Simple;
		public static EditingMode DefaultExpressionEditing = EditingMode.Simple;
	}
}