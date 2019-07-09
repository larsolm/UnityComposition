using PiRhoSoft.Composition.Engine;
using PiRhoSoft.SnippetsEditor;
using PiRhoSoft.Utilities.Editor;
using PiRhoSoft.UtilityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace PiRhoSoft.Composition.Editor
{
	[InitializeOnLoad]
	public class InstructionGraphWindow : ViewportWindow
	{
		// inspiration and parts of the implementation for this class have been adapted from the MIT licensed Unity
		// Node Editor Base Extension: https://unitylist.com/p/tb/Unity-Node-Editor-Base

		private static readonly Icon _gridTexture = Icon.Base64("iVBORw0KGgoAAAANSUhEUgAAAEAAAABACAIAAAAlC+aJAAAABGdBTUEAALGPC/xhBQAAAAlwSFlzAAAOwgAADsIBFShKgAAAABl0RVh0U29mdHdhcmUAcGFpbnQubmV0IDQuMC4xNzNun2MAAAKnSURBVGhD7ZnbbiJBDETzNwTCfbm/Av//S3uGKkXTETYbCS2zG5+HpNsu3F3tYWaUvH2kTCaT1Wq13W4ZgKMtSv26obETLYpTarlcRhogNZ1Od7vdbDZLZOPxeL1eX6/XMlAGvkCqDPSgRBkwipcBUwZ6KPUyA4gS0LHeZrNhAI62KEU50NiJFsUptVgsIg2QYn/4xEYie39/52Q7A/zKYUnOw5MYlgRPYihFozyJQcZxeHIPjhXN5XJ508IJ6MCTmD+XeZTyUMaxHg6HzoBbEkATh34JORBAiaEb6L7SMUjRUU4fc7RFqZfdhRwIoEQZMIqXAVMGeihVBgyp7xlAlICO9epB1qH48w2o9QnUoqGexCADT2Io9VCGSWT89PweHNZ+v+/ehRwIQMd6lLud79rRFqVu+99q7ESL4tpZpIFP2ef4Lhy/DbglATSxvgNG8ecbQJ2AFB3l9DFHW5RSZzV2okVxSj35NupAACXKgFG8DJgy0EOpMmBIfc8AogR0rFfPgQ7Fn9uB1/xh66FMGvD8HnTyeDx2BjiPBNrNufIBBuBoi1JU1OUBTrQoTikKRhpQit3T+UQ2n88p9V+8zOmSikA69LuQAwGUKANG8TJgykAPpcqAIfXDDCBKQMd6g36QoUugljpws/3haItS6oDGTrQorg5EGiDF2dMBXhYSGR5YrnuVQPovwkH4ZQ67OVwYqD2JUQc8iaEULfUkgLNni1xpnt+DJrgDuqQiuAqH/h1AnYB06HchBwIoUQaM4mXAlIEeSpUBQ+qHGWCUwPOC5w47YwCOtiiFT9DYiRbFKcWDNtIAKfaNT3aZyEajEQfRGcBHAnviMHjr0P4cbVFqf0NjJ1oUpxSbizSg1Ol0wmoiY/f+Jx8mchCBJzF/X4bgfD7/Bp0ChIMH9TUUAAAAAElFTkSuQmCC");
		private const string _windowIcon = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAABGdBTUEAALGPC/xhBQAAACBjSFJNAAB6JgAAgIQAAPoAAACA6AAAdTAAAOpgAAA6mAAAF3CculE8AAAACXBIWXMAABJ0AAASdAHeZh94AAAAB3RJTUUH4wEOBR4Wp+XVagAAABh0RVh0U29mdHdhcmUAcGFpbnQubmV0IDQuMS41ZEdYUgAAATdJREFUOE+lkUtuglAYhWnnfQwITUdEQF6CPFRimBqcdKCzdl3GCZg0cS8O3IeJ2NA90HNSBsqlsYkkPx+H++VcbpDqur5pfm+SdJem6UOSJI/Xhh79i4LRaPQ0Ho8PmNNkMqnAsqGQMQf6FwVBEDzj5ck0zfcwDD/7/f4badv2x3nmOj36QkEcx5XneVs878FNQyHToy8URFFUYsdsOByuDcOYksjzVs7odRZg+AU5xJ3v+ysSuWjlnF5nAYQSZ1wOBoMCnDUUMr3OAuxSOY6zhbgHNw2FTE8oUFWVBWWv18sgrZGnJM4+P89cp0dfKMD5+AW567o7cNWwaOWcXmcBFktd1xeWZRWaps1I5GUrL+j9VXDEb/oCv8FTw658FApw3SuK8iLL8uu1oUf/ouCW6Xz5/6mlH0LCqCZdcm2YAAAAAElFTkSuQmCC";
		private const string _pauseIcon = "iVBORw0KGgoAAAANSUhEUgAAABEAAAAOCAYAAADJ7fe0AAAABGdBTUEAALGPC/xhBQAAACBjSFJNAAB6JgAAgIQAAPoAAACA6AAAdTAAAOpgAAA6mAAAF3CculE8AAAACXBIWXMAAA7CAAAOwgEVKEqAAAAAB3RJTUUH4wEOBR0hNHUjpgAAABh0RVh0U29mdHdhcmUAcGFpbnQubmV0IDQuMS41ZEdYUgAAAGVJREFUOE+l0bERwCAMQ1EvxACswWhZ2LHuKKSLKByKV+gfrojMvGZjl41dnzDnHOXZxqkxGVCPVsltnRqTAe7ANSYD3IFrTAa4A9eYDHAHrjEZ4A5cYzKgHt1/8R82dtnYERHxAqpHVWhCcdFJAAAAAElFTkSuQmCC";
		private const string _stopIcon = "iVBORw0KGgoAAAANSUhEUgAAABEAAAAOCAYAAADJ7fe0AAAABGdBTUEAALGPC/xhBQAAACBjSFJNAAB6JgAAgIQAAPoAAACA6AAAdTAAAOpgAAA6mAAAF3CculE8AAAACXBIWXMAAA7CAAAOwgEVKEqAAAAAB3RJTUUH4wEOBR0hNHUjpgAAABh0RVh0U29mdHdhcmUAcGFpbnQubmV0IDQuMS41ZEdYUgAAAE9JREFUOE/t0bENACAIBMBfiAFYg9FcGKEUiBFtLa7gw9MAVX1Whl1l2JUCZiYzNih2lsHZkhjdkNhZBudLoRT9I9nRkfcX3yjDrjLsAIAJNpdTpphdxWYAAAAASUVORK5CYII=";

		private static readonly Label _windowLabel = new Label(Icon.Base64(_windowIcon), label: "Instruction Graph");
		private static readonly Style _headerStyle = new Style(CreateHeaderStyle);
		private static readonly Style _connectionStyle = new Style(CreateConnectionStyle);
		private static readonly Style _commentStyle = new Style(CreateCommentStyle);
		private static readonly Style _watchStyle = new Style(CreateWatchStyle);
		private static readonly Label _outputButton = new Label(Icon.BuiltIn("sv_icon_dot0_sml"));//, "Click and drag to make a connection from this output");
		private static readonly Label _inputButton = new Label(Icon.BuiltIn("sv_icon_dot0_sml"));//, "Drag an output to here to make a connection");
		private static readonly Label _removeButton = new Label(Icon.BuiltIn("d_Toolbar Minus"));//, "Remove this node");
		private static readonly Label _playButton = new Label(Icon.BuiltIn("Animation.Play"), "", "Resume graph execution");
		private static readonly Label _playDisabledButton = new Label(Icon.BuiltIn("Animation.Play"));
		private static readonly Label _pauseButton = new Label(Icon.Base64(_pauseIcon), "", "Pause graph execution before running the next node");
		private static readonly Label _pauseDisabledButton = new Label(Icon.Base64(_pauseIcon));
		private static readonly Label _stepButton = new Label(Icon.BuiltIn("Animation.NextKey"), "", "Run the next node then pause again");
		private static readonly Label _stepDisabledButton = new Label(Icon.BuiltIn("Animation.NextKey"));
		private static readonly Label _stopButton = new Label(Icon.Base64(_stopIcon), "", "Halt execution of the current branch of the graph and continue with the next branch");
		private static readonly Label _stopDisabledButton = new Label(Icon.Base64(_stopIcon));
		private static readonly Label _addBreakpointButton = new Label(Icon.BuiltIn("Animation.Record"), "", "Set a breakpoint on this node");
		private static readonly Label _removeBreakpointButton = new Label(Icon.BuiltIn("Animation.Record"), "", "Remove the breakpoint from this node");
		private static readonly Label _breakpointDisabledButton = new Label(Icon.BuiltIn("Animation.Record"));

		private static readonly Label _enableBreakpointsButton = new Label(Icon.BuiltIn("Animation.Record"), "", "Enable node breakpoints for all graphs");
		private static readonly Label _disableBreakpointsButton = new Label(Icon.BuiltIn("Animation.Record"), "", "Disable node breakpoins for all graphs");
		private static readonly Label _enableLoggingButton = new Label(Icon.BuiltIn("UnityEditor.ConsoleWindow"), "", "Enable logging of graph execution for all graphs");
		private static readonly Label _disableLoggingButton = new Label(Icon.BuiltIn("UnityEditor.ConsoleWindow"), "", "Disable logging of graph execution for all graphs");

		private static readonly Label _lockButton = new Label(Icon.BuiltIn("AssemblyLock"), "", "Lock this window so it won't be used when other graphs are opened");
		private static readonly Label _unlockButton = new Label(Icon.BuiltIn("AssemblyLock"), "", "Unlock this window so it can be used to show other graphs");
		private static readonly Label _openWatchButton = new Label(Icon.BuiltIn("UnityEditor.LookDevView"), "", "Open the watch window");

		private static readonly GUIContent _createNodeContent = new GUIContent("Create", "Create a new node");

		private const float _knobRadius = 6.0f;
		private const float _toolbarHeight = 17.0f;
		private const float _toolbarButtonWidth = 60.0f;
		private const float _toolbarPadding = 5.0f;
		private const float _dragTolerance = 4.0f;

		private const float _gridSize = 64.0f / 5.0f;

		private static BoolPreference _breakpointsEnabled = new BoolPreference("PiRhoSoft.Composition.InstructionGraph.BreakpointsEnabled", true);
		private static BoolPreference _loggingEnabled = new BoolPreference("PiRhoSoft.Composition.InstructionGraph.LoggingEnabled", false);
		private static BoolPreference _snapToGrid = new BoolPreference("PiRhoSoft.Composition.InstructionGraph.SnapToGrid", true);
		private static IntPreference _snapAmount = new IntPreference("PiRhoSoft.Composition.InstructionGraph.SnapAmount", 1);

		private static Color _hoveredColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
		private static Color _selectedColor = new Color(0.96f, 0.95f, 0.2f, 0.75f);
		private static Color _nodeColor = new Color(0.23f, 0.24f, 0.29f, 1.0f);
		private static Color _commentColor = new Color(0.13f, 0.24f, 0.14f, 1.0f);
		private static Color _knobColor = new Color(0.49f, 0.73f, 1.0f, 1.0f);

		private static Color _breakColor = new Color(1.0f, 0.2f, 0.2f, 1.0f);
		private static Color _disabledBreakColor = new Color(1.0f, 0.6f, 0.6f, 1.0f);
		private static Color _activeColor = new Color(0.0f, 0.9f, 0.0f, 1.0f);
		private static Color _callstackColor = new Color(0.3f, 0.8f, 0.3f, 1.0f);

		private GenericMenu _viewMenu;
		private GenericMenu _editMenu;
		private GenericMenu _contextMenu;
		private SettingsMenu _settingsMenu;
		private SelectionTree _nodeTree;
		private NodeList _nodeList;

		private Graph _graph = null;
		private StartNode _start = null;
		private List<GraphNode.NodeData> _nodes = new List<GraphNode.NodeData>();

		private Vector2 _createPosition;
		private GraphNode.NodeData _toRemove = null;
		private int _showContextMenu = 0;
		private int _showNodeId = -1;
		private bool _showCreateNode = false;

		private bool _isLocked = false;

		private MouseState _mouseMoveState = MouseState.Select;
		private MouseState _mouseDragState = MouseState.Hover;
		private bool _simulateDrag = false;
		private Vector2 _mouseDragOffset;
		private Vector2 _mouseDragStart;
		private Rect _mouseDragBounds;

		private GraphNode.NodeData _hoveredNode;
		private GraphNode.ConnectionData _hoveredOutput;
		private GraphNode.NodeData _hoveredInput;
		private GraphNode.NodeData _hoveredInteraction;

		private List<GraphNode.NodeData> _pendingNodes = new List<GraphNode.NodeData>();
		private List<GraphNode.ConnectionData> _pendingOutputs = new List<GraphNode.ConnectionData>();
		private GraphNode.NodeData _pendingInput;

		private List<GraphNode.NodeData> _selectedNodes = new List<GraphNode.NodeData>();
		private List<GraphNode.ConnectionData> _selectedConnections = new List<GraphNode.ConnectionData>();
		private GraphNode.NodeData _selectedInteraction;

		#region Window Access

		public static InstructionGraphWindow FindWindowForGraph(Graph graph)
		{
			var windows = Resources.FindObjectsOfTypeAll<InstructionGraphWindow>();

			foreach (var window in windows)
			{
				if (window._graph == graph)
					return window;
			}

			foreach (var window in windows)
			{
				if (window._graph == null)
					return window;
			}

			foreach (var window in windows)
			{
				if (!window._isLocked)
				{
					window.SetGraph(graph);
					return window;
				}
			}

			return null;
		}

		public static InstructionGraphWindow ShowWindowForGraph(Graph graph)
		{
			var window = FindWindowForGraph(graph);

			if (window == null)
				window = ShowNewWindow();
			else
				window.Focus();

			if (window._graph != graph)
				window.SetGraph(graph);

			return window;
		}

		//[MenuItem("Window/PiRho Soft/Instruction Graph")]
		public static InstructionGraphWindow ShowNewWindow()
		{
			var window = CreateInstance<InstructionGraphWindow>();
			window.Show();
			return window;
		}

		#endregion

		#region Setup

		static InstructionGraphWindow()
		{
			Graph.OnBreakpointHit += BreakpointHit;
		}

		protected override void Setup(InputManager input)
		{
			base.Setup(input);

			titleContent = _windowLabel.Content;

			CreateNodeTree(ref _nodeTree, ref _nodeList);
			CreateViewMenu(ref _viewMenu, string.Empty);
			CreateEditMenu(ref _editMenu);

			_contextMenu = CreateContextMenu();
			_settingsMenu = new SettingsMenu();

			SetupInput(input);
			SetupNodes();

			autoRepaintOnSceneChange = true;
			Undo.undoRedoPerformed += RebuildNodes;
			Selection.selectionChanged += UpdateSelection;
			EditorApplication.playModeStateChanged += PlayModeChanged;

			// this has to be here because Unity doesn't allow EditorPrefs access in a static constructor
			Graph.IsDebugBreakEnabled = _breakpointsEnabled.Value;
			Graph.IsDebugLoggingEnabled = _loggingEnabled.Value;
		}

		protected override void Teardown()
		{
			EditorApplication.playModeStateChanged -= PlayModeChanged;
			Selection.selectionChanged -= UpdateSelection;
			Undo.undoRedoPerformed -= RebuildNodes;

			TeardownNodes();

			_viewMenu = null;
			_editMenu = null;
			_contextMenu = null;
			_settingsMenu = null;
			_nodeTree = null;
			_nodeList = null;

			base.Teardown();
		}

		private void PlayModeChanged(PlayModeStateChange state)
		{
			RebuildNodes();
		}

		private void RebuildNodes()
		{
			TeardownNodes();
			SetupNodes();
			UpdateSelection();
			Repaint();
		}

		#endregion

		#region Graph Management

		private class StartNode : GraphNode
		{
			public Graph Graph;

			public override Color NodeColor => Colors.Start;
			public override IEnumerator Run(Graph graph, GraphStore variables, int iteration) { yield break; }

			//public override void GetConnections(NodeData data) => Graph.GetConnections(data);
			//public override void SetConnection(ConnectionData connection, InstructionGraphNode target) => Graph.SetConnection(connection, target);
		}

		public void SetGraph(Graph graph)
		{
			if (graph != _graph)
			{
				_graph = graph;
				TeardownNodes();
				SetupNodes();
				ShowAll();
			}
		}

		private void CreateGraph(Type type)
		{
			var graph = AssetHelper.Create(type) as Graph;
			if (graph)
				SetGraph(graph);
		}

		#endregion

		#region Node Management

		private void SetupNodes()
		{
			if (_graph != null)
			{
				GraphEditor.SyncNodes(_graph);

				_start = CreateInstance<StartNode>();
				_start.Name = "Start";
				_start.Graph = _graph;

				AddNode(_start);

				foreach (var node in _graph.Nodes)
					AddNode(node);

				// add all the nodes first so connection nodes are available before trying to wire them up

				foreach (var node in _nodes)
					SetupOutputConnections(node);
			}
		}

		private void TeardownNodes()
		{
			_nodes.Clear();
			_start = null;
		}

		private void RefreshNode(GraphNode node)
		{
			var data = GetNodeData(node);
			if (data != null)
			{
				data.ClearConnections();
				node.GetConnections(data);
				SetupOutputConnections(data);
			}
		}

		private void CreateNode(Type type, string name, Vector2 position)
		{
			var node = GraphEditor.CreateNode(_graph, type, name, position);
			var data = AddNode(node);
			SetSelection(data);
		}

		public GraphNode.NodeData AddNode(GraphNode node)
		{
			var data = new GraphNode.NodeData(node);
			node.GetConnections(data);
			_nodes.Add(data);
			return data;
		}

		private void RemoveNode(GraphNode.NodeData node)
		{
			var connections = _nodes
				.SelectMany(data => data.Connections)
				.Where(connection => connection.Target == node)
				.ToList();

			if (node.Node != _start)
			{
				GraphEditor.DestroyNode(_graph, node.Node, connections, _start);
				_nodes.Remove(node);
			}
		}

		private GraphNode.NodeData GetNodeData(GraphNode node)
		{
			foreach (var data in _nodes)
			{
				if (data.Node == node)
					return data;
			}

			return null;
		}

		private Rect TakeHeaderHeight(ref Rect rect)
		{
			return RectHelper.TakeHeight(ref rect, 0);// InstructionGraphNode.NodeData.HeaderHeight);
		}

		private Rect GetInputBounds(GraphNode.NodeData node)
		{
			var rect = new Rect();
			var header = TakeHeaderHeight(ref rect);
			header.width = header.height;

			return RectHelper.Adjust(header, RectHelper.IconWidth, RectHelper.IconWidth, RectHorizontalAlignment.Center, RectVerticalAlignment.Middle);
		}

		private Rect GetOutputBounds(GraphNode.ConnectionData connection)
		{
			var node = GetNodeData(connection.From);
			var rect = new Rect();

			TakeHeaderHeight(ref rect);
			
			//rect.x = rect.xMax - InstructionGraphNode.NodeData.LineHeight;
			//rect.y += connection.FromIndex * InstructionGraphNode.NodeData.LineHeight;
			//rect.width = InstructionGraphNode.NodeData.LineHeight;
			//rect.height = InstructionGraphNode.NodeData.LineHeight;

			return rect;
		}

		private Rect GetInteractionBounds(GraphNode.NodeData node, int index)
		{
			var rect = new Rect();
			var header = TakeHeaderHeight(ref rect);
			var offset = (header.height * (index + 1));

			header.x = header.xMax - offset;
			header.width = header.height;

			return RectHelper.Adjust(header, RectHelper.IconWidth, RectHelper.IconWidth, RectHorizontalAlignment.Center, RectVerticalAlignment.Middle);
		}

		#endregion

		#region Connection Management

		private void SetupOutputConnections(GraphNode.NodeData node)
		{
			foreach (var connection in node.Connections)
			{
				foreach (var target in _nodes)
				{
					if (target.Node != _start && connection.To == target.Node)
						connection.SetTarget(target);
				}
			}
		}

		private void SetConnection(GraphNode.ConnectionData connection, GraphNode.NodeData node)
		{
			GraphEditor.ChangeConnectionTarget(_graph, connection, node, connection.From == _start);
		}

		#endregion

		#region Selection Management

		private void UpdateSelection()
		{
			_selectedNodes.Clear();

			foreach (var selection in Selection.objects)
			{
				if (selection is GraphNode node)
				{
					var data = GetNodeData(node);
					if (data != null)
						_selectedNodes.Add(data);
				}
				else if (selection is Graph graph && graph == _graph)
				{
					var data = GetNodeData(_start);
					if (data != null)
						_selectedNodes.Add(data);
				}
			}

			Repaint();
		}

		private void ApplySelection()
		{
			Selection.objects = _selectedNodes.Select(node => node.Node == _start ? (UnityEngine.Object)_graph : (UnityEngine.Object)node.Node).ToArray();
		}

		private void RefreshSelectedNode()
		{
			if (Selection.activeObject is GraphNode node)
				RefreshNode(node);
			else if (Selection.activeObject is Graph graph)
				RefreshNode(_start);
		}

		private void RemoveSelectedNodes()
		{
			// make a copy since remove will modify the list
			var nodes = _selectedNodes
				.Where(node => node.Node != _start)
				.ToList();

			foreach (var node in nodes)
				RemoveNode(node);
		}

		#endregion

		#region Debugging

		public static void BreakpointHit(Graph graph, GraphNode node)
		{
			var window = ShowWindowForGraph(graph);
			window.GoToNode(node);
		}

		#endregion

		#region Menus

		private class SettingsMenu : PopupWindowContent
		{
			private const float _padding = 5.0f;

			private static readonly GUIContent _snapContent = new GUIContent("Snap To Grid", "Whether to snap node positions to the grid");
			private static readonly GUIContent _amountContent = new GUIContent("Snap Amount", "The increment of squares to snap to");
			private static readonly GUIContent _defaultsContent = new GUIContent("Revert to Defaults", "Reset all values to their original value");

			public override Vector2 GetWindowSize()
			{
				return new Vector2(300, RectHelper.LineHeight * 3 + 2 * _padding);
			}

			public override void OnGUI(Rect rect)
			{
				rect = RectHelper.Inset(rect, _padding);

				var snapToggleRect = RectHelper.TakeLine(ref rect);
				var snapAmountRect = RectHelper.TakeLine(ref rect);

				_snapToGrid.Value = EditorGUI.Toggle(snapToggleRect, _snapContent, _snapToGrid.Value);
				_snapAmount.Value = EditorGUI.IntSlider(snapAmountRect, _amountContent, _snapAmount.Value, 1, 25);

				var defaultsRect = RectHelper.TakeLine(ref rect);

				if (GUI.Button(defaultsRect, _defaultsContent))
					_snapAmount.Value = 1;
			}
		}

		private class NodeList
		{
			public List<Type> Types;
			public GUIContent[] Names;
		}

		private void CreateNodeTree(ref SelectionTree tree, ref NodeList nodes)
		{
			if (tree == null)
				tree = new SelectionTree();

			if (nodes == null)
				nodes = new NodeList();

			nodes.Types = TypeHelper.ListDerivedTypes<GraphNode>(false);
			nodes.Names = nodes.Types.Select(type =>
				{
					var attribute = TypeHelper.GetAttribute<CreateGraphNodeMenuAttribute>(type);
					return new GUIContent(attribute == null ? type.Name : attribute.MenuName, AssetPreview.GetMiniTypeThumbnail(type));
				})
				.ToArray();

			tree.Add("Nodes", nodes.Names);
		}

		private void CreateNodeMenu(ref GenericMenu menu)
		{
			if (menu == null)
				menu = new GenericMenu();

			menu.AddItem(new GUIContent("Create _SPACE"), false, () => _showCreateNode = true);
		}


		private void CreateViewMenu(ref GenericMenu menu, string prefix)
		{
			if (menu == null)
				menu = new GenericMenu();

			menu.AddItem(new GUIContent(prefix + "Go To Start _HOME"), false, GoToStart);
			menu.AddItem(new GUIContent(prefix + "Zoom To Selection _TAB"), false, GoToSelection);
			menu.AddItem(new GUIContent(prefix + "Show All _END"), false, ShowAll);
			menu.AddSeparator(prefix);
			menu.AddItem(new GUIContent(prefix + "Zoom In _PGUP"), false, ShowAll);
			menu.AddItem(new GUIContent(prefix + "Zoom Out _PGDN"), false, ShowAll);
		}

		private void CreateEditMenu(ref GenericMenu menu)
		{
			if (menu == null)
				menu = new GenericMenu();

			menu.AddItem(new GUIContent("Cut %x"), false, CutNodes);
			menu.AddItem(new GUIContent("Copy %c"), false, CopyNodes);
			menu.AddItem(new GUIContent("Paste %v"), false, PasteNodes);
			menu.AddItem(new GUIContent("Duplicate %d"), false, DuplicateNodes);
			menu.AddSeparator(string.Empty);
			menu.AddItem(new GUIContent("Delete _DELETE"), false, ShowAll);
		}

		private GenericMenu CreateContextMenu()
		{
			var menu = new GenericMenu();

			CreateNodeMenu(ref menu);
			menu.AddSeparator(string.Empty);
			CreateViewMenu(ref menu, "View/");
			menu.AddSeparator(string.Empty);
			CreateEditMenu(ref menu);

			return menu;
		}

		private void ShowCreateNode(Rect rect)
		{
			_showNodeId = GUIUtility.GetControlID(FocusType.Passive);
			SelectionPopup.Show(_showNodeId, rect, new SelectionState { Index = -1 }, _nodeTree);
		}

		#endregion

		#region View

		public void GoToStart()
		{
			if (_graph != null)
				GoToNode(_start);
		}

		public void GoToSelection()
		{
			if (_graph != null && _selectedNodes.Count > 0)
				ShowAll(_selectedNodes);
		}

		public void GoToNode(GraphNode node)
		{
			if (_graph != null)
			{
				//var data = GetNodeData(node);
				//GoTo(data.Bounds.center, 1.0f); // go to zoom 1 first so ViewArea is the right size
				//Pan(new Vector2(0.0f, 0.0f));
			}
		}

		private void ShowAll()
		{
			ShowAll(_nodes);
		}

		private void ShowAll(List<GraphNode.NodeData> nodes)
		{
			if (_graph != null)
			{
				var left = float.MaxValue;
				var right = float.MinValue;
				var top = float.MaxValue;
				var bottom = float.MinValue;

				foreach (var node in nodes)
				{
					//left = Math.Min(left, node.Bounds.xMin);
					//right = Math.Max(right, node.Bounds.xMax);
					//top = Math.Min(top, node.Bounds.yMin);
					//bottom = Math.Max(bottom, node.Bounds.yMax);
				}

				ShowAll(Rect.MinMaxRect(left, top, right, bottom), new RectOffset(10, 10, (int)_toolbarHeight + 10, 10));
			}
		}

		private GraphNode.NodeData GetNode(Vector2 position)
		{
			if (_graph != null)
			{
				for (var i = _nodes.Count - 1; i >= 0; --i)
				{
					var node = _nodes[i];

					//if (node.Bounds.Contains(position))
					//	return node;
				}
			}

			return null;
		}

		private GraphNode.ConnectionData GetConnection(GraphNode.NodeData node, Vector2 position)
		{
			foreach (var connection in node.Connections)
			{
				var bounds = GetOutputBounds(connection);

				if (bounds.Contains(position))
					return connection;
			}

			return null;
		}

		#endregion

		#region Drawing

		private float ToolbarBottom => _toolbarHeight;

		private static GUIStyle CreateHeaderStyle()
		{
			var style = CreateConnectionStyle();
			style.fontStyle = FontStyle.Bold;
			return style;
		}

		private static GUIStyle CreateConnectionStyle()
		{
			var style = new GUIStyle();
			style.padding.left = 5;
			style.clipping = TextClipping.Clip;
			//style.fixedWidth = InstructionGraphNode.NodeData.Width - 3 * RectHelper.IconWidth;
			style.alignment = TextAnchor.MiddleLeft;
			style.normal.textColor = new Color(0.8f, 0.8f, 0.8f, 1.0f);
			return style;
		}

		private static GUIStyle CreateCommentStyle()
		{
			var style = new GUIStyle();
			style.padding.left = 5;
			style.clipping = TextClipping.Clip;
			//style.fixedWidth = InstructionGraphNode.NodeData.Width;
			style.alignment = TextAnchor.UpperLeft;
			style.normal.textColor = new Color(0.7f, 0.7f, 0.7f, 1.0f);
			style.wordWrap = true;
			return style;
		}

		private static GUIStyle CreateWatchStyle()
		{
			var style = new GUIStyle();
			style.normal.background = EditorGUIUtility.whiteTexture;
			return style;
		}

		protected override void Draw(Rect rect)
		{
			RefreshSelectedNode();

			if (Event.current.type == EventType.Repaint)
			{
				DrawOffsetBackground(rect, _gridTexture.Content);
				DrawNodes(rect);
				DrawConnections(rect);
				DrawConnectionPreview();

				if (_mouseDragState == MouseState.Select)
					Handles.DrawSolidRectangleWithOutline(_mouseDragBounds, new Color(1.0f, 1.0f, 1.0f, 0.25f), Color.white);
			}
		}

		protected override void PostDraw(Rect rect)
		{
			DrawToolbar(rect);

			if (_toRemove != null)
			{
				RemoveNode(_toRemove);
				_toRemove = null;
			}

			if (_showContextMenu == 1 && Event.current.type == EventType.Repaint)
			{
				// When a popup menu is shown no repaint event will get sent until it is dismissed. This delays the
				// context menu until one repaint event has been processed.
				_showContextMenu = 2;
				Repaint();
			}
			else if (_showContextMenu == 2)
			{
				_contextMenu.ShowAsContext();
				_showContextMenu = 0;
			}

			if (_showCreateNode)
			{
				ShowCreateNode(new Rect(_toolbarPadding, 0, _toolbarButtonWidth, _toolbarHeight));
				_showCreateNode = false;
			}

			if (_showNodeId > 0 && SelectionPopup.HasSelection(_showNodeId))
			{
				_showNodeId = -1;
				GUI.changed = true;
				var createSelection = SelectionPopup.TakeSelection();

				if (createSelection.Index >= 0 && createSelection.Index < _nodeList.Types.Count)
				{
					var type = _nodeList.Types[createSelection.Index];
					CreateNode(type, type.Name, _createPosition);
				}
			}
		}

		private void DrawToolbar(Rect rect)
		{
			var toolbarRect = RectHelper.TakeHeight(ref rect, _toolbarHeight);
			var buttonWidth = 28;

			GUI.Box(toolbarRect, GUIContent.none, EditorStyles.toolbar);

			RectHelper.TakeWidth(ref toolbarRect, _toolbarPadding);
			RectHelper.TakeTrailingWidth(ref toolbarRect, _toolbarPadding);

			var createRect = RectHelper.TakeWidth(ref toolbarRect, _toolbarButtonWidth);
			var viewRect = RectHelper.TakeWidth(ref toolbarRect, _toolbarButtonWidth);
			var editRect = RectHelper.TakeWidth(ref toolbarRect, _toolbarButtonWidth);
			var playRect = RectHelper.TakeWidth(ref toolbarRect, buttonWidth);
			var pauseRect = RectHelper.TakeWidth(ref toolbarRect, buttonWidth);
			var stepRect = RectHelper.TakeWidth(ref toolbarRect, buttonWidth);
			var stopRect = RectHelper.TakeWidth(ref toolbarRect, buttonWidth);
			var breakRect = RectHelper.TakeWidth(ref toolbarRect, buttonWidth);

			var watchRect = RectHelper.TakeTrailingWidth(ref toolbarRect, buttonWidth);
			var lockRect = RectHelper.TakeTrailingWidth(ref toolbarRect, buttonWidth);

			var graphRect = RectHelper.TakeTrailingWidth(ref toolbarRect, _toolbarButtonWidth * 2);

			var settingsRect = RectHelper.TakeTrailingWidth(ref toolbarRect, _toolbarButtonWidth);
			var debugLoggingRect = RectHelper.TakeTrailingWidth(ref toolbarRect, buttonWidth);
			var debugBreakRect = RectHelper.TakeTrailingWidth(ref toolbarRect, buttonWidth);

			using (new EditorGUI.DisabledGroupScope(_graph == null))
			{
				var id = GUIUtility.GetControlID(FocusType.Passive);

				if (GUI.Button(createRect, _createNodeContent, EditorStyles.toolbarDropDown))
				{
					_showNodeId = id;
					_createPosition = ViewArea.center;

					SelectionPopup.Show(_showNodeId, createRect, new SelectionState { Index = -1 }, _nodeTree);
				}

				if (GUI.Button(viewRect, "View", EditorStyles.toolbarDropDown))
					_viewMenu.DropDown(new Rect(viewRect.x, viewRect.yMax, 0f, 0f));

				if (GUI.Button(editRect, "Edit", EditorStyles.toolbarDropDown))
				{
					_createPosition = ViewArea.center;
					_editMenu.DropDown(new Rect(editRect.x, editRect.yMax, 0f, 0f));
				}
			}

			var isEnabled = Application.isPlaying && _graph != null && _graph.IsRunning;
			var isPlaying = isEnabled && _graph.DebugState == Graph.PlaybackState.Running;
			var isPaused = isEnabled && _graph.DebugState == Graph.PlaybackState.Paused;
			var isStepping = isEnabled && _graph.DebugState == Graph.PlaybackState.Step;
			var isStopping = isEnabled && _graph.DebugState == Graph.PlaybackState.Stopped;

			using (new EditorGUI.DisabledScope(!isEnabled))
			{
				var playButton = isPlaying || !isEnabled ? _playDisabledButton : _playButton;
				var pauseButton = isPaused || !isEnabled ? _pauseDisabledButton : _pauseButton;
				var stepButton = isStepping || !isEnabled ? _stepDisabledButton : _stepButton;
				var stopButton = isStopping || !isEnabled ? _stopDisabledButton : _stopButton;

				var shouldPlay = GUI.Toggle(playRect, isPlaying, playButton.Content, EditorStyles.toolbarButton);
				var shouldPause = GUI.Toggle(pauseRect, isPaused, pauseButton.Content, EditorStyles.toolbarButton);
				var shouldStep = GUI.Toggle(stepRect, isStepping, stepButton.Content, EditorStyles.toolbarButton);
				var shouldStop = GUI.Toggle(stopRect, isStopping, stopButton.Content, EditorStyles.toolbarButton);

				if (isEnabled)
				{
					if (shouldPlay != isPlaying) _graph.DebugPlay();
					if (shouldPause != isPaused) _graph.DebugPause();
					if (shouldStep != isStepping) _graph.DebugStep();
					if (shouldStop != isStopping) _graph.DebugStop();
				}
			}

			var canBreak = _selectedNodes.Count == 1 && _selectedNodes[0].Node != _start && !(_selectedNodes[0].Node is CommentNode);
			var hasBreak = canBreak && _selectedNodes[0].Node.IsBreakpoint;

			using (new EditorGUI.DisabledScope(!canBreak))
			{
				var breakpointButton = canBreak ? (hasBreak ? _removeBreakpointButton : _addBreakpointButton) : _breakpointDisabledButton;
				hasBreak = GUI.Toggle(breakRect, hasBreak, breakpointButton.Content, EditorStyles.toolbarButton);

				if (canBreak)
					_selectedNodes[0].Node.IsBreakpoint = hasBreak;
			}

			Graph.IsDebugBreakEnabled = GUI.Toggle(debugBreakRect, Graph.IsDebugBreakEnabled, Graph.IsDebugBreakEnabled ? _disableBreakpointsButton.Content : _enableBreakpointsButton.Content, EditorStyles.toolbarButton);
			Graph.IsDebugLoggingEnabled = GUI.Toggle(debugLoggingRect, Graph.IsDebugLoggingEnabled, Graph.IsDebugLoggingEnabled ? _disableLoggingButton.Content : _enableLoggingButton.Content, EditorStyles.toolbarButton);

			_breakpointsEnabled.Value = Graph.IsDebugBreakEnabled;
			_loggingEnabled.Value = Graph.IsDebugLoggingEnabled;

			if (GUI.Button(settingsRect, "Settings", EditorStyles.toolbarDropDown))
				PopupWindow.Show(new Rect(settingsRect.x, settingsRect.yMax, 0f, 0f), _settingsMenu);

			var graphList = AssetHelper.GetAssetList<Graph>();
			var graphIndex = graphList.Assets.IndexOf(_graph);
			var graphSelection = SelectionPopup.Draw(graphRect, new GUIContent(_graph ? _graph.name : "No Graph Selected", "Select or create a new Instruction Graph"), EditorStyles.toolbarDropDown, new SelectionState { Index = graphIndex, Tab = 0 }, null);

			if (graphSelection.Tab == 0)
			{
				if (graphSelection.Index != graphIndex)
				{
					var graph = graphList.Assets[graphSelection.Index] as Graph;
					SetGraph(graph);
				}
			}

			_isLocked = GUI.Toggle(lockRect, _isLocked, _isLocked ? _unlockButton.Content : _lockButton.Content, EditorStyles.toolbarButton);

			//if (GUI.Button(watchRect, _openWatchButton.Content, EditorStyles.toolbarButton))
			//	WatchWindow.ShowWindow();
		}
		

		private void DrawNodes(Rect rect)
		{
			foreach (var node in _nodes)
			{
				//if (rect.Overlaps(node.Bounds))
				//	DrawNode(node);
			}
		}

		private int GetNodeIteration(GraphNode.NodeData node)
		{
			if (Application.isPlaying && _graph != null && _graph.IsRunning)
			{
				if (node.Node == _start)
					return 0;

				return _graph.IsInCallStack(node.Node);
			}

			return -1;
		}

		private void DrawNode(GraphNode.NodeData node)
		{
			var isComment = node.Node is CommentNode;

			var rect = new Rect();

			var outlineRect = rect;
			var headerRect = TakeHeaderHeight(ref rect);
			var inputRect = GetInputBounds(node);
			var deleteRect = GetInteractionBounds(node, 0);
			var headerColor = node.Node.NodeColor;
			var nodeColor = isComment ? _commentColor : _nodeColor;

			var labelRect = RectHelper.AdjustHeight(headerRect, EditorGUIUtility.singleLineHeight, RectVerticalAlignment.Middle);
			var inputIconRect = RectHelper.Adjust(inputRect, _inputButton.Content.image.width, _inputButton.Content.image.height, RectHorizontalAlignment.Center, RectVerticalAlignment.Middle);
			var deleteIconRect = RectHelper.Adjust(deleteRect, _removeButton.Content.image.width, _removeButton.Content.image.height, RectHorizontalAlignment.Center, RectVerticalAlignment.Middle);

			if (!isComment && node.Node != _start)
				RectHelper.TakeWidth(ref labelRect, RectHelper.IconWidth);

			RectHelper.TakeTrailingWidth(ref labelRect, headerRect.height);

			if (isComment)
			{
				EditorGUI.DrawRect(outlineRect, _commentColor);
			}
			else
			{
				EditorGUI.DrawRect(headerRect, headerColor);
				EditorGUI.DrawRect(rect, nodeColor);
			}

			if (node.Node != _start && !isComment)
				EditorGUI.LabelField(inputIconRect, _inputButton.Content, GUIStyle.none);

			var iteration = GetNodeIteration(node);
			var nodeLabel = iteration > 0
				? string.Format("{0} ({1})", node.Node.Name, iteration)
				: node.Node.Name;

			EditorGUI.LabelField(labelRect, nodeLabel, _headerStyle.Content);

			if (node.Node != _start)
			{
				EditorGUI.LabelField(deleteIconRect, _removeButton.Content, GUIStyle.none);

				if (_hoveredInteraction == node)
					Handles.DrawSolidRectangleWithOutline(deleteRect, Color.clear, _hoveredColor);

				if (_selectedInteraction == node)
					Handles.DrawSolidRectangleWithOutline(deleteRect, Color.clear, _selectedColor);
			}

			if (isComment)
			{
				//EditorGUI.LabelField(commentRect, commentNode.Comment, _commentStyle.Content);
			}
			else
			{
				foreach (var connection in node.Connections)
				{
					var outputRect = GetOutputBounds(connection);
					var outputIconRect = RectHelper.Adjust(outputRect, _outputButton.Content.image.width, _outputButton.Content.image.height, RectHorizontalAlignment.Center, RectVerticalAlignment.Middle);
					//var outputLabelRect = RectHelper.TakeHeight(ref rect, InstructionGraphNode.NodeData.LineHeight);

					// tooltip positioning is pretty messed up with zoom so not showing them for now
					var label = new GUIContent(ObjectNames.NicifyVariableName(connection.Name));//, Label.GetTooltip(connection.From.GetType(), connection.Field));

					//EditorGUI.LabelField(outputLabelRect, label, _connectionStyle.Content);
					EditorGUI.LabelField(outputIconRect, _outputButton.Content, GUIStyle.none);

					if (connection == _hoveredOutput)
						Handles.DrawSolidRectangleWithOutline(outputRect, Color.clear, _hoveredColor);

					if (_selectedConnections.Contains(connection))
						Handles.DrawSolidRectangleWithOutline(outputRect, Color.clear, _selectedColor);
				}
			}

			var selected = _selectedNodes.Contains(node);

			if (_hoveredInput == node)
				Handles.DrawSolidRectangleWithOutline(inputRect, Color.clear, _hoveredColor);

			if (iteration >= 0)
			{
				var executing = _graph.IsExecuting(node.Node);
				var paused = _graph.DebugState == Graph.PlaybackState.Paused;

				var color = executing ? (paused ? _breakColor : _activeColor) : _callstackColor;
				Handles.DrawSolidRectangleWithOutline(outlineRect, Color.clear, color);
			}

			if (selected)
				Handles.DrawSolidRectangleWithOutline(outlineRect, Color.clear, _selectedColor);

			// only draw the hover when not the node is not selected unless it's for a pending connection
			if (_pendingInput == node || (!selected && (_hoveredNode == node || _pendingNodes.Contains(node))))
				Handles.DrawSolidRectangleWithOutline(outlineRect, Color.clear, _hoveredColor);
		}

		private Rect Outset(Rect rect, float amount)
		{
			rect.x -= amount;
			rect.y -= amount;
			rect.width += amount * 2.0f;
			rect.height += amount * 2.0f;

			return rect;
		}

		private void DrawConnections(Rect rect)
		{
			foreach (var node in _nodes)
			{
				foreach (var connection in node.Connections)
				{
					if (connection.Target != null)
						DrawConnection(connection, connection.Target);
				}
			}
		}

		private void DrawConnectionPreview()
		{
			foreach (var connection in _selectedConnections)
			{
				if (_pendingInput != null)
				{
					DrawConnection(connection, _pendingInput);
				}
				else
				{
					var outputBounds = GetOutputBounds(connection);
					var start = outputBounds.center;
			
					HandleHelper.DrawBezier(start, Event.current.mousePosition, _knobColor);
					HandleHelper.DrawCircle(start, _knobRadius, _knobColor);
				}
			}
		}

		private void DrawConnection(GraphNode.ConnectionData from, GraphNode.NodeData to)
		{
			var outputBounds = GetOutputBounds(from);
			var inputBounds = GetInputBounds(to);

			var start = outputBounds.center;
			var end = inputBounds.center;

			var startColor = _knobColor;
			var lineColor = _knobColor;
			var endColor = _knobColor;

			if (Application.isPlaying && _graph.IsInCallStack(to.Node, from.Name))
			{
				startColor = _callstackColor;
				lineColor = _callstackColor;
				endColor = _callstackColor; // breakpoint color takes precedence
			}
			else if (_selectedNodes.Contains(GetNodeData(from.From)))
			{
				lineColor = _selectedColor; // callstack color takes precedence
			}

			if (to.Node.IsBreakpoint)
				endColor = Graph.IsDebugBreakEnabled ? _breakColor : _disabledBreakColor;

			if (outputBounds.xMax > inputBounds.xMin)
			{
				var difference = (end.y - start.y) / 3;
				var magnitude = 200.0f;

				Handles.DrawBezier(start, end, start + new Vector2(magnitude, difference), end - new Vector2(magnitude, difference), lineColor, null, 3);
			}
			else
			{
				HandleHelper.DrawBezier(start, end, lineColor);
			}

			HandleHelper.DrawCircle(start, _knobRadius, startColor);
			HandleHelper.DrawCircle(end, _knobRadius, endColor);
		}

		#endregion

		#region Copy and Paste

		private static List<GraphNode.NodeData> _copiedNodes = new List<GraphNode.NodeData>();

		private void CutNodes()
		{
			CopyNodes();
			RemoveSelectedNodes();
		}

		private void CopyNodes()
		{
			if (_graph != null && _selectedNodes.Count > 0)
			{
				_copiedNodes.Clear();

				var sourceNodes = _selectedNodes.Select(node => node.Node).Where(node => node != _start).ToList();

				foreach (var node in sourceNodes)
				{
					var copy = GraphEditor.CloneNode(node);
					var data = new GraphNode.NodeData(copy);
					copy.GetConnections(data);
					_copiedNodes.Add(data);
				}

				foreach (var copy in _copiedNodes)
				{
					foreach (var connection in copy.Connections)
					{
						var index = connection.To != null ? sourceNodes.IndexOf(connection.To) : -1;
						connection.ChangeTarget(index >= 0 ? _copiedNodes[index] : null);
					}
				}
			}
		}

		private void PasteNodes()
		{
			if (_graph != null && _copiedNodes.Count > 0)
			{
				var pastedNodes = new List<GraphNode.NodeData>();

				foreach (var node in _copiedNodes)
				{
					_nodes.Add(node);
					pastedNodes.Add(node);
					SetupOutputConnections(node);
				}

				GraphEditor.AddClonedNodes(_graph, _copiedNodes, _createPosition);
				TransferSelection(ref pastedNodes);
				CopyNodes(); // re-copy so the same set of nodes can be pasted twice
			}
		}

		private void DuplicateNodes()
		{
			CopyNodes();
			PasteNodes();
		}

		#endregion

		#region Input

		private enum MouseState
		{
			Hover,
			Select,
			Move,
			Connect,
			Interact
		}

		private void SetupInput(InputManager input)
		{
			wantsMouseMove = true;

			SetNoHover();

			SetupSelection(input);
			SetupHotkeys(input);
			SetupContextMenu(input);
		}

		protected override bool IsMouseInViewport()
		{
			var mouse = ViewportToWindow(Event.current.mousePosition);

			if (mouse.y <= ToolbarBottom)
				return false;

			return true;
		}

		private void SetupSelection(InputManager input)
		{
			input.Create<InputManager.EventTrigger>()
				.SetEvent(EventType.MouseMove)
				.AddCondition(IsMouseInViewport)
				.AddAction(UpdateHover);

			input.Create<InputManager.MouseTrigger>()
				.SetEvent(EventType.MouseDown, InputManager.MouseButton.Left)
				.AddCondition(IsMouseInViewport)
				.AddAction(StartSelection);

			input.Create<InputManager.MouseTrigger>()
				.SetEvent(EventType.MouseDrag, InputManager.MouseButton.Left)
				.AddAction(DragSelection);

			input.Create<InputManager.MouseTrigger>()
				.SetRawEvent(EventType.MouseUp, InputManager.MouseButton.Left)
				.AddAction(EndSelection);
		}

		private void SetupHotkeys(InputManager input)
		{
			input.Create<InputManager.KeyboardTrigger>()
				.SetEvent(EventType.KeyDown, KeyCode.Delete)
				.AddAction(RemoveSelectedNodes);

			input.Create<InputManager.KeyboardTrigger>()
				.SetEvent(EventType.KeyDown, KeyCode.Home)
				.AddAction(GoToStart);

			input.Create<InputManager.KeyboardTrigger>()
				.SetEvent(EventType.KeyDown, KeyCode.Tab)
				.AddAction(GoToSelection);

			input.Create<InputManager.KeyboardTrigger>()
				.SetEvent(EventType.KeyDown, KeyCode.End)
				.AddAction(ShowAll);

			input.Create<InputManager.KeyboardTrigger>()
				.SetEvent(EventType.KeyDown, KeyCode.PageUp)
				.AddAction(() => Zoom(ScrollWheelZoomAmount, ViewArea.center));

			input.Create<InputManager.KeyboardTrigger>()
				.SetEvent(EventType.KeyDown, KeyCode.PageDown)
				.AddAction(() => Zoom(-ScrollWheelZoomAmount, ViewArea.center));

			input.Create<InputManager.KeyboardTrigger>()
				.SetEvent(EventType.KeyDown, KeyCode.X, control: true)
				.AddAction(CutNodes);

			input.Create<InputManager.KeyboardTrigger>()
				.SetEvent(EventType.KeyDown, KeyCode.C, control: true)
				.AddAction(CopyNodes);

			input.Create<InputManager.KeyboardTrigger>()
				.SetEvent(EventType.KeyDown, KeyCode.V, control: true)
				.AddAction(() => { _createPosition = ViewArea.center; PasteNodes(); });

			input.Create<InputManager.KeyboardTrigger>()
				.SetEvent(EventType.KeyDown, KeyCode.D, control: true)
				.AddAction(() => { _createPosition = ViewArea.center; DuplicateNodes(); });

			input.Create<InputManager.KeyboardTrigger>()
				.SetEvent(EventType.KeyDown, KeyCode.Space)
				.AddAction(() => { _createPosition = ViewArea.center; _showCreateNode = true; });
		}

		private void SetupContextMenu(InputManager input)
		{
			var wasMouseDragged = 0;

			input.Create<InputManager.EventTrigger>()
				.SetEvent(EventType.MouseDown)
				.AddAction(() => wasMouseDragged = 0);

			input.Create<InputManager.EventTrigger>()
				.SetEvent(EventType.MouseDrag)
				.AddAction(() => wasMouseDragged++);

			input.Create<InputManager.MouseTrigger>()
				.SetEvent(EventType.MouseDown, InputManager.MouseButton.Right)
				.AddCondition(IsMouseInViewport)
				.AddAction(() => _createPosition = Event.current.mousePosition);

			input.Create<InputManager.MouseTrigger>()
				.SetEvent(EventType.MouseUp, InputManager.MouseButton.Right)
				.AddCondition(IsMouseInViewport)
				.AddCondition(() => _graph != null && wasMouseDragged < 4 && !_simulateDrag)
				.AddAction(PrepareContextMenu);
		}

		private void PrepareContextMenu()
		{
			if (_hoveredNode != null && !_selectedNodes.Contains(_hoveredNode))
			{
				_showContextMenu = 1;

				SetSelection(_hoveredNode);
				Repaint();
			}
			else
			{
				_showContextMenu = 2;
			}
		}

		#endregion

		#region Mouse Hovering

		private void UpdateHover()
		{
			if (_simulateDrag)
			{
				UpdateDrag(false);
			}
			else
			{
				var node = GetNode(Event.current.mousePosition);

				if (node != null)
				{
					var connection = GetConnection(node, Event.current.mousePosition);

					if (connection != null)
						SetHoveredOutput(connection);
					else if (GetInputBounds(node).Contains(Event.current.mousePosition) && HasInput(node) && !(node.Node is CommentNode))
						SetHoveredInput(node);
					else if (node.Node != _start && GetInteractionBounds(node, 0).Contains(Event.current.mousePosition))
						SetHoveredInteraction(node);
					else
						SetHoveredNode(node);
				}
				else
				{
					SetNoHover();
				}
			}
		}

		private bool HasInput(GraphNode.NodeData node)
		{
			foreach (var outputNode in _nodes)
			{
				foreach (var outputConnection in outputNode.Connections)
				{
					if (outputConnection.Target == node)
						return true;
				}
			}

			return false;
		}

		private void ResetHover()
		{
			_hoveredNode = null;
			_hoveredOutput = null;
			_hoveredInput = null;
			_hoveredInteraction = null;
		}

		private void SetNoHover()
		{
			if (_mouseMoveState != MouseState.Select)
			{
				ResetHover();
				_mouseMoveState = MouseState.Select;
				Repaint();
			}
		}

		private void SetHoveredNode(GraphNode.NodeData node)
		{
			if (_mouseMoveState != MouseState.Move || _hoveredNode != node)
			{
				ResetHover();
				_mouseMoveState = MouseState.Move;
				_hoveredNode = node;
				Repaint();
			}
		}

		private void SetHoveredOutput(GraphNode.ConnectionData connection)
		{
			if (_mouseMoveState != MouseState.Connect || _hoveredOutput != connection)
			{
				ResetHover();
				_mouseMoveState = MouseState.Connect;
				_hoveredOutput = connection;
				Repaint();
			}
		}

		private void SetHoveredInput(GraphNode.NodeData node)
		{
			if (_mouseMoveState != MouseState.Connect || _hoveredInput != node)
			{
				ResetHover();
				_mouseMoveState = MouseState.Connect;
				_hoveredInput = node;
				Repaint();
			}
		}

		private void SetHoveredInteraction(GraphNode.NodeData interaction)
		{
			if (_mouseMoveState != MouseState.Interact || _hoveredInteraction != interaction)
			{
				ResetHover();
				_mouseMoveState = MouseState.Interact;
				_hoveredInteraction = interaction;
				Repaint();
			}
		}

		#endregion

		#region Mouse Selection

		private void StartSelection()
		{
			_mouseDragState = _mouseMoveState;

			switch (_mouseDragState)
			{
				case MouseState.Select:
				{
					ClearSelection();
					_mouseDragStart = Event.current.mousePosition;
					_mouseDragBounds = new Rect(_mouseDragStart, Vector2.zero);
					break;
				}
				case MouseState.Move:
				{
					if (_hoveredNode != null && !_selectedNodes.Contains(_hoveredNode))
						SetSelection(_hoveredNode);

					break;
				}
				case MouseState.Connect:
				{
					_simulateDrag = !_simulateDrag;

					if (_simulateDrag)
					{
						if (_hoveredInput != null)
						{
							foreach (var outputNode in _nodes)
							{
								foreach (var outputConnection in outputNode.Connections)
								{
									if (outputConnection.Target == _hoveredInput)
										_pendingOutputs.Add(outputConnection);
								}
							}

							TransferSelection(ref _pendingOutputs);
							_pendingInput = _hoveredInput;
						}
						else if (_hoveredOutput != null)
						{
							SetSelection(_hoveredOutput);
						}
					}

					break;
				}
				case MouseState.Interact:
				{
					if (_hoveredInteraction != null)
					{
						_selectedInteraction = _hoveredInteraction;
						Repaint();
					}

					break;
				}
			}

			ResetHover();
		}

		private void DragSelection()
		{
			UpdateDrag(true);
		}

		private void UpdateDrag(bool dragging)
		{
			switch (_mouseDragState)
			{
				case MouseState.Select:
				{
					var left = Math.Min(_mouseDragStart.x, Event.current.mousePosition.x);
					var top = Math.Min(_mouseDragStart.y, Event.current.mousePosition.y);
					var right = Math.Max(_mouseDragStart.x, Event.current.mousePosition.x);
					var bottom = Math.Max(_mouseDragStart.y, Event.current.mousePosition.y);

					_mouseDragBounds.Set(left, top, right - left, bottom - top);
					_pendingNodes.Clear();


					break;
				}
				case MouseState.Move:
				{

					break;
				}
				case MouseState.Connect:
				{
					var node = GetNode(Event.current.mousePosition);
					var canConnect = node != null && node.Node != _start && !(node.Node is CommentNode) && !_selectedConnections.Select(connection => connection.From).Contains(node.Node);

					if (dragging)
						_simulateDrag = false;

					_pendingInput = canConnect ? node : null;

					break;
				}
				case MouseState.Interact:
				{
					_selectedInteraction = null;
					break;
				}
			}

			Repaint();
		}

		private void EndSelection()
		{
			if (_simulateDrag)
				return;

			switch (_mouseDragState)
			{
				case MouseState.Select:
				{
					TransferSelection(ref _pendingNodes);
					Repaint();
					break;
				}
				case MouseState.Move:
				{
					break;
				}
				case MouseState.Connect:
				{
					if (_pendingInput != null)
					{
						foreach (var connection in _selectedConnections)
							SetConnection(connection, _pendingInput);

						_pendingInput = null;
					}

					_selectedConnections.Clear();
					Repaint();

					break;
				}
				case MouseState.Interact:
				{
					if (_selectedInteraction != null)
					{
						_toRemove = _selectedInteraction;
						_selectedInteraction = null;
						Repaint();
					}

					break;
				}
			}

			_mouseDragState = MouseState.Hover;
			UpdateHover();
		}

		private void ClearSelection()
		{
			if (_selectedNodes.Count > 0 || _selectedConnections.Count > 0)
			{
				_selectedNodes.Clear();
				_selectedConnections.Clear();
				Repaint();
			}
		}

		private void SetSelection(GraphNode.NodeData node)
		{
			if (_selectedNodes.Count != 1 || _selectedNodes[0] != node)
			{
				_selectedNodes.Clear();
				_selectedNodes.Add(node);
				ApplySelection();
			}
		}

		private void TransferSelection(ref List<GraphNode.NodeData> nodes)
		{
			var selected = _selectedNodes;
			_selectedNodes = nodes;
			nodes = selected;

			selected.Clear();
			ApplySelection();
		}

		private void SetSelection(GraphNode.ConnectionData connection)
		{
			if (_selectedConnections.Count != 1 || _selectedConnections[0] != connection)
			{
				_selectedConnections.Add(connection);
				SetConnection(connection, null);
				Repaint();
			}
		}

		private void TransferSelection(ref List<GraphNode.ConnectionData> connections)
		{
			var selected = _selectedConnections;
			_selectedConnections = connections;
			connections = selected;

			foreach (var connection in _selectedConnections)
				SetConnection(connection, null);

			selected.Clear();
			Repaint();
		}

		#endregion
	}
}
