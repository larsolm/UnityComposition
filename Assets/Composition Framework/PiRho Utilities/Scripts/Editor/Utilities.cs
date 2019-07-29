namespace PiRhoSoft.Utilities.Editor
{
	public static class Utilities
	{
		private static string _elementsPath = null;
		private const string _elementsFolder = "Elements/";
		private const string _editorFolder = "PiRho Utilities/Scripts/Editor/";

		public static string ElementsPath
		{
			get
			{
				if (_elementsPath == null)
					_elementsPath = AssetHelper.FindEditorPath(nameof(Utilities), _editorFolder, _elementsFolder);

				return _elementsPath;
			}
			set
			{
				_elementsPath = value; // settable so PiRho Utilities can be moved or renamed by end users
			}
		}
	}
}
