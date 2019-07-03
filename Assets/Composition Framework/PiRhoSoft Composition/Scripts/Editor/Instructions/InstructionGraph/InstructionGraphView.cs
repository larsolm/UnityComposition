using PiRhoSoft.CompositionEngine;
using PiRhoSoft.PargonUtilities.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.CompositionEditor
{
	public class InstructionGraphViewNodeProvider : PickerProvider<Type> { }

	public class InstructionGraphView : GraphView
	{
		private const string _ussGraphClass = "graph";

		public InstructionGraph Graph { get; private set; }

		private readonly InstructionGraphViewWindow _window;
		private readonly InstructionGraphViewNodeProvider _nodeProvider;
		private readonly InstructionGraphViewConnector _nodeConnector;

		private StartNode _start = null;
		private Vector2 _createPosition;

		#region Initialization

		public InstructionGraphView(InstructionGraphViewWindow window, InstructionGraph graph)
		{
			Graph = graph;

			_window = window;
			_nodeProvider = ScriptableObject.CreateInstance<InstructionGraphViewNodeProvider>();
			_nodeConnector = new InstructionGraphViewConnector(_nodeProvider);

			AddToClassList(_ussGraphClass);
			SetupZoom(0.05f, ContentZoomer.DefaultMaxScale);
			SetupNodeProvider();
			SetupNodes();
			SetupConnections();

			nodeCreationRequest = OnShowCreateNode;
			graphViewChanged = OnGraphChanged;
			deleteSelection = OnDeleteSelection;

			this.AddManipulator(new ContentDragger());
			this.AddManipulator(new SelectionDragger());
			this.AddManipulator(new RectangleSelector());
			this.AddManipulator(new ClickSelector());
		}

		private void SetupNodeProvider()
		{
			var types = TypeHelper.ListDerivedTypes<InstructionGraphNode>(false).Where(type => type != typeof(StartNode)).ToList();
			var paths = types.Select(type => TypeHelper.GetAttribute<CreateInstructionGraphNodeMenuAttribute>(type)?.MenuName ?? type.Name).ToList();

			_nodeProvider.Setup("Create Node", paths, types, type => AssetPreview.GetMiniTypeThumbnail(type), selectedType => CreateNode(selectedType));
		}

		#endregion

		#region Overrides

		protected override bool canDeleteSelection => base.canDeleteSelection;
		protected override bool canCopySelection => base.canCopySelection;

		public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
		{
			base.BuildContextualMenu(evt);
		}

		public override List<Port> GetCompatiblePorts(Port start, NodeAdapter nodeAdapter)
		{
			var compatiblePorts = new List<Port>();

			if (start is InstructionGraphViewPort startNode)
			{
				var type = start.GetType();

				foreach (var port in ports.ToList())
				{
					if (port is InstructionGraphViewPort portNode && startNode.Node.Data != portNode.Node.Data && type != port.GetType())
						compatiblePorts.Add(port);
				}
			}

			return compatiblePorts;
		}

		#endregion

		#region Node Management

		private class StartNode : InstructionGraphNode
		{
			public InstructionGraph Graph;

			public override Color NodeColor => Colors.Start;
			public override IEnumerator Run(InstructionGraph graph, InstructionStore variables, int iteration) { yield break; }

			public override void GetConnections(NodeData data) => data.AddConnections(Graph);
			public override void SetConnection(ConnectionData connection, InstructionGraphNode target) => connection.ApplyConnection(Graph, target);
		}

		private void SetupNodes()
		{
			InstructionGraphEditor.SyncNodes(Graph);

			_start = ScriptableObject.CreateInstance<StartNode>();
			_start.Name = "Start";
			_start.Graph = Graph;
			_start.GraphPosition = Vector2.zero;

			AddNode(_start);

			foreach (var node in Graph.Nodes)
				AddNode(node);
		}

		private void SetupConnections()
		{
			ports.ForEach(port =>
			{
				if (port is InstructionGraphViewOutputPort output)
					SetupConnection(output);
			});
		}

		private void SetupConnection(InstructionGraphViewOutputPort output)
		{
			ports.ForEach(port =>
			{
				if (port is InstructionGraphViewInputPort input && output.Connection.To == input.Node.Data.Node)
					AddConnection(output, input);
			});
		}

		private void CreateNode(Type type)
		{
			var windowPosition = _window.rootVisualElement.ChangeCoordinatesTo(_window.rootVisualElement.parent, _createPosition - _window.position.position);
			var graphPosition = contentViewContainer.WorldToLocal(windowPosition);
			var node = InstructionGraphEditor.CreateNode(Graph, type, type.Name, graphPosition);

			AddNode(node);
		}

		private void AddNode(InstructionGraphNode node)
		{
			var nodeElement = new InstructionGraphViewNode(Graph, node, _nodeConnector, node is StartNode);

			AddElement(nodeElement);
		}

		private void AddConnection(InstructionGraphViewOutputPort output, InstructionGraphViewInputPort input)
		{
			output.Connection.SetTarget(input.Node.Data);

			var edge = new Edge
			{
				output = output,
				input = input
			};

			AddEdge(edge);
		}

		private void AddEdge(Edge edge)
		{
			edge.output.Connect(edge);
			edge.input.Connect(edge);

			AddElement(edge);

			if (edge.output is InstructionGraphViewOutputPort output && edge.input is InstructionGraphViewInputPort input)
				InstructionGraphEditor.ChangeConnectionTarget(Graph, output.Connection, input.Node.Data, output.Node.IsStartNode);

			//sourceNodeView.gvNode.RefreshPorts();
			//targetNodeView.gvNode.RefreshPorts();
			//sourceNodeView.UpdatePortInputTypes();
			//targetNodeView.UpdatePortInputTypes();
		}

		#endregion

		#region Callbacks

		private void OnShowCreateNode(NodeCreationContext context)
		{
			_createPosition = context.screenMousePosition;
			SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), _nodeProvider);
		}

		private void OnDeleteSelection(string operationName, AskUser askUser)
		{
			var nodes = selection.OfType<InstructionGraphViewNode>().Where(node => !node.IsStartNode).ToList();

			foreach (var node in nodes)
			{
				var connections = nodes.ToList()
					.OfType<InstructionGraphViewNode>()
					.SelectMany(data => data.Data.Connections)
					.Where(connection => connection.Target == node.Data)
					.ToList();

				InstructionGraphEditor.DestroyNode(Graph, node.Data.Node, connections, _start);

				RemoveElement(node);
			}

			selection.Clear();
		}

		private GraphViewChange OnGraphChanged(GraphViewChange graphViewChange)
		{
			//if (graphView is InstructionGraphView graph && edge.output is InstructionGraphNodeOutputPort output && edge.input is InstructionGraphNodeInputPort input)
			//	InstructionGraphEditor.ChangeConnectionTarget(graph.Editor.Graph, output.Connection, input.Data, output.Connection.From is InstructionGraphViewEditor.StartNode);

			if (graphViewChange.movedElements != null)
			{
				foreach (var element in graphViewChange.movedElements)
				{
					if (element is InstructionGraphViewNode node && !node.IsStartNode)
						InstructionGraphEditor.SetNodePosition(node.Data.Node, node.Data.Node.GraphPosition + graphViewChange.moveDelta);
				}
			}

			if (graphViewChange.elementsToRemove != null)
			{
				foreach (var element in graphViewChange.elementsToRemove)
				{
				}
			}

			if (graphViewChange.edgesToCreate != null)
			{
				foreach (var edge in graphViewChange.edgesToCreate)
					AddEdge(edge);
			}

			return graphViewChange;
		}

		#endregion
	}

	public class InstructionGraphViewEditor : VisualElement
	{
		private const string _styleSheetPath = Composition.StylePath + "Instructions/InstructionGraph/InstructionGraphView.uss";
		private const string _ussBaseClass = "pargon-instruction-graph-view";

		private readonly InstructionGraphViewWindow _window;

		public InstructionGraphView GraphView { get; private set; }
		public InstructionGraph CurrentGraph => GraphView?.Graph;

		public InstructionGraphViewEditor(InstructionGraphViewWindow window)
		{
			_window = window;

			ElementHelper.AddStyleSheet(this, _styleSheetPath);
			AddToClassList(_ussBaseClass);
			CreateToolbar();
		}

		public void SetGraph(InstructionGraph graph)
		{
			if (GraphView == null || graph != GraphView.Graph)
			{
				if (GraphView != null)
					GraphView.RemoveFromHierarchy();

				GraphView = new InstructionGraphView(_window, graph);

				Add(GraphView);
			}
		}

		private void CreateGraph(Type type)
		{
			var graph = AssetHelper.Create(type) as InstructionGraph;
			if (graph)
				SetGraph(graph);
		}

		private void CreateToolbar()
		{
		}
	}
}