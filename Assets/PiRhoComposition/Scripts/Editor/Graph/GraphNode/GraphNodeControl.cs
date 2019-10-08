using PiRhoSoft.Utilities.Editor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Composition.Editor
{
	public class GraphNodeControl : VisualElement
	{
		public const string Stylesheet = "Graph/GraphNode/GraphNodeStyle.uss";
		public const string UssClassName = "pirho-graph-node";
		public const string LabelUssClassName = UssClassName + "__label";
		public const string IconUssClassName = UssClassName + "__icon";

		public Object Value { get; private set; }

		private Label _label;
		private IconButton _icon;

		public GraphNodeControl(GraphNode value)
		{
			Value = value;

			_label = new Label();
			_label.AddToClassList(LabelUssClassName);

			_icon = new IconButton(Icon.Inspect.Texture, "Select and edit this node", () => GraphEditor.SelectNode(Value as GraphNode));
			_icon.AddToClassList(IconUssClassName);

			Add(_label);
			Add(_icon);

			AddToClassList(UssClassName);
			this.AddStyleSheet(Configuration.EditorPath, Stylesheet);

			Refresh();
		}

		public void SetValueWithoutNotify(Object newValue)
		{
			Value = newValue;
			Refresh();
		}

		private void Refresh()
		{
			_label.text = Value ? Value.name : "Unconnected";
			_icon.SetEnabled(Value);
		}
	}
}
