using PiRhoSoft.Utilities.Editor;
using UnityEngine.UIElements;

namespace PiRhoSoft.Composition.Editor
{
	public class GraphCallerField : BaseField<GraphCaller>
	{
		public static readonly string UssClassName = "pirho-graph-caller-field";
		public static readonly string LabelUssClassName = UssClassName + "__label";
		public static readonly string InputUssClassName = UssClassName + "__input";

		private GraphCallerControl _control;

		public GraphCallerField(string label, GraphCaller value) : base(label, null)
		{
			Setup(value);
		}

		private void Setup(GraphCaller value)
		{
			_control = new GraphCallerControl(value);
			_control.AddToClassList(InputUssClassName);
			_control.RegisterCallback<ChangeEvent<GraphCaller>>(evt => base.value = evt.newValue);

			labelElement.AddToClassList(LabelUssClassName);

			this.SetVisualInput(_control);
			AddToClassList(UssClassName);
			SetValueWithoutNotify(value);
		}

		public override void SetValueWithoutNotify(GraphCaller newValue)
		{
			base.SetValueWithoutNotify(newValue);
			_control.SetValueWithoutNotify(newValue);
		}
	}
}
