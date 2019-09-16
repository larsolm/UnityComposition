﻿using PiRhoSoft.Utilities.Editor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Composition.Editor
{
	public class VariableField : BaseField<Variable>
	{
		public static readonly string UssClassName = "pirho-variable-field";
		public static readonly string LabelUssClassName = UssClassName + "__label";
		public static readonly string InputUssClassName = UssClassName + "__input";

		private VariableControl _control;

		public VariableField(string label, Variable value, VariableDefinition definition, Object owner) : base(label, null)
		{
			Setup(value, definition, owner);
		}

		private void Setup(Variable value, VariableDefinition definition, Object owner)
		{
			_control = new VariableControl(value, definition, owner);
			_control.AddToClassList(InputUssClassName);
			_control.RegisterCallback<ChangeEvent<Variable>>(evt => base.value = evt.newValue);

			labelElement.AddToClassList(LabelUssClassName);

			this.SetVisualInput(_control);
			AddToClassList(UssClassName);
			SetValueWithoutNotify(value);
		}

		public override void SetValueWithoutNotify(Variable newValue)
		{
			base.SetValueWithoutNotify(newValue);
			_control.SetValueWithoutNotify(newValue);
		}
	}
}