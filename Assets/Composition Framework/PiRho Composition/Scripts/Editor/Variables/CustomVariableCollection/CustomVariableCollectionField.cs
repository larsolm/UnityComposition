using PiRhoSoft.Utilities.Editor;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace PiRhoSoft.Composition.Editor
{
	public class CustomVariableCollectionField : BaseField<CustomVariableCollection>
	{
		public static readonly string UssClassName = "pirho-custom-variable-collection-field";
		public static readonly string LabelUssClassName = UssClassName + "__label";
		public static readonly string InputUssClassName = UssClassName + "__input";

		private CustomVariableCollectionControl _control;

		public CustomVariableCollectionField(string label, CustomVariableCollection value, Object owner) : base(label, null)
		{
			Setup(value, owner);
		}

		private void Setup(CustomVariableCollection value, Object owner)
		{
			// TODO: should this be on control?
			//if (owner is ISchemaOwner schemaOwner)
			//	schemaOwner.SetupSchema();

			_control = new CustomVariableCollectionControl(value);
			_control.AddToClassList(InputUssClassName);
			_control.RegisterCallback<ChangeEvent<CustomVariableCollection>>(evt => base.value = evt.newValue);

			labelElement.AddToClassList(LabelUssClassName);

			this.SetVisualInput(_control);
			AddToClassList(UssClassName);
			SetValueWithoutNotify(value);
		}

		public override void SetValueWithoutNotify(CustomVariableCollection newValue)
		{
			base.SetValueWithoutNotify(newValue);
			_control.SetValueWithoutNotify(newValue);
		}
	}
}
