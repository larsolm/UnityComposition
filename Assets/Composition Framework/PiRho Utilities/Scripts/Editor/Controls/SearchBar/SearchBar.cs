using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.PargonUtilities.Editor
{
	public class SearchBar : VisualElement
	{
		private const string _styleSheetPath = Utilities.AssetPath + "Controls/SearchBar/SearchBar.uss";
		private const string _ussBaseClass = "pargon-search-base";
		private const string _ussLeftClass = "left";
		private const string _ussRightClass = "right";
		private const string _ussTextClass = "text";
		private const string _ussInputClass = "input";

		public string Text => _text.value;

		private readonly TextField _text;
		private readonly VisualElement _right;

		private readonly Texture2D _leftImage = Icon.SearchBarLeft.Content as Texture2D;
		private readonly Texture2D _rightImage = Icon.SearchBarRight.Content as Texture2D;
		private readonly Texture2D _rightCancelImage = Icon.SearchBarRightCancel.Content as Texture2D;

		public SearchBar()
		{
			ElementHelper.AddStyleSheet(this, _styleSheetPath);
			AddToClassList(_ussBaseClass);

			var left = new VisualElement();
			left.AddToClassList(_ussLeftClass);
			left.style.backgroundImage = _leftImage;
			left.style.unitySliceLeft = _leftImage.width;
			left.style.paddingLeft = _leftImage.width;

			_right = new VisualElement();
			_right.AddToClassList(_ussRightClass);
			_right.AddManipulator(new Clickable(ClearText));
			_right.style.backgroundImage = _rightImage;
			_right.style.width = _rightImage.width;

			_text = new TextField();
			_text.AddToClassList(_ussTextClass);
			_text.RegisterValueChangedCallback(evt => TextChanged(evt.newValue));

			left.Add(_text);

			Add(left);
			Add(_right);
		}

		public void FocusText()
		{
			_text[0].Focus();
		}

		public void ClearText()
		{
			_text.value = string.Empty;
		}

		private void TextChanged(string text)
		{
			_right.style.backgroundImage = string.IsNullOrEmpty(text) ? _rightImage : _rightCancelImage;
		}

		#region UXML

		private class Factory : UxmlFactory<SearchBar,Traits> { }

		private class Traits : UxmlTraits
		{
			public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
			{
				get { yield break; }
			}
		}

		#endregion
	}
}