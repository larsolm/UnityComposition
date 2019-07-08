using TMPro;
using UnityEngine;

namespace PiRhoSoft.Composition.Engine
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(TMP_Text))]
	public abstract class StringBinding : VariableBinding
	{
		[Tooltip("Whether to auto size the rect transform of this object based on the width of the text. Useful for menus that need to expand ")]
		public bool AutoSizeContainer = false;

		private TMP_Text _text;

		public TMP_Text Text
		{
			get
			{
				// can't look up in awake because it's possible to update bindings before the component is enabled

				if (!_text)
					_text = GetComponent<TMP_Text>();

				return _text;
			}
		}

		protected void SetText(string text, bool enabled)
		{
			Text.enabled = enabled;
			Text.text = text;
			Text.autoSizeTextContainer = AutoSizeContainer;
		}
	}
}
