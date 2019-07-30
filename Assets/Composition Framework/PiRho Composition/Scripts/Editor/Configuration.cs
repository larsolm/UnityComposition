using PiRhoSoft.Utilities.Editor;

namespace PiRhoSoft.Composition.Editor
{
	public static class Configuration
	{
		private static string _editorPath = null;
		private const string _editorFolder = "PiRho Composition/Scripts/Editor/";

		internal static string EditorPath
		{
			get
			{
				if (_editorPath == null)
					_editorPath = AssetHelper.FindEditorPath(nameof(Configuration), _editorFolder);

				return _editorPath;
			}
			set
			{
				_editorPath = value; // settable so PiRho Composition can be moved or renamed by end users
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