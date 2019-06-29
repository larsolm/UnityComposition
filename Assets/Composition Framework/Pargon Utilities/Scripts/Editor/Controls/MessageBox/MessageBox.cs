using PiRhoSoft.PargonUtilities.Engine;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.PargonUtilities.Editor
{
	public class MessageBox : VisualElement
	{
		private const string _styleSheetPath = Utilities.AssetPath + "Controls/MessageBox/MessageBox.uss";

		public MessageBoxType Type
		{
			get
			{
				return _type;
			}
			set
			{
				_type = value;
				_image.image = GetIcon(_type);
			}
		}

		public string Message
		{
			get	{ return _label.text; }
			set	{ _label.text = value; }
		}

		private Image _image;
		private Label _label;
		private MessageBoxType _type;

		public MessageBox(MessageBoxType type, string message)
		{
			ElementHelper.AddStyleSheet(this, _styleSheetPath);

			_image = new Image();
			_label = new Label();

			Type = type;
			Message = message;
			
			Add(_image);
			Add(_label);
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