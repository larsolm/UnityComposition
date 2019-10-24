using PiRhoSoft.Utilities.Editor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace PiRhoSoft.Composition.Editor
{
	[CustomPropertyDrawer(typeof(VariableSource), true)]
	public class VariableSourceDrawer : PropertyDrawer
	{
		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var frame = new Frame();
			frame.SetLabel(property.displayName, this.GetTooltip());

			foreach (var child in property.Children())
			{
				var field = new PropertyField(child);
				frame.Content.Add(field);
			}

			return frame;
		}
	}
}
