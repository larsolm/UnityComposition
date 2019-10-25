using PiRhoSoft.Utilities.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Composition.Editor
{
	public class GraphNodeField : BaseField<Object>
	{
		public const string UssClassName = "pirho-graph-node-field";
		public const string LabelUssClassName = UssClassName + "__label";
		public const string InputUssClassName = UssClassName + "__input";

		public GraphNodeControl Control { get; private set; }

		public GraphNodeField(SerializedProperty property) : base(property.displayName, null)
		{
			Setup(property.GetObject<GraphNode>());

			this.ConfigureProperty(property);
		}

		private void Setup(GraphNode value)
		{
			Control = new GraphNodeControl(value);
			Control.AddToClassList(InputUssClassName);
			Control.RegisterCallback<ChangeEvent<Object>>(evt => base.value = evt.newValue);

			labelElement.AddToClassList(LabelUssClassName);

			this.SetVisualInput(Control);
			AddToClassList(UssClassName);
			SetValueWithoutNotify(value);
		}

		public override void SetValueWithoutNotify(Object newValue)
		{
			base.SetValueWithoutNotify(newValue);
			Control.SetValueWithoutNotify(newValue);
		}
	}
}
