using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UIElements;

namespace PiRhoSoft.Composition.Editor
{
	public class StartGraphViewNode : GraphViewNode, IOutputNode
	{
		public const string StartUssClassName = UssClassName + "--start";

		public GraphViewInputPort Input { get; private set; }
		public List<GraphViewOutputPort> Outputs { get; private set; }

		public override bool IsMovable() => false;

		public StartGraphViewNode(GraphNode node, GraphViewConnector nodeConnector) : base(node)
		{
			AddToClassList(StartUssClassName);

			title = "Start";

			Outputs = new List<GraphViewOutputPort>(Data.Connections.Count);
			CreateOutputs(Outputs, nodeConnector, null);
			RefreshPorts();
		}

		public void UpdateColors(bool active)
		{
			foreach (var output in Outputs)
				output.UpdateColor();
		}

		public override void OnSelected()
		{
			base.OnSelected();

			Selection.activeObject = Data.Node.Graph;
		}

		public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
		{
			evt.menu.AppendAction("View Documentation", action => ViewDocumentation(typeof(Graph)));
			evt.menu.AppendSeparator();
			evt.StopPropagation();
		}
	}
}
