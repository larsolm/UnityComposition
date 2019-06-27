using UnityEditor;
using UnityEngine;

namespace PiRhoSoft.PargonUtilities.Editor
{
	public class BasePickerWindow : EditorWindow
	{
		private static BasePickerWindow _instance;

		public static void Show<T>(Rect rect, BasePicker<T> picker) where T : class
		{
			if (_instance != null)
				_instance.Close();

			var position = GUIUtility.GUIToScreenPoint(rect.position);

			_instance = CreateInstance<BasePickerWindow>();
			_instance.ShowAsDropDown(new Rect(position, rect.size), new Vector2(Mathf.Max(rect.width, 200), 330));
			_instance.rootVisualElement.Add(picker);

			picker.OnSelected += selectedItem => _instance.Close();
			picker.FocusSearch();
		}
	}
}
