using PiRhoSoft.PargonUtilities.Engine;
using UnityEditor;
using UnityEngine.UIElements;

namespace PiRhoSoft.PargonUtilities.Editor
{
	[CustomPropertyDrawer(typeof(AutocompleteAttribute))]
	class AutocompleteDrawer : PropertyDrawer
	{
		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var element = new ListElement();

			return element;
		}
	}
}
