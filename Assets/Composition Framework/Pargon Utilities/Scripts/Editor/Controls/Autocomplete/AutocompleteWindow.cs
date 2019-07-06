using UnityEditor;
using UnityEngine;

namespace PiRhoSoft.PargonUtilities.Editor
{
	public class AutocompleteWindow : EditorWindow
	{
		public AutocompletePopup Popup { get; private set; }
		
		private bool _isClosed = true;
		private bool _shouldClose = false;

		private void OnEnable()
		{
			// trickery to make sure any windows hanging around from a reload are hidden so they don't get stuck

			if (!_isClosed)
				_shouldClose = true;
		}

		private void Update()
		{
			if (_shouldClose)
			{
				Close();
				_shouldClose = false;
			}

			if (Popup != null)
			{
				//var resized = position;
				//resized.height = Popup.worldBound.height;
				//position = resized;

				Debug.Log(position);
			}
		}

		public void Show(AutocompletePopup popup, Vector2 location)
		{
			rootVisualElement.Add(popup);
			ShowPopup();
			position = new Rect(location, new Vector2(150, 250));
			Debug.Log($"start: {location}");

			Popup = popup;
			_isClosed = false;
		}

		public void Hide()
		{
			rootVisualElement.Remove(Popup);
			Close();

			_isClosed = true;
			Popup = null;
		}
	}
}