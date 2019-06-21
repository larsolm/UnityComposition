using PiRhoSoft.PargonUtilities.Engine;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.PargonUtilities.Editor
{
	public class MessageBox : VisualElement
	{
		private const string _styleSheetPath = Utilities.AssetPath + "Controls/MessageBox/MessageBox.uss";

		public void Setup(MessageBoxType type, string message)
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

		#region UXML

		private class Factory : UxmlFactory<MessageBox, Traits> { }

		private class Traits : UxmlTraits
		{
			private UxmlStringAttributeDescription _type = new UxmlStringAttributeDescription { name = "type", use = UxmlAttributeDescription.Use.Required };
			private UxmlStringAttributeDescription _label = new UxmlStringAttributeDescription { name = "label", use = UxmlAttributeDescription.Use.Required };

			public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
			{
				get { yield break; }
			}

			public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
			{
				base.Init(ve, bag, cc);

				var message = ve as MessageBox;
				var type = _type.GetValueFromBag(bag, cc);
				var label = _label.GetValueFromBag(bag, cc);

				message.Setup(MessageBoxType.Info, label);
			}
		}

		#endregion
	}
}