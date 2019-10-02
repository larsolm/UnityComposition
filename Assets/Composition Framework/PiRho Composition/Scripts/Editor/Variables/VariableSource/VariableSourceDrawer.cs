using PiRhoSoft.Utilities.Editor;
using UnityEditor;
using UnityEngine.UIElements;

namespace PiRhoSoft.Composition.Editor
{
	[CustomPropertyDrawer(typeof(VariableSource), true)]
	public class VariableSourceDrawer : InlineDrawer
	{
		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			return CreatePropertyGUI(property, false);
		}
	}
}
