using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	public class FieldContainer : VisualElement
	{
		public FieldContainer(string label = null, string tooltip = null)
		{
			AddToClassList(BaseField<int>.ussClassName);

			if (!string.IsNullOrEmpty(label))
				CreateLabel(label, tooltip);
			else
				this.tooltip = tooltip;
		}

		private void CreateLabel(string text, string tooltip)
		{
			var label = new Label(text) { tooltip = tooltip };
			label.AddToClassList(PropertyField.labelUssClassName);
			label.AddToClassList(BaseField<string>.labelUssClassName);

			Add(label);
		}
	}
}
