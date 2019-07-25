using PiRhoSoft.Utilities.Editor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Composition.Editor
{
	public class VariableSetField : BaseField<VariableSet>
	{
		public static readonly string UssClassName = "pirho-variable-set-field";
		public static readonly string LabelUssClassName = UssClassName + "__label";
		public static readonly string InputUssClassName = UssClassName + "__input";

		private VariableSetControl _control;

		public VariableSetField(string label, VariableSet value, Object owner) : base(label, null)
		{
			Setup(value, owner);
		}

		private void Setup(VariableSet value, Object owner)
		{
			// TODO: Should this stuff be on control?
			if (owner is ISchemaOwner schemaOwner)
				schemaOwner.SetupSchema();

			if (value.Owner != null && value.NeedsUpdate)
			{
				using (new ChangeScope(owner))
					value.Update();
			}

			_control = new VariableSetControl(value);
			_control.AddToClassList(InputUssClassName);
			_control.RegisterCallback<ChangeEvent<VariableSet>>(evt => base.value = evt.newValue);

			labelElement.AddToClassList(LabelUssClassName);

			this.SetVisualInput(_control);
			AddToClassList(UssClassName);
			SetValueWithoutNotify(value);
		}

		public override void SetValueWithoutNotify(VariableSet newValue)
		{
			base.SetValueWithoutNotify(newValue);
			_control.SetValueWithoutNotify(newValue);
		}
	}
}
