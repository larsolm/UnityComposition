using PiRhoSoft.Utilities.Editor;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace PiRhoSoft.Composition.Editor
{
	public class VariablePoolField : BaseField<OpenStore>
	{
		public static readonly string UssClassName = "pirho-variable-pool-field";
		public static readonly string LabelUssClassName = UssClassName + "__label";
		public static readonly string InputUssClassName = UssClassName + "__input";

		private VariablePoolControl _control;

		public VariablePoolField(string label, OpenStore value, Object owner) : base(label, null)
		{
			Setup(value, owner);
		}

		private void Setup(OpenStore value, Object owner)
		{
			// TODO: should this be on control?
			//if (owner is ISchemaOwner schemaOwner)
			//	schemaOwner.SetupSchema();

			_control = new VariablePoolControl(value);
			_control.AddToClassList(InputUssClassName);
			_control.RegisterCallback<ChangeEvent<OpenStore>>(evt => base.value = evt.newValue);

			labelElement.AddToClassList(LabelUssClassName);

			this.SetVisualInput(_control);
			AddToClassList(UssClassName);
			SetValueWithoutNotify(value);
		}

		public override void SetValueWithoutNotify(OpenStore newValue)
		{
			base.SetValueWithoutNotify(newValue);
			_control.SetValueWithoutNotify(newValue);
		}
	}
}
