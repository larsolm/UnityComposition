using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	public class FieldContainer : VisualElement
	{
		public const string UssClassName = "pirho-field-container";

		public Label Label { get; private set; }

		public FieldContainer(string label) : this(label, null)
		{
		}

		public FieldContainer(string label, string tooltip)
		{
			if (label != null)
			{
				Label = new Label(label) { tooltip = tooltip };
				Label.AddToClassList(PropertyField.labelUssClassName);
				Label.AddToClassList(BaseFieldExtensions.LabelUssClassName);
				Add(Label);
			}
			else
			{
				AddToClassList(BaseFieldExtensions.NoLabelVariantUssClassName);
			}

			AddToClassList(BaseFieldExtensions.UssClassName);
			AddToClassList(UssClassName);
		}
	}
}