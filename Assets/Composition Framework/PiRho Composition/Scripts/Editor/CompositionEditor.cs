using PiRhoSoft.Utilities.Editor;

namespace PiRhoSoft.Composition
{
	public static class CompositionEditor
	{
		private static string _editorPath = null;
		private const string _editorFolder = "PiRho Composition/Scripts/Editor/";

		public static string EditorPath
		{
			get
			{
				if (_editorPath == null)
					_editorPath = AssetHelper.FindEditorPath(nameof(CompositionEditor), _editorFolder, string.Empty);

				return _editorPath;
			}
			set
			{
				_editorPath = value; // settable so PiRho Composition can be moved or renamed by end users
			}
		}
	}
}
