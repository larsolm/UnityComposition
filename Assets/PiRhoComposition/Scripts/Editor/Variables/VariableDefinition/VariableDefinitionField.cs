﻿using PiRhoSoft.Utilities.Editor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Composition.Editor
{
	public class VariableDefinitionField : BaseField<VariableDefinition>
	{
		public static readonly string UssClassName = "pirho-variable-definition-field";
		public static readonly string LabelUssClassName = UssClassName + "__label";
		public static readonly string InputUssClassName = UssClassName + "__input";

		private VariableDefinitionControl _control;

		public VariableDefinitionField(string label, VariableDefinition value, Object owner) : base(label, null)
		{
			Setup(value, owner);
		}

		private void Setup(VariableDefinition value, Object owner)
		{
			_control = new VariableDefinitionControl(value, owner);
			_control.AddToClassList(InputUssClassName);
			_control.RegisterCallback<ChangeEvent<VariableDefinition>>(evt => base.value = evt.newValue);

			labelElement.AddToClassList(LabelUssClassName);

			this.SetVisualInput(_control);
			AddToClassList(UssClassName);
			SetValueWithoutNotify(value);
		}

		public override void SetValueWithoutNotify(VariableDefinition newValue)
		{
			base.SetValueWithoutNotify(newValue);
			_control.SetValueWithoutNotify(newValue);
		}
	}
}