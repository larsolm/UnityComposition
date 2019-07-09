using PiRhoSoft.Composition.Engine;
using PiRhoSoft.Utilities.Editor;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Composition.Editor
{
	public class GraphViewNode : Node
	{
		private const string _ussDeleteClass = "delete-button";
		private const string _ussBreakClass = "break-button";
		private const string _ussBreakContainerClass = "break-container";
		private const string _ussEnableClass = "enabled";

		private static readonly Color _edgeColor = new Color(0.49f, 0.73f, 1.0f, 1.0f);
		private static readonly Color _breakColor = new Color(1.0f, 0.2f, 0.2f, 1.0f);
		private static readonly Color _disabledBreakColor = new Color(1.0f, 0.6f, 0.6f, 1.0f);
		private static readonly Color _activeColor = new Color(0.0f, 0.9f, 0.0f, 1.0f);
		private static readonly Color _callstackColor = new Color(0.3f, 0.8f, 0.3f, 1.0f);

		private static readonly Icon _deleteIcon = Icon.BuiltIn("d_LookDevClose");

		public GraphNode.NodeData Data { get; private set; }

		public bool IsStartNode { get; private set; }
		public override bool IsAscendable() => true;
		public override bool IsDroppable() => false;
		public override bool IsMovable() => !IsStartNode;
		public override bool IsResizable() => false;
		public override bool IsRenamable() => !IsStartNode;
		public override bool IsSelectable() => true;
		protected override void ToggleCollapse() { }

		private GraphViewInputPort _input;
		private VisualElement _breakpoint;

		private readonly Graph _graph;

		public GraphViewNode(Graph graph, GraphNode node, GraphViewConnector nodeConnector, bool isStart)
		{
			_graph = graph;

			IsStartNode = isStart;
			Data = new GraphNode.NodeData(node);
			Data.Node.GetConnections(Data);

			title = node.Name;
			titleContainer.style.backgroundColor = node.NodeColor;
			titleContainer.style.unityFontStyleAndWeight = FontStyle.Bold;

			if (!IsStartNode)
				CreateInput(nodeConnector);

			if (Data.Connections.Count == 0)
				expanded = false;
			else
				CreateOutputs(nodeConnector);

			ElementHelper.SetVisible(m_CollapseButton, false);

			RefreshPorts();
			SetPosition(new Rect(node.GraphPosition, Vector2.zero));
		}

		private void CreateInput(GraphViewConnector nodeConnector)
		{
			_input = new GraphViewInputPort(this, nodeConnector) { tooltip = "Drag an output to here to make a connection" };

			var deleteButton = new Image { image = _deleteIcon.Content, tooltip = "Delete this node" };
			deleteButton.AddToClassList(_ussDeleteClass);
			deleteButton.AddManipulator(new Clickable(DeleteNode));

			var container = new VisualElement{ tooltip = "Toggle this node as a breakpoint" };
			container.AddToClassList(_ussBreakContainerClass);
			container.AddManipulator(new Clickable(ToggleBreakpoint));

			_breakpoint = new VisualElement();
			_breakpoint.AddToClassList(_ussBreakClass);

			container.Add(_breakpoint);

			UpdateBreakpoint();

			_input.Add(container);

			titleContainer.Insert(0, _input);
			titleButtonContainer.Add(deleteButton);
		}

		private void CreateOutputs(GraphViewConnector nodeConnector)
		{
			foreach (var connection in Data.Connections)
				outputContainer.Add(new GraphViewOutputPort(this, connection, nodeConnector) { portName = connection.Name, tooltip = "Click and drag to make a connection from this output" });
		}

		public void UpdateColors()
		{
			var executing = _graph.IsExecuting(Data.Node);
			var paused = _graph.DebugState == Graph.PlaybackState.Paused;
			var nodeColor = executing ? (paused ? _breakColor : _activeColor) : _callstackColor;
			var outputs = _input.connections.Select(edge => edge.output).OfType<GraphViewOutputPort>();

			_input.portColor = _edgeColor;

			foreach (var output in outputs)
			{
				if (_graph.IsInCallStack(Data.Node, output.Node.Data.Node.Name))
				{
					output.portColor = _callstackColor;
					_input.portColor = _callstackColor;
				}
				else
				{
					output.portColor = _edgeColor;
				}
			}
		}

		public void UpdateBreakpoint()
		{
			ElementHelper.ToggleClass(_breakpoint, _ussEnableClass, Data.Node.IsBreakpoint);
		}

		private void DeleteNode()
		{
		}

		private void ToggleBreakpoint()
		{
			Data.Node.IsBreakpoint = !Data.Node.IsBreakpoint;
			UpdateBreakpoint();
		}

		private void ViewDocumentation()
		{
			var help = TypeHelper.GetAttribute<HelpURLAttribute>(Data.Node.GetType());
			if (help != null)
				Application.OpenURL(help.URL);
		}

		public override void OnSelected()
		{
			base.OnSelected();

			Selection.activeObject = Data.Node;
		}

		public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
		{
			evt.menu.AppendAction("Toggle Breakpoint", action => ToggleBreakpoint());
			evt.menu.AppendAction("View Documentation", action => ViewDocumentation());
			evt.menu.AppendSeparator();
		}
	}
}
