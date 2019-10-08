using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UIElements;

namespace PiRhoSoft.Composition.Editor
{
	public class StartGraphViewNode : GraphViewNode
	{
		public const string StartUssClassName = UssClassName + "--start";
		public const string StartOutputUssClassName = GraphViewOutputPort.UssOutputClassName + "-start";

		public GraphViewOutputPort Output { get; private set; }

		public override bool IsMovable() => false;

		public StartGraphViewNode(GraphNode node, GraphViewConnector nodeConnector) : base(node)
		{
			AddToClassList(StartUssClassName);

			title = "Start";
			expanded = false;

			CreateOutput(nodeConnector);
		}

		private void CreateOutput(GraphViewConnector nodeConnector)
		{
			Data.RefreshConnections();

			var connection = Data.Connections[0];

			Output = new GraphViewOutputPort(this, Data.Connections[0], nodeConnector, null) { tooltip = "Click and drag to make a connection from this output" };
			Output.AddToClassList(StartOutputUssClassName);

			titleButtonContainer.Add(Output);

			RefreshPorts();
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
