using PiRhoSoft.PargonUtilities.Engine;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.PargonUtilities.Editor
{
	public class MessageBox : VisualElement
	{
		private const string _styleSheetPath = Utilities.AssetPath + "Controls/MessageBox/MessageBox.uss";

		public MessageBox(MessageBoxType type, string message)
		{
			ElementHelper.AddStyleSheet(this, _styleSheetPath);

			Add(new Image { image = GetIcon(type) });
			Add(new Label(message));
		}

		private Texture GetIcon(MessageBoxType type)
		{
			switch (type)
			{
				case MessageBoxType.Info: return Icon.Info.Content;
				case MessageBoxType.Warning: return Icon.Warning.Content;
				case MessageBoxType.Error: return Icon.Error.Content;
			}

			return null;
		}
	}
}