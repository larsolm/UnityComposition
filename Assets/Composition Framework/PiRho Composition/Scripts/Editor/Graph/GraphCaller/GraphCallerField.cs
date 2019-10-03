﻿using PiRhoSoft.Utilities.Editor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Composition.Editor
{
	public class GraphCallerField : BaseField<Object>
	{
		public static readonly string UssClassName = "pirho-graph-caller-field";
		public static readonly string LabelUssClassName = UssClassName + "__label";
		public static readonly string InputUssClassName = UssClassName + "__input";

		private GraphCallerControl _control;

		public GraphCallerField(string label, GraphCaller value, Object owner) : base(label, null)
		{
			Setup(value, owner);
		}

		private void Setup(GraphCaller value, Object owner)
		{
			_control = new GraphCallerControl(value, owner);
			_control.AddToClassList(InputUssClassName);
			_control.RegisterCallback<ChangeEvent<Object>>(evt => base.value = evt.newValue);

			labelElement.AddToClassList(LabelUssClassName);

			this.SetVisualInput(_control);
			AddToClassList(UssClassName);
			SetValueWithoutNotify(value.Graph);
		}

		public override void SetValueWithoutNotify(Object newValue)
		{
			base.SetValueWithoutNotify(newValue);
			_control.SetValueWithoutNotify(newValue);
		}
	}
}
