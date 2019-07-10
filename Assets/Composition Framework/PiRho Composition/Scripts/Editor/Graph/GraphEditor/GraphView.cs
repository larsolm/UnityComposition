using PiRhoSoft.Composition.Engine;
using PiRhoSoft.Utilities.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Composition.Editor
{
	public class GraphViewNodeProvider : PickerProvider<Type> { }
	public class GraphProvider : PickerProvider<Graph> { }

	public class GraphView : UnityEditor.Experimental.GraphView.GraphView
	{
		private const string _ussGraphClass = "graph";

		private static readonly List<GraphNode> _copiedNodes = new List<GraphNode>();

		public Graph Graph { get; private set; }
		public bool CanCut => canCutSelection;
		public bool CanCopy => canCopySelection;
		public bool CanPaste => canPaste;
		public bool CanDuplicate => canDuplicateSelection;
		public bool CanDelete => canDeleteSelection;

		private readonly GraphViewWindow _window;
		private readonly GraphViewNodeProvider _nodeProvider;
		private readonly GraphViewConnector _nodeConnector;

		private StartNode _start = null;
		private Vector2 _createPosition;

		#region Initialization

		public GraphView(GraphViewWindow window, Graph graph)
		{
			Graph = graph;

			_window = window;
			_nodeProvider = ScriptableObject.CreateInstance<GraphViewNodeProvider>();
			_nodeConnector = new GraphViewConnector(_nodeProvider);

			AddToClassList(_ussGraphClass);
			SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
			SetupNodeProvider();
			SetupNodes();
			SetupConnections();
			RegisterCallback<KeyDownEvent>(OnKeyDown);

			nodeCreationRequest = OnShowCreateNode;
			graphViewChanged = OnGraphChanged;
			deleteSelection = OnDeleteSelection;
			canPasteSerializedData = data => canPaste;
			serializeGraphElements = OnCopy;
			unserializeAndPaste = OnPaste;

			this.AddManipulator(new ContentDragger());
			this.AddManipulator(new SelectionDragger());
			this.AddManipulator(new RectangleSelector());
			this.AddManipulator(new ClickSelector());

			Undo.undoRedoPerformed += Rebuild;
			EditorApplication.playModeStateChanged += PlayStateChanged;

			if (Application.isPlaying) // Make sure the callback still gets set if the window was opened during play mode
				PlayStateChanged(PlayModeStateChange.EnteredPlayMode);
		}

		private void SetupNodeProvider()
		{
			var types = TypeHelper.ListDerivedTypes<GraphNode>(false).Where(type => type != typeof(StartNode)).ToList();
			var paths = types.Select(type => TypeHelper.GetAttribute<CreateGraphNodeMenuAttribute>(type)?.MenuName ?? type.Name).ToList();

			_nodeProvider.Setup("Create Node", paths, types, type => AssetPreview.GetMiniTypeThumbnail(type), selectedType => CreateNode(selectedType));
		}

		#endregion

		#region Overrides

		protected override bool canCutSelection => selection.OfType<GraphViewNode>().Where(node => !node.IsStartNode).Any();
		protected override bool canCopySelection => canCutSelection;
		protected override bool canPaste => _copiedNodes.Count > 0;
		protected override bool canDuplicateSelection => canCopySelection;
		protected override bool canDeleteSelection => canCutSelection;

		public override List<Port> GetCompatiblePorts(Port start, NodeAdapter nodeAdapter)
		{
			var compatiblePorts = new List<Port>();

			if (start is GraphViewPort startNode)
			{
				var type = start.GetType();

				foreach (var port in ports.ToList())
				{
					if (port is GraphViewPort portNode && startNode.Node.Data != portNode.Node.Data && type != port.GetType())
						compatiblePorts.Add(port);
				}
			}

			return compatiblePorts;
		}

		public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
		{
			base.BuildContextualMenu(evt);
			_createPosition = evt.mousePosition;
		}

		private Vector2 MouseToGraphPosition(Vector2 position)
		{
			var windowPosition = _window.rootVisualElement.ChangeCoordinatesTo(_window.rootVisualElement.parent, position - _window.position.position);
			return contentViewContainer.WorldToLocal(windowPosition);
		}

		#endregion

		#region Node Management

		private class StartNode : GraphNode
		{
			public Graph Graph;

			public override Color NodeColor => Colors.Start;
			public override IEnumerator Run(Graph graph, GraphStore variables, int iteration) { yield break; }

			public override void GetConnections(NodeData data) => data.AddConnections(Graph);
			public override void SetConnection(ConnectionData connection, GraphNode target) => connection.ApplyConnection(Graph, target);
		}

		private void SetupNodes()
		{
			GraphEditor.SyncNodes(Graph);

			_start = ScriptableObject.CreateInstance<StartNode>();
			_start.name = "Start";
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
				if (port is GraphViewOutputPort output)
					SetupConnection(output);
			});
		}

		private void SetupConnection(GraphViewOutputPort output)
		{
			ports.ForEach(port =>
			{
				if (port is GraphViewInputPort input && output.Connection.To == input.Node.Data.Node)
					AddConnection(output, input);
			});
		}

		private void CreateNode(Type type)
		{
			var position = MouseToGraphPosition(_createPosition);
			var node = GraphEditor.CreateNode(Graph, type, type.Name, position);

			AddNode(node);
		}

		private GraphViewNode AddNode(GraphNode node)
		{
			var nodeElement = node is CommentNode ? (GraphViewNode)new CommentGraphViewNode(node) : new DefaultGraphViewNode(Graph, node, _nodeConnector, node is StartNode);
			AddElement(nodeElement);
			return nodeElement;
		}

		public void RemoveNode(GraphViewNode node)
		{
			if (node is IInputOutputNode ioNode)
			{
				foreach (var edge in ioNode.Input.connections.ToList()) // must use ToList() because internal enumerable is modified
					RemoveEdge(edge);

				foreach (var output in ioNode.Outputs)
				{
					foreach (var edge in output.connections.ToList()) // must use ToList() because internal enumerable is modified
						RemoveEdge(edge);
				}
			}

			GraphEditor.DestroyNode(Graph, node.Data.Node);

			RemoveElement(node);
		}

		private void AddConnection(GraphViewOutputPort output, GraphViewInputPort input)
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
			if (edge.output is GraphViewOutputPort output && edge.input is GraphViewInputPort input)
			{
				GraphEditor.ChangeConnectionTarget(Graph, output.Connection, input.Node.Data, output.Node.IsStartNode);

				edge.output.Connect(edge);
				edge.input.Connect(edge);
				edge.capabilities &= ~Capabilities.Selectable;

				AddElement(edge);
			}
		}

		private void RemoveEdge(Edge edge)
		{
			if (edge.output is GraphViewOutputPort output && edge.input is GraphViewInputPort input)
			{
				GraphEditor.ChangeConnectionTarget(Graph, output.Connection, null, output.Node.IsStartNode);

				edge.output.Disconnect(edge);
				edge.input.Disconnect(edge);

				RemoveElement(edge);
			}
		}

		#endregion

		#region Callbacks

		private void Rebuild()
		{
			DeleteElements(graphElements.ToList());
			SetupNodes();
			SetupConnections();
		}

		private void PlayStateChanged(PlayModeStateChange state)
		{
			if (state == PlayModeStateChange.EnteredPlayMode)
				Graph.OnProcessFrame += FrameChanged;
			else if (state == PlayModeStateChange.ExitingPlayMode)
				Graph.OnProcessFrame -= FrameChanged;
		}

		private void FrameChanged(GraphNode graphNode, int iteration)
		{
			nodes.ForEach(node =>
			{
				if (node is IInputOutputNode ioNode)
					ioNode.UpdateColors(ioNode.Data.Node == graphNode, iteration);
			});
		}

		private void OnShowCreateNode(NodeCreationContext context)
		{
			_createPosition = context.screenMousePosition;
			SearchWindow.Open(new SearchWindowContext(_createPosition), _nodeProvider);
		}

		private void OnDeleteSelection(string operationName, AskUser askUser)
		{
			var nodes = selection.OfType<GraphViewNode>().Where(node => !node.IsStartNode).ToList(); // must use ToList() because internal enumerable is modified

			foreach (var node in nodes)
				RemoveNode(node);

			ClearSelection();
		}

		private string OnCopy(IEnumerable<GraphElement> elements)
		{
			Copy();
			return string.Empty;
		}

		private void OnPaste(string operationName, string data)
		{
			Paste();
		}

		private GraphViewChange OnGraphChanged(GraphViewChange graphViewChange)
		{
			if (graphViewChange.elementsToRemove != null)
			{
				foreach (var element in graphViewChange.elementsToRemove)
				{
					if (element is Edge edge)
						RemoveEdge(edge);
				}
			}
			
			if (graphViewChange.edgesToCreate != null)
			{
				foreach (var edge in graphViewChange.edgesToCreate)
					AddEdge(edge);
			}

			if (graphViewChange.movedElements != null)
			{
				foreach (var element in graphViewChange.movedElements)
				{
					if (element is GraphViewNode node && !node.IsStartNode)
						GraphEditor.SetNodePosition(node.Data.Node, node.Data.Node.GraphPosition + graphViewChange.moveDelta);
				}
			}

			return graphViewChange;
		}

		private void OnKeyDown(KeyDownEvent evt)
		{
			if (evt.keyCode == KeyCode.Home)
				GoToStart();
			else if (evt.keyCode == KeyCode.Tab)
				GoToSelection();
			else if (evt.keyCode == KeyCode.End)
				ShowAll();
		}


		#endregion

		#region View Menu

		public void ShowAll()
		{
			FrameAll();
		}

		public void GoToStart()
		{
			FrameOrigin();
		}

		public void GoToSelection()
		{
			FrameSelection();
		}

		public void GoToNode(GraphNode node)
		{
			var target = nodes.ToList().OfType<GraphViewNode>().Where(graphNode => graphNode.Data.Node == node).FirstOrDefault();
			if (target != null)
			{
				ClearSelection();
				AddToSelection(target);
				FrameSelection();
			}
		}

		#endregion

		#region Edit Menu

		public void Cut()
		{
			Copy();
			Delete();
		}

		public void Copy()
		{
			_copiedNodes.Clear();

			var sourceNodes = selection.OfType<GraphViewNode>().Where(node => !node.IsStartNode).Select(node => node.Data.Node).ToList();
			var copiedData = new List<GraphNode.NodeData>();

			foreach (var node in sourceNodes)
			{
				var copy = GraphEditor.CloneNode(node);
				var data = new GraphNode.NodeData(copy);
				copiedData.Add(data);
				_copiedNodes.Add(copy);
			}

			foreach (var data in copiedData)
			{
				foreach (var connection in data.Connections)
				{
					var index = connection.To != null ? sourceNodes.IndexOf(connection.To) : -1;
					connection.ChangeTarget(index >= 0 ? copiedData[index] : null);
				}
			}
		}

		public void Paste()
		{
			ClearSelection();
			
			foreach (var copy in _copiedNodes)
			{
				var node = AddNode(copy);
				AddToSelection(node);

				if (copy is IInputOutputNode ioNode)
				{
					foreach (var output in ioNode.Outputs)
						SetupConnection(output);
				}
			}
			
			GraphEditor.AddClonedNodes(Graph, _copiedNodes, MouseToGraphPosition(_createPosition));

			Copy(); // re-copy so the same set of nodes can be pasted twice
		}

		public void Duplicate()
		{
			Copy();
			Paste();
		}

		public void Delete()
		{
			DeleteSelectionCallback(AskUser.DontAskUser);
		}

		#endregion
	}

	public class GraphViewEditor : VisualElement
	{
		private const string _styleSheetPath = Engine.Composition.StylePath + "Graph/GraphEditor/GraphView.uss";
		private const string _ussBaseClass = "pirho-graph-view";
		private const string _ussToolbarClass = "toolbar";
		private const string _ussLargeButtonClass = "large-button";
		private const string _ussSmallButtonClass = "small-button";
		private const string _ussBreakDisabledClass = "break-disabled";
		private const string _ussButtonEnabled = "enabled";
		private const string _ussFirstClass = "first";

		private static readonly Icon _playIcon = Icon.BuiltIn("Animation.Play");
		private static readonly Icon _pauseIcon = Icon.BuiltIn("PauseButton");
		private static readonly Icon _stopIcon = Icon.BuiltIn("PreMatQuad");
		private static readonly Icon _stepIcon = Icon.BuiltIn("Animation.NextKey");
		private static readonly Icon _breakIcon = Icon.BuiltIn("Animation.Record");
		private static readonly Icon _logIcon = Icon.BuiltIn("UnityEditor.ConsoleWindow");

		private static BoolPreference _breakpointsEnabled = new BoolPreference("PiRhoSoft.Composition.Graph.BreakpointsEnabled", true);
		private static BoolPreference _loggingEnabled = new BoolPreference("PiRhoSoft.Composition.Graph.LoggingEnabled", false);

		private readonly GraphViewWindow _window;
		private readonly GraphProvider _graphProvider;

		private Label _graphButton;
		private Image _breakButton;
		private Image _loggingButton;
		private Image _lockButton;
		private Image _playButton;
		private Image _pauseButton;
		private Image _stepButton;
		private Image _stopButton;

		public Graph CurrentGraph => GraphView?.Graph;
		public GraphView GraphView { get; private set; }
		public bool IsLocked { get; private set; }

		public GraphViewEditor(GraphViewWindow window)
		{
			_window = window;
			_graphProvider = ScriptableObject.CreateInstance<GraphProvider>();

			ElementHelper.AddStyleSheet(this, _styleSheetPath);
			AddToClassList(_ussBaseClass);

			CreateToolbar();

			// this has to be here because Unity doesn't allow EditorPrefs access in a static constructor
			Graph.IsDebugBreakEnabled = _breakpointsEnabled.Value;
			Graph.IsDebugLoggingEnabled = _loggingEnabled.Value;
		}

		public void SetGraph(Graph graph)
		{
			if (GraphView == null || graph != GraphView.Graph)
			{
				if (GraphView != null)
					GraphView.RemoveFromHierarchy();

				GraphView = new GraphView(_window, graph);

				Add(GraphView);
				RefreshToolbar();
			}
		}

		private void CreateToolbar()
		{
			var toolbar = new VisualElement();
			toolbar.AddToClassList(_ussToolbarClass);

			var editButton = new Label("Edit");
			editButton.AddToClassList(_ussLargeButtonClass);
			editButton.AddToClassList(_ussFirstClass);
			editButton.AddManipulator(new Clickable(() => CreateEditMenu().DropDown(new Rect(editButton.LocalToWorld(new Vector2(0, editButton.layout.height)), Vector2.zero))));

			var viewButton = new Label("View");
			viewButton.AddToClassList(_ussLargeButtonClass);
			viewButton.AddManipulator(new Clickable(() => CreateViewMenu().DropDown(new Rect(viewButton.LocalToWorld(new Vector2(0, viewButton.layout.height)), Vector2.zero))));

			_playButton = new Image { image = _playIcon.Content, tooltip = "Resume execution of the graph" };
			_playButton.AddToClassList(_ussSmallButtonClass);
			_playButton.AddManipulator(new Clickable(() => CurrentGraph.DebugPlay()));

			_pauseButton = new Image { image = _pauseIcon.Content, tooltip = "Pause the execution of the graph" };
			_pauseButton.AddToClassList(_ussSmallButtonClass);
			_pauseButton.AddManipulator(new Clickable(() => CurrentGraph.DebugPause()));

			_stepButton = new Image { image = _stepIcon.Content, tooltip = "Step forward one node in the graph" };
			_stepButton.AddToClassList(_ussSmallButtonClass);
			_stepButton.AddManipulator(new Clickable(() => CurrentGraph.DebugStep()));

			_stopButton = new Image { image = _stopIcon.Content, tooltip = "Stop executing the graph", tintColor = Color.gray };
			_stopButton.AddToClassList(_ussSmallButtonClass);
			_stopButton.AddManipulator(new Clickable(() => CurrentGraph.DebugStop()));

			var gap = new VisualElement();
			gap.style.flexGrow = 1;

			_breakButton = new Image { image = _breakIcon.Content, tooltip = "Enable/Disable node breakpoints for all graphs" };
			_breakButton.AddToClassList(_ussSmallButtonClass);
			_breakButton.AddToClassList(_ussFirstClass);
			_breakButton.AddManipulator(new Clickable(ToggleBreakpointsEnabled));

			_loggingButton = new Image { image = _logIcon.Content, tooltip = "Enable/Disable logging of graph execution for all graphs" };
			_loggingButton.AddToClassList(_ussSmallButtonClass);
			_loggingButton.AddManipulator(new Clickable(ToggleLoggingEnabled));

			_graphButton = new Label() { tooltip = "Select a graph to edit" };
			_graphButton.AddToClassList(_ussLargeButtonClass);
			_graphButton.AddManipulator(new Clickable(() => ShowGraphPicker(GUIUtility.GUIToScreenPoint(_graphButton.layout.position))));

			_lockButton = new Image { tintColor = Color.black, tooltip = "Lock/Unlock this window so it won't be used when other graphs are opened" };
			_lockButton.AddToClassList(_ussSmallButtonClass);
			_lockButton.AddManipulator(new Clickable(ToggleLockingEnabled));

			var watchButton = new Image { image = Icon.View.Content, tooltip = "Open the Watch Window" };
			watchButton.AddToClassList(_ussSmallButtonClass);
			watchButton.AddManipulator(new Clickable(WatchWindow.ShowWindow));

			RefreshToolbar();

			toolbar.Add(editButton);
			toolbar.Add(viewButton);
			toolbar.Add(_playButton);
			toolbar.Add(_pauseButton);
			toolbar.Add(_stepButton);
			toolbar.Add(_stopButton);
			toolbar.Add(gap);
			toolbar.Add(_breakButton);
			toolbar.Add(_loggingButton);
			toolbar.Add(_graphButton);
			toolbar.Add(watchButton);
			toolbar.Add(_lockButton);

			Add(toolbar);
		}

		private GenericMenu CreateViewMenu()
		{
			var	menu = new GenericMenu();

			menu.AddItem(new GUIContent("Show All _END"), false, GraphView.ShowAll);
			menu.AddItem(new GUIContent("Go To Start _HOME"), false, GraphView.GoToStart);
			menu.AddItem(new GUIContent("Zoom To Selection _TAB"), false, GraphView.GoToSelection);

			return menu;
		}

		private GenericMenu CreateEditMenu()
		{
			var menu = new GenericMenu();

			if (GraphView.CanCut)
				menu.AddItem(new GUIContent("Cut %x"), false, GraphView.Cut);
			else
				menu.AddDisabledItem(new GUIContent("Cut %x"));

			if (GraphView.CanCopy)
				menu.AddItem(new GUIContent("Copy %c"), false, GraphView.Copy);
			else
				menu.AddDisabledItem(new GUIContent("Copy %c"));

			if (GraphView.CanPaste)
				menu.AddItem(new GUIContent("Paste %v"), false, GraphView.Paste);
			else
				menu.AddDisabledItem(new GUIContent("Paste %v"));

			if (GraphView.CanDuplicate)
				menu.AddItem(new GUIContent("Duplicate %d"), false, GraphView.Duplicate);
			else
				menu.AddDisabledItem(new GUIContent("Duplicate %d"));

			menu.AddSeparator(string.Empty);

			if (GraphView.CanDelete)
				menu.AddItem(new GUIContent("Delete _DELETE"), false, GraphView.Delete);
			else
				menu.AddDisabledItem(new GUIContent("Delete _DELETE"));

			return menu;
		}

		private void RefreshToolbar()
		{
			var isEnabled = Application.isPlaying && CurrentGraph != null && CurrentGraph.IsRunning;
			var isPlaying = isEnabled && CurrentGraph.DebugState == Graph.PlaybackState.Running;
			var isPaused = isEnabled && CurrentGraph.DebugState == Graph.PlaybackState.Paused;
			var isStepping = isEnabled && CurrentGraph.DebugState == Graph.PlaybackState.Step;
			var isStopping = isEnabled && CurrentGraph.DebugState == Graph.PlaybackState.Stopped;

			_graphButton.text = CurrentGraph == null ? "No Graph Selected" : CurrentGraph.name;
			_lockButton.image = IsLocked ? Icon.Locked.Content : Icon.Unlocked.Content;

			if (!isEnabled)
			{
				_playButton.SetEnabled(false);
				_pauseButton.SetEnabled(false);
				_stepButton.SetEnabled(false);
				_stopButton.SetEnabled(false);
			}

			ElementHelper.ToggleClass(_playButton, _ussButtonEnabled, isEnabled && isPlaying);
			ElementHelper.ToggleClass(_pauseButton, _ussButtonEnabled, isEnabled && isPaused);
			ElementHelper.ToggleClass(_stepButton, _ussButtonEnabled, isEnabled && isStepping);
			ElementHelper.ToggleClass(_stopButton, _ussButtonEnabled, isEnabled && isStopping);
			ElementHelper.ToggleClass(_breakButton, _ussButtonEnabled, Graph.IsDebugBreakEnabled);
			ElementHelper.ToggleClass(_loggingButton, _ussButtonEnabled, Graph.IsDebugLoggingEnabled);
			ElementHelper.ToggleClass(_lockButton, _ussButtonEnabled, IsLocked);
			ElementHelper.ToggleClass(this, _ussBreakDisabledClass, !Graph.IsDebugBreakEnabled);
		}

		private void ShowGraphPicker(Vector2 position)
		{
			var graphs = AssetHelper.GetAssetList<Graph>();

			_graphProvider.Setup("Select Graph", graphs.Paths, graphs.Assets.Cast<Graph>().ToList(), graph => AssetPreview.GetMiniThumbnail(graph), selectedGraph => SetGraph(selectedGraph));

			SearchWindow.Open(new SearchWindowContext(position), _graphProvider);
		}

		private void ToggleBreakpointsEnabled()
		{
			Graph.IsDebugBreakEnabled = !Graph.IsDebugBreakEnabled;
			_breakpointsEnabled.Value = Graph.IsDebugBreakEnabled;

			RefreshToolbar();
		}

		private void ToggleLoggingEnabled()
		{
			Graph.IsDebugLoggingEnabled = !Graph.IsDebugLoggingEnabled;
			_loggingEnabled.Value = Graph.IsDebugLoggingEnabled;

			RefreshToolbar();
		}

		private void ToggleLockingEnabled()
		{
			IsLocked = !IsLocked;
			RefreshToolbar();
		}
	}
}