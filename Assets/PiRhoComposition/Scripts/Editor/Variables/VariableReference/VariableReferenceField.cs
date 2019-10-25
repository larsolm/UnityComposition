using PiRhoSoft.Utilities.Editor;
using UnityEditor;
using UnityEngine.UIElements;

namespace PiRhoSoft.Composition.Editor
{
	public class VariableReferenceField : BaseField<string>
	{
		public const string UssClassName = "pirho-variable-reference-field";
		public const string LabelUssClassName = UssClassName + "__label";
		public const string InputUssClassName = UssClassName + "__input";

		public VariableReferenceControl Control { get; private set; }

		public VariableReferenceField(SerializedProperty property, IAutocompleteItem autocomplete) : base(property.displayName, null)
		{
			Setup(property.GetObject<VariableReference>(), autocomplete);

			var variableProperty = property.FindPropertyRelative(VariableReference.VariableField);
			this.ConfigureProperty(variableProperty);
			this.SetLabel(property.displayName);
		}

		private void Setup(VariableReference value, IAutocompleteItem autocomplete)
		{
			Control = new VariableReferenceControl(value, autocomplete);
			Control.AddToClassList(InputUssClassName);
			Control.RegisterCallback<ChangeEvent<string>>(evt => base.value = evt.newValue);

			labelElement.AddToClassList(LabelUssClassName);

			this.SetVisualInput(Control);
			AddToClassList(UssClassName);
			SetValueWithoutNotify(value.Variable);
		}

		public override void SetValueWithoutNotify(string newValue)
		{
			base.SetValueWithoutNotify(newValue);
			Control.SetValueWithoutNotify(newValue);
		}
	}
}
