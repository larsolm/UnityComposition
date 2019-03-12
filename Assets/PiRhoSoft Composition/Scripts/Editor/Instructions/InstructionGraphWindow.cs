using PiRhoSoft.CompositionEngine;
using PiRhoSoft.SnippetsEditor;
using PiRhoSoft.UtilityEditor;
using PiRhoSoft.UtilityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using MenuItem = UnityEditor.MenuItem;

namespace PiRhoSoft.CompositionEditor
{
	[InitializeOnLoad]
	public class InstructionGraphWindow : ViewportWindow
	{
		// inspiration and parts of the implementation for this class have been adapted from the MIT licensed Unity
		// Node Editor Base Extension: https://unitylist.com/p/tb/Unity-Node-Editor-Base

		private static readonly Base64Texture _gridTexture = new Base64Texture("iVBORw0KGgoAAAANSUhEUgAAAEAAAABACAIAAAAlC+aJAAAABGdBTUEAALGPC/xhBQAAAAlwSFlzAAAOwgAADsIBFShKgAAAABl0RVh0U29mdHdhcmUAcGFpbnQubmV0IDQuMC4xNzNun2MAAAKnSURBVGhD7ZnbbiJBDETzNwTCfbm/Av//S3uGKkXTETYbCS2zG5+HpNsu3F3tYWaUvH2kTCaT1Wq13W4ZgKMtSv26obETLYpTarlcRhogNZ1Od7vdbDZLZOPxeL1eX6/XMlAGvkCqDPSgRBkwipcBUwZ6KPUyA4gS0LHeZrNhAI62KEU50NiJFsUptVgsIg2QYn/4xEYie39/52Q7A/zKYUnOw5MYlgRPYihFozyJQcZxeHIPjhXN5XJ508IJ6MCTmD+XeZTyUMaxHg6HzoBbEkATh34JORBAiaEb6L7SMUjRUU4fc7RFqZfdhRwIoEQZMIqXAVMGeihVBgyp7xlAlICO9epB1qH48w2o9QnUoqGexCADT2Io9VCGSWT89PweHNZ+v+/ehRwIQMd6lLud79rRFqVu+99q7ESL4tpZpIFP2ef4Lhy/DbglATSxvgNG8ecbQJ2AFB3l9DFHW5RSZzV2okVxSj35NupAACXKgFG8DJgy0EOpMmBIfc8AogR0rFfPgQ7Fn9uB1/xh66FMGvD8HnTyeDx2BjiPBNrNufIBBuBoi1JU1OUBTrQoTikKRhpQit3T+UQ2n88p9V+8zOmSikA69LuQAwGUKANG8TJgykAPpcqAIfXDDCBKQMd6g36QoUugljpws/3haItS6oDGTrQorg5EGiDF2dMBXhYSGR5YrnuVQPovwkH4ZQ67OVwYqD2JUQc8iaEULfUkgLNni1xpnt+DJrgDuqQiuAqH/h1AnYB06HchBwIoUQaM4mXAlIEeSpUBQ+qHGWCUwPOC5w47YwCOtiiFT9DYiRbFKcWDNtIAKfaNT3aZyEajEQfRGcBHAnviMHjr0P4cbVFqf0NjJ1oUpxSbizSg1Ol0wmoiY/f+Jx8mchCBJzF/X4bgfD7/Bp0ChIMH9TUUAAAAAElFTkSuQmCC");
		private static readonly Base64Texture _windowIcon = new Base64Texture("iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAABGdBTUEAALGPC/xhBQAAACBjSFJNAAB6JgAAgIQAAPoAAACA6AAAdTAAAOpgAAA6mAAAF3CculE8AAAACXBIWXMAABJ0AAASdAHeZh94AAAAB3RJTUUH4wEOBR4Wp+XVagAAABh0RVh0U29mdHdhcmUAcGFpbnQubmV0IDQuMS41ZEdYUgAAATdJREFUOE+lkUtuglAYhWnnfQwITUdEQF6CPFRimBqcdKCzdl3GCZg0cS8O3IeJ2NA90HNSBsqlsYkkPx+H++VcbpDqur5pfm+SdJem6UOSJI/Xhh79i4LRaPQ0Ho8PmNNkMqnAsqGQMQf6FwVBEDzj5ck0zfcwDD/7/f4badv2x3nmOj36QkEcx5XneVs878FNQyHToy8URFFUYsdsOByuDcOYksjzVs7odRZg+AU5xJ3v+ysSuWjlnF5nAYQSZ1wOBoMCnDUUMr3OAuxSOY6zhbgHNw2FTE8oUFWVBWWv18sgrZGnJM4+P89cp0dfKMD5+AW567o7cNWwaOWcXmcBFktd1xeWZRWaps1I5GUrL+j9VXDEb/oCv8FTw658FApw3SuK8iLL8uu1oUf/ouCW6Xz5/6mlH0LCqCZdcm2YAAAAAElFTkSuQmCC");

		private static readonly string _pauseIcon = "iVBORw0KGgoAAAANSUhEUgAAABEAAAAOCAYAAADJ7fe0AAAABGdBTUEAALGPC/xhBQAAACBjSFJNAAB6JgAAgIQAAPoAAACA6AAAdTAAAOpgAAA6mAAAF3CculE8AAAACXBIWXMAAA7CAAAOwgEVKEqAAAAAB3RJTUUH4wEOBR0hNHUjpgAAABh0RVh0U29mdHdhcmUAcGFpbnQubmV0IDQuMS41ZEdYUgAAAGVJREFUOE+l0bERwCAMQ1EvxACswWhZ2LHuKKSLKByKV+gfrojMvGZjl41dnzDnHOXZxqkxGVCPVsltnRqTAe7ANSYD3IFrTAa4A9eYDHAHrjEZ4A5cYzKgHt1/8R82dtnYERHxAqpHVWhCcdFJAAAAAElFTkSuQmCC";
		private static readonly string _stopIcon = "iVBORw0KGgoAAAANSUhEUgAAABEAAAAOCAYAAADJ7fe0AAAABGdBTUEAALGPC/xhBQAAACBjSFJNAAB6JgAAgIQAAPoAAACA6AAAdTAAAOpgAAA6mAAAF3CculE8AAAACXBIWXMAAA7CAAAOwgEVKEqAAAAAB3RJTUUH4wEOBR0hNHUjpgAAABh0RVh0U29mdHdhcmUAcGFpbnQubmV0IDQuMS41ZEdYUgAAAE9JREFUOE/t0bENACAIBMBfiAFYg9FcGKEUiBFtLa7gw9MAVX1Whl1l2JUCZiYzNih2lsHZkhjdkNhZBudLoRT9I9nRkfcX3yjDrjLsAIAJNpdTpphdxWYAAAAASUVORK5CYII=";

		private static readonly GUIContent _titleLabel = new GUIContent("Instruction Graph");
		private static readonly StaticStyle _headerStyle = new StaticStyle(CreateHeaderStyle);
		private static readonly StaticStyle _connectionStyle = new StaticStyle(CreateConnectionStyle);
		private static readonly StaticStyle _commentStyle = new StaticStyle(CreateCommentStyle);
		private static readonly StaticStyle _watchStyle = new StaticStyle(CreateWatchStyle);
		private static readonly IconButton _outputButton = new IconButton("sv_icon_dot0_sml");//, "Click and drag to make a connection from this output");
		private static readonly IconButton _inputButton = new IconButton("sv_icon_dot0_sml");//, "Drag an output to here to make a connection");
		private static readonly IconButton _removeButton = new IconButton("d_Toolbar Minus");//, "Remove this node");
		private static readonly IconButton _playButton = new IconButton("Animation.Play", "Resume graph execution");
		private static readonly IconButton _playDisabledButton = new IconButton("Animation.Play");
		private static readonly Base64Button _pauseButton = new Base64Button(_pauseIcon, "Pause graph execution before running the next node");
		private static readonly Base64Button _pauseDisabledButton = new Base64Button(_pauseIcon);
		private static readonly IconButton _stepButton = new IconButton("Animation.NextKey", "Run the next node then pause again");
		private static readonly IconButton _stepDisabledButton = new IconButton("Animation.NextKey");
		private static readonly Base64Button _stopButton = new Base64Button(_stopIcon, "Halt execution of the current branch of the graph and continue with the next branch");
		private static readonly Base64Button _stopDisabledButton = new Base64Button(_stopIcon);
		private static readonly IconButton _addBreakpointButton = new IconButton("Animation.Record", "Set a breakpoint on this node");
		private static readonly IconButton _removeBreakpointButton = new IconButton("Animation.Record", "Remove the breakpoint from this node");
		private static readonly IconButton _breakpointDisabledButton = new IconButton("Animation.Record");

		private static readonly IconButton _enableBreakpointsButton = new IconButton("Animation.Record", "Enable node breakpoints for all graphs");
		private static readonly IconButton _disableBreakpointsButton = new IconButton("Animation.Record", "Disable node breakpoins for all graphs");
		private static readonly IconButton _enableLoggingButton = new IconButton("UnityEditor.ConsoleWindow", "Enable logging of graph execution for all graphs");
		private static readonly IconButton _disableLoggingButton = new IconButton("UnityEditor.ConsoleWindow", "Disable logging of graph execution for all graphs");

		private static readonly IconButton _lockButton = new IconButton("AssemblyLock", "Lock this window so it won't be used when other graphs are opened");
		private static readonly IconButton _unlockButton = new IconButton("AssemblyLock", "Unlock this window so it can be used to show other graphs");
		private static readonly IconButton _disabledWatchButton = new IconButton("UnityEditor.LookDevView");
		private static readonly IconButton _openWatchButton = new IconButton("UnityEditor.LookDevView", "Open the variables panel");
		private static readonly IconButton _closeWatchButton = new IconButton("UnityEditor.LookDevView", "Close the variables panel");

		private const float _knobRadius = 6.0f;
		private const float _toolbarPadding = 17.0f;
		private const float _toolbarHeight = 17.0f;
		private const float _watchWidth = 300.0f;
		private const float _toolbarButtonWidth = 60.0f;
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
		private GenericMenu _nodeMenu;
		private GenericMenu _editMenu;
		private GenericMenu _contextMenu;
		private SettingsMenu _settingsMenu;

		private InstructionGraph _graph = null;
		private StartNode _start = null;
		private List<InstructionGraphNode.NodeData> _nodes = new List<InstructionGraphNode.NodeData>();

		private Vector2 _createPosition;
		private InstructionGraphNode.NodeData _toRemove = null;
		private int _showContextMenu = 0;

		private bool _isLocked = false;
		private bool _isWatchOpen = false;
		private Vector2 _watchScrollPosition;

		private MouseState _mouseMoveState = MouseState.Select;
		private MouseState _mouseDragState = MouseState.Hover;
		private bool _simulateDrag = false;
		private Vector2 _mouseDragOffset;
		private Vector2 _mouseDragStart;
		private Rect _mouseDragBounds;

		private InstructionGraphNode.NodeData _hoveredNode;
		private InstructionGraphNode.ConnectionData _hoveredOutput;
		private InstructionGraphNode.NodeData _hoveredInput;
		private InstructionGraphNode.NodeData _hoveredInteraction;

		private List<InstructionGraphNode.NodeData> _pendingNodes = new List<InstructionGraphNode.NodeData>();
		private List<InstructionGraphNode.ConnectionData> _pendingOutputs = new List<InstructionGraphNode.ConnectionData>();
		private InstructionGraphNode.NodeData _pendingInput;

		private List<InstructionGraphNode.NodeData> _selectedNodes = new List<InstructionGraphNode.NodeData>();
		private List<InstructionGraphNode.ConnectionData> _selectedConnections = new List<InstructionGraphNode.ConnectionData>();
		private InstructionGraphNode.NodeData _selectedInteraction;

		private InstructionGraph _watching;
		private VariableStoreControl _thisStore;
		private VariableStoreControl _inputStore;
		private VariableStoreControl _outputStore;
		private VariableStoreControl _localStore;
		private VariableStoreControl _globalStore;
		private VariableStoreControl _selectedStore;

		#region Window Access

		public static InstructionGraphWindow FindWindowForGraph(InstructionGraph graph)
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

		public static InstructionGraphWindow ShowWindowForGraph(InstructionGraph graph)
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

		[MenuItem("Window/PiRho Soft/Instruction Graph")]
		public static InstructionGraphWindow ShowNewWindow()
		{
			var window = CreateInstance<InstructionGraphWindow>();
			window.titleContent = _titleLabel;
			window.Show();
			return window;
		}

		[OnOpenAsset]
		static bool OpenAsset(int instanceID, int line)
		{
			var graph = EditorUtility.InstanceIDToObject(instanceID) as InstructionGraph;

			if (graph != null)
			{
				ShowWindowForGraph(graph);
				return true;
			}

			return false;
		}

		#endregion

		#region Setup

		static InstructionGraphWindow()
		{
			InstructionGraph.OnBreakpointHit += BreakpointHit;
		}

		protected override void Setup(InputManager input)
		{
			base.Setup(input);

			titleContent.image = _windowIcon.Texture;

			CreateViewMenu(ref _viewMenu, string.Empty);
			CreateNodeMenu(ref _nodeMenu, string.Empty);
			CreateEditMenu(ref _editMenu, string.Empty);

			_contextMenu = CreateContextMenu();
			_settingsMenu = new SettingsMenu();

			SetupInput(input);
			SetupNodes();

			autoRepaintOnSceneChange = true;
			Undo.undoRedoPerformed += RebuildNodes;
			Selection.selectionChanged += UpdateSelection;
			EditorApplication.playModeStateChanged += PlayModeChanged;

			// this has to be here because Unity doesn't allow EditorPrefs access in a static constructor
			InstructionGraph.IsDebugBreakEnabled = _breakpointsEnabled.Value;
			InstructionGraph.IsDebugLoggingEnabled = _loggingEnabled.Value;
		}

		protected override void Teardown()
		{
			EditorApplication.playModeStateChanged -= PlayModeChanged;
			Selection.selectionChanged -= UpdateSelection;
			Undo.undoRedoPerformed -= RebuildNodes;

			TeardownNodes();

			_viewMenu = null;
			_nodeMenu = null;
			_editMenu = null;
			_contextMenu = null;
			_settingsMenu = null;

			base.Teardown();
		}

		private void PlayModeChanged(PlayModeStateChange state)
		{
			RebuildNodes();

			if (state == PlayModeStateChange.EnteredEditMode)
				TeardownWatch();
		}

		private void RebuildNodes()
		{
			TeardownNodes();
			SetupNodes();
			UpdateSelection();
			Repaint();
		}

		void OnInspectorUpdate()
		{
			if (IsWatchOpen)
				Repaint();
		}

		#endregion

		#region Graph Management

		private class StartNode : InstructionGraphNode
		{
			public InstructionGraph Graph;

			public override Color NodeColor => Colors.Start;
			protected override IEnumerator Run_(InstructionGraph graph, InstructionStore variables, int iteration) { yield break; }

			public override void GetConnections(NodeData data) => Graph.GetConnections(data);
			public override void SetConnection(ConnectionData connection, InstructionGraphNode target) => Graph.SetConnection(connection, target);
		}

		public void SetGraph(InstructionGraph graph)
		{
			if (graph != _graph)
			{
				_graph = graph;
				TeardownNodes();
				SetupNodes();
				ShowAll();
			}
		}

		private void SetGraph(object selected)
		{
			if (selected is InstructionGraph graph)
				SetGraph(graph);
		}

		private void CreateGraph(object selected)
		{
			if (selected is Type type)
			{
				var path = EditorUtility.SaveFilePanel("Create a new Instruction Graph", "Assets", type.Name + ".asset", "asset");
				if (path.Length != 0)
				{
					var index = path.IndexOf("Assets");

					if (index > 0)
					{
						var relativePath = path.Substring(index);
						var graph = CreateInstance(type);
						AssetDatabase.CreateAsset(graph, relativePath);
						SetGraph(graph);
					}
				}
			}
		}

		#endregion

		#region Node Management

		private void SetupNodes()
		{
			if (_graph != null)
			{
				_start = CreateInstance<StartNode>();
				_start.Name = "Start";
				_start.GraphPosition = _graph.StartPosition;
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

		private void RefreshNode(InstructionGraphNode node)
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
			var node = InstructionGraphEditor.CreateNode(_graph, type, name, position);
			var data = AddNode(node);
			SetSelection(data);
		}

		public InstructionGraphNode.NodeData AddNode(InstructionGraphNode node)
		{
			var data = new InstructionGraphNode.NodeData(node);
			node.GetConnections(data);
			_nodes.Add(data);
			return data;
		}

		private void RemoveNode(InstructionGraphNode.NodeData node)
		{
			var connections = _nodes
				.SelectMany(data => data.Connections)
				.Where(connection => connection.Target == node)
				.ToList();

			if (node.Node != _start)
			{
				InstructionGraphEditor.DestroyNode(_graph, node.Node, connections, _start);
				_nodes.Remove(node);
			}
		}

		private InstructionGraphNode.NodeData GetNodeData(InstructionGraphNode node)
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
			return RectHelper.TakeHeight(ref rect, InstructionGraphNode.NodeData.HeaderHeight);
		}

		private Rect GetInputBounds(InstructionGraphNode.NodeData node)
		{
			var rect = node.Bounds;
			var header = TakeHeaderHeight(ref rect);
			header.width = header.height;

			return RectHelper.Adjust(header, RectHelper.IconWidth, RectHelper.IconWidth, RectHorizontalAlignment.Center, RectVerticalAlignment.Middle);
		}

		private Rect GetOutputBounds(InstructionGraphNode.ConnectionData connection)
		{
			var node = GetNodeData(connection.From);
			var rect = node.Bounds;

			TakeHeaderHeight(ref rect);
			
			rect.x = rect.xMax - InstructionGraphNode.NodeData.LineHeight;
			rect.y += connection.FromIndex * InstructionGraphNode.NodeData.LineHeight;
			rect.width = InstructionGraphNode.NodeData.LineHeight;
			rect.height = InstructionGraphNode.NodeData.LineHeight;

			return rect;
		}

		private Rect GetInteractionBounds(InstructionGraphNode.NodeData node, int index)
		{
			var rect = node.Bounds;
			var header = TakeHeaderHeight(ref rect);
			var offset = (header.height * (index + 1));

			header.x = header.xMax - offset;
			header.width = header.height;

			return RectHelper.Adjust(header, RectHelper.IconWidth, RectHelper.IconWidth, RectHorizontalAlignment.Center, RectVerticalAlignment.Middle);
		}

		#endregion

		#region Connection Management

		private void SetupOutputConnections(InstructionGraphNode.NodeData node)
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

		private void SetConnection(InstructionGraphNode.ConnectionData connection, InstructionGraphNode.NodeData node)
		{
			InstructionGraphEditor.ChangeConnectionTarget(_graph, connection, node, connection.From == _start);
		}

		#endregion

		#region Selection Management

		private void UpdateSelection()
		{
			_selectedNodes.Clear();

			foreach (var selection in Selection.objects)
			{
				if (selection is InstructionGraphNode node)
				{
					var data = GetNodeData(node);
					if (data != null)
						_selectedNodes.Add(data);
				}
				else if (selection is InstructionGraph graph && graph == _graph)
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
			if (Selection.activeObject is InstructionGraphNode node)
				RefreshNode(node);
			else if (Selection.activeObject is InstructionGraph graph)
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

		public static void BreakpointHit(InstructionGraph graph, InstructionGraphNode node)
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
				{
					_snapAmount.Value = 1;
				}
			}
		}

		private void CreateViewMenu(ref GenericMenu menu, string prefix)
		{
			if (menu == null)
				menu = new GenericMenu();

			menu.AddItem(new GUIContent(prefix + "Go To Start"), false, GoToStart);
			menu.AddItem(new GUIContent(prefix + "Zoom To Selection"), false, GoToSelection);
			menu.AddItem(new GUIContent(prefix + "Show All"), false, ShowAll);
		}

		private void CreateNodeMenu(ref GenericMenu menu, string prefix)
		{
			if (menu == null)
				menu = new GenericMenu();

			var types = TypeHelper.ListDerivedTypes<InstructionGraphNode>()
				.Where(type => type != typeof(MockupNode) && type != typeof(CommentNode))
				.Select(type =>
				{
					var attribute = TypeHelper.GetAttribute<CreateInstructionGraphNodeMenuAttribute>(type);
					var menuName = prefix + (attribute == null ? ObjectNames.NicifyVariableName(type.Name) : attribute.MenuName);
					var last = menuName.LastIndexOf("/");

					return new
					{
						Type = type,
						Order = attribute == null ? 0 : attribute.Order,
						Menu = menuName.Substring(0, last + 1),
						Name = menuName.Substring(last + 1)
					};
				})
				.OrderBy(type => type.Menu)
				.ThenBy(type => type.Order);

			var previousOrder = 0;
			var previousMenu = "";

			foreach (var type in types)
			{
				if (type.Menu == previousMenu && type.Order - previousOrder >= 5)
					menu.AddSeparator(type.Menu);

				previousOrder = type.Order;
				previousMenu = type.Menu;
				menu.AddItem(new GUIContent(type.Menu + type.Name), false, () => CreateNode(type.Type, type.Name, _createPosition));
			}

			menu.AddItem(new GUIContent(prefix), false, null);
			menu.AddItem(new GUIContent(prefix + "Comment"), false, () => CreateNode(typeof(CommentNode), "Comment", _createPosition));
			menu.AddItem(new GUIContent(prefix + "Mockup"), false, () => CreateNode(typeof(MockupNode), "Mockup", _createPosition));
		}

		private void CreateEditMenu(ref GenericMenu menu, string prefix)
		{
			if (menu == null)
				menu = new GenericMenu();

			menu.AddItem(new GUIContent(prefix + "Cut"), false, CutNodes);
			menu.AddItem(new GUIContent(prefix + "Copy"), false, CopyNodes);
			menu.AddItem(new GUIContent(prefix + "Paste"), false, PasteNodes);
		}

		private GenericMenu CreateContextMenu()
		{
			var menu = new GenericMenu();

			CreateNodeMenu(ref menu, "Create/");
			CreateViewMenu(ref menu, "View/");
			menu.AddSeparator(string.Empty);
			CreateEditMenu(ref menu, string.Empty);

			return menu;
		}

		private GenericMenu CreateSelectMenu()
		{
			var types = TypeHelper.ListDerivedTypes<InstructionGraph>();

			var menu = new GenericMenu();

			foreach (var type in types)
				menu.AddItem(new GUIContent("Create Graph/" + ObjectNames.NicifyVariableName(type.Name)), false, CreateGraph, type);

			var graphs = AssetHelper.GetAssetList<InstructionGraph>(false, false);

			if (graphs.Assets.Count > 0)
				menu.AddSeparator("");

			foreach (var graph in graphs.Assets)
				menu.AddItem(new GUIContent(graph.name), graph == _graph, SetGraph, graph);

			return menu;
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

		public void GoToNode(InstructionGraphNode node)
		{
			if (_graph != null)
			{
				var data = GetNodeData(node);
				GoTo(data.Bounds.center, 1.0f); // go to zoom 1 first so ViewArea is the right size
				Pan(new Vector2(ViewArea.width * -0.5f + data.Bounds.width, 0.0f)); // then pan so the node is on the left side
			}
		}

		private void ShowAll()
		{
			ShowAll(_nodes);
		}

		private void ShowAll(List<InstructionGraphNode.NodeData> nodes)
		{
			if (_graph != null)
			{
				var left = float.MaxValue;
				var right = float.MinValue;
				var top = float.MaxValue;
				var bottom = float.MinValue;

				foreach (var node in nodes)
				{
					left = Math.Min(left, node.Bounds.xMin);
					right = Math.Max(right, node.Bounds.xMax);
					top = Math.Min(top, node.Bounds.yMin);
					bottom = Math.Max(bottom, node.Bounds.yMax);
				}

				ShowAll(Rect.MinMaxRect(left, top, right, bottom), new RectOffset(10, 10, (int)_toolbarHeight + 10, 10));
			}
		}

		private InstructionGraphNode.NodeData GetNode(Vector2 position)
		{
			if (_graph != null)
			{
				for (var i = _nodes.Count - 1; i >= 0; --i)
				{
					var node = _nodes[i];

					if (node.Bounds.Contains(position))
						return node;
				}
			}

			return null;
		}

		private InstructionGraphNode.ConnectionData GetConnection(InstructionGraphNode.NodeData node, Vector2 position)
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
		private bool IsWatchOpen => Application.isPlaying && _graph != null && _graph.IsRunning && _isWatchOpen;
		private float WatchLeft => _isWatchOpen ? position.width - _watchWidth : position.width;

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
			style.fixedWidth = InstructionGraphNode.NodeData.Width - 3 * RectHelper.IconWidth;
			style.alignment = TextAnchor.MiddleLeft;
			style.normal.textColor = new Color(0.8f, 0.8f, 0.8f, 1.0f);
			return style;
		}

		private static GUIStyle CreateCommentStyle()
		{
			var style = new GUIStyle();
			style.padding.left = 5;
			style.clipping = TextClipping.Clip;
			style.fixedWidth = InstructionGraphNode.NodeData.Width;
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
				DrawOffsetBackground(rect, _gridTexture.Texture);
				DrawNodes(rect);
				DrawConnections(rect);
				DrawConnectionPreview();

				if (_mouseDragState == MouseState.Select)
					Handles.DrawSolidRectangleWithOutline(_mouseDragBounds, new Color(1.0f, 1.0f, 1.0f, 0.25f), Color.white);
			}
		}

		protected override void PostDraw(Rect rect)
		{
			if (IsWatchOpen)
				DrawWatch(rect);

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
		}

		private void DrawToolbar(Rect rect)
		{
			var padding = _toolbarPadding;

			using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
			{
				using (new EditorGUI.DisabledGroupScope(_graph == null))
				{
					if (GUILayout.Button("Create", EditorStyles.toolbarDropDown, GUILayout.Width(_toolbarButtonWidth)))
					{
						_createPosition = ViewArea.center;
						_nodeMenu.DropDown(new Rect(padding, _toolbarHeight, 0.0f, 0.0f));
					}

					padding += _toolbarButtonWidth;

					if (GUILayout.Button("View", EditorStyles.toolbarDropDown, GUILayout.Width(_toolbarButtonWidth)))
						_viewMenu.DropDown(new Rect(padding, _toolbarHeight, 0f, 0f));

					padding += _toolbarButtonWidth;

					if (GUILayout.Button("Edit", EditorStyles.toolbarDropDown, GUILayout.Width(_toolbarButtonWidth)))
					{
						_createPosition = ViewArea.center;
						_editMenu.DropDown(new Rect(padding, _toolbarHeight, 0f, 0f));
					}
				}

				var isEnabled = Application.isPlaying && _graph != null && _graph.IsRunning;
				var isPlaying = isEnabled && _graph.DebugState == InstructionGraph.PlaybackState.Running;
				var isPaused = isEnabled && _graph.DebugState == InstructionGraph.PlaybackState.Paused;
				var isStepping = isEnabled && _graph.DebugState == InstructionGraph.PlaybackState.Step;
				var isStopping = isEnabled && _graph.DebugState == InstructionGraph.PlaybackState.Stopped;

				using (new EditorGUI.DisabledScope(!isEnabled))
				{
					var playButton = isPlaying || !isEnabled ? _playDisabledButton : _playButton;
					var pauseButton = isPaused || !isEnabled ? _pauseDisabledButton : _pauseButton;
					var stepButton = isStepping || !isEnabled ? _stepDisabledButton : _stepButton;
					var stopButton = isStopping || !isEnabled ? _stopDisabledButton : _stopButton;

					var shouldPlay = GUILayout.Toggle(isPlaying, playButton.Content, EditorStyles.toolbarButton);
					var shouldPause = GUILayout.Toggle(isPaused, pauseButton.Content, EditorStyles.toolbarButton);
					var shouldStep = GUILayout.Toggle(isStepping, stepButton.Content, EditorStyles.toolbarButton);
					var shouldStop = GUILayout.Toggle(isStopping, stopButton.Content, EditorStyles.toolbarButton);

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
					hasBreak = GUILayout.Toggle(hasBreak, breakpointButton.Content, EditorStyles.toolbarButton);

					if (canBreak)
						_selectedNodes[0].Node.IsBreakpoint = hasBreak;
				}

				GUILayout.FlexibleSpace();

				InstructionGraph.IsDebugBreakEnabled = GUILayout.Toggle(InstructionGraph.IsDebugBreakEnabled, InstructionGraph.IsDebugBreakEnabled ? _disableBreakpointsButton.Content : _enableBreakpointsButton.Content, EditorStyles.toolbarButton);
				InstructionGraph.IsDebugLoggingEnabled = GUILayout.Toggle(InstructionGraph.IsDebugLoggingEnabled, InstructionGraph.IsDebugLoggingEnabled ? _disableLoggingButton.Content : _enableLoggingButton.Content, EditorStyles.toolbarButton);

				_breakpointsEnabled.Value = InstructionGraph.IsDebugBreakEnabled;
				_loggingEnabled.Value = InstructionGraph.IsDebugLoggingEnabled;

				if (GUILayout.Button("Settings", EditorStyles.toolbarDropDown, GUILayout.Width(_toolbarButtonWidth)))
					PopupWindow.Show(new Rect(position.width - (4 * _toolbarButtonWidth), _toolbarHeight, 0, 0), _settingsMenu);

				if (GUILayout.Button(_graph == null ? "No Graph Selected" : _graph.name, EditorStyles.toolbarDropDown, GUILayout.Width(_toolbarButtonWidth * 2)))
				{
					var menu = CreateSelectMenu();
					menu.DropDown(new Rect(position.width - (3 * _toolbarButtonWidth), _toolbarHeight, 0, 0));
				}

				_isLocked = GUILayout.Toggle(_isLocked, _isLocked ? _unlockButton.Content : _lockButton.Content, EditorStyles.toolbarButton);

				using (new EditorGUI.DisabledScope(_graph == null || !_graph.IsRunning))
				{
					var icon = (_graph != null && _graph.IsRunning) ? (_isWatchOpen ? _closeWatchButton : _openWatchButton) : _disabledWatchButton;
					_isWatchOpen = GUILayout.Toggle(_isWatchOpen, icon.Content, EditorStyles.toolbarButton);
				}
			}
		}

		private void DrawNodes(Rect rect)
		{
			foreach (var node in _nodes)
			{
				if (rect.Overlaps(node.Bounds))
					DrawNode(node);
			}
		}

		private int GetNodeIteration(InstructionGraphNode.NodeData node)
		{
			if (Application.isPlaying && _graph != null && _graph.IsRunning)
			{
				if (node.Node == _start)
					return 0;

				return _graph.IsInCallStack(node.Node);
			}

			return -1;
		}

		private void DrawNode(InstructionGraphNode.NodeData node)
		{
			var isComment = node.Node is CommentNode;

			if (isComment)
				node.InnerHeight = _commentStyle.Style.CalcHeight(new GUIContent((node.Node as CommentNode).Comment), InstructionGraphNode.NodeData.Width);

			var rect = node.Bounds;

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

			EditorGUI.LabelField(labelRect, nodeLabel, _headerStyle.Style);

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
				var commentRect = RectHelper.Inset(rect, 0.0f, 0.0f, 0.0f, InstructionGraphNode.NodeData.FooterHeight);
				var commentNode = node.Node as CommentNode;

				EditorGUI.LabelField(commentRect, commentNode.Comment, _commentStyle.Style);
			}
			else
			{
				foreach (var connection in node.Connections)
				{
					var outputRect = GetOutputBounds(connection);
					var outputIconRect = RectHelper.Adjust(outputRect, _outputButton.Content.image.width, _outputButton.Content.image.height, RectHorizontalAlignment.Center, RectVerticalAlignment.Middle);
					var outputLabelRect = RectHelper.TakeHeight(ref rect, InstructionGraphNode.NodeData.LineHeight);

					// tooltip positioning is pretty messed up with zoom so not showing them for now
					var label = new GUIContent(ObjectNames.NicifyVariableName(connection.Name));//, Label.GetTooltip(connection.From.GetType(), connection.Field));

					EditorGUI.LabelField(outputLabelRect, label, _connectionStyle.Style);
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
				var paused = _graph.DebugState == InstructionGraph.PlaybackState.Paused;

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

		private void DrawConnection(InstructionGraphNode.ConnectionData from, InstructionGraphNode.NodeData to)
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
				endColor = InstructionGraph.IsDebugBreakEnabled ? _breakColor : _disabledBreakColor;

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

		private void DrawWatch(Rect rect)
		{
			if (_watching != _graph)
				SetupWatch();
			else if (_thisStore == null || _thisStore.Store != _graph.Store.This)
				UpdateWatchThis();

			rect.x = WatchLeft;
			rect.y = ToolbarBottom;
			rect.width -= WatchLeft;
			rect.height -= ToolbarBottom + 1;

			using (ColorScope.BackgroundColor(new Color(0.82f, 0.82f, 0.82f)))
			{
				GUI.Box(rect, "", _watchStyle.Style);
			}

			using (new GUILayout.AreaScope(rect))
			{
				using (var scroller = new EditorGUILayout.ScrollViewScope(_watchScrollPosition, false, false, GUI.skin.horizontalScrollbar, GUI.skin.verticalScrollbar, GUI.skin.scrollView, GUILayout.Width(rect.width), GUILayout.Height(rect.height)))
				{
					EditorGUILayout.Space();

					if (_selectedStore != null) _selectedStore.Draw();
					if (_thisStore != null) _thisStore.Draw();
					if (_localStore != null) _localStore.Draw();
					if (_globalStore != null) _globalStore.Draw();
					if (_inputStore != null) _inputStore.Draw();
					if (_outputStore != null) _outputStore.Draw();

					_watchScrollPosition = scroller.scrollPosition;

					EditorGUILayout.Space();
				}
			}

			UpdateWatchSelected(_selectedStore);
			UpdateWatchSelected(_thisStore);
			UpdateWatchSelected(_localStore);
			UpdateWatchSelected(_globalStore);
			UpdateWatchSelected(_inputStore);
			UpdateWatchSelected(_outputStore);
		}

		private void SetupWatch()
		{
			_watching = _graph;
			_thisStore = CreateStoreControl(InstructionStore.ThisStoreName, _graph.Store.This as IVariableStore, _thisStore);
			_inputStore = CreateStoreControl(InstructionStore.InputStoreName, _graph.Store.Input, _inputStore);
			_outputStore = CreateStoreControl(InstructionStore.OutputStoreName, _graph.Store.Output, _outputStore);
			_localStore = CreateStoreControl(InstructionStore.LocalStoreName, _graph.Store.Local, _localStore);
			_globalStore = CreateStoreControl(InstructionStore.GlobalStoreName, _graph.Store.Global, _globalStore);
		}

		private void TeardownWatch()
		{
			_watching = null;
			_thisStore = null;
			_inputStore = null;
			_outputStore = null;
			_localStore = null;
			_globalStore = null;
			_selectedStore = null;
		}

		private void UpdateWatchThis()
		{
			_thisStore = CreateStoreControl(InstructionStore.ThisStoreName, _graph.Store.This as IVariableStore, _thisStore);
		}

		private void UpdateWatchSelected(VariableStoreControl control)
		{
			if (control != null && control.Selected != null)
				_selectedStore = CreateStoreControl(control.SelectedName, control.Selected, _selectedStore);
		}

		private VariableStoreControl CreateStoreControl(string name, IVariableStore store, VariableStoreControl existing)
		{
			if (existing == null || existing.Store != store)
				return store != null ? new VariableStoreControl().Setup(name, store) : null;
			else
				return existing;
		}

		#endregion

		#region Copy and Paste

		private static List<InstructionGraphNode.NodeData> _copiedNodes = new List<InstructionGraphNode.NodeData>();

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
					var copy = InstructionGraphEditor.CloneNode(node);
					var data = new InstructionGraphNode.NodeData(copy);
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
				var pastedNodes = new List<InstructionGraphNode.NodeData>();

				foreach (var node in _copiedNodes)
				{
					_nodes.Add(node);
					pastedNodes.Add(node);
					SetupOutputConnections(node);
				}

				InstructionGraphEditor.AddClonedNodes(_graph, _copiedNodes, _createPosition);
				TransferSelection(ref pastedNodes);
				CopyNodes(); // re-copy so the same set of nodes can be pasted twice
			}
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

			if (IsWatchOpen && mouse.x >= WatchLeft)
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
				.SetEvent(EventType.KeyDown, KeyCode.LeftArrow)
				.AddAction(MoveLeft);

			input.Create<InputManager.KeyboardTrigger>()
				.SetEvent(EventType.KeyDown, KeyCode.RightArrow)
				.AddAction(MoveRight);

			input.Create<InputManager.KeyboardTrigger>()
				.SetEvent(EventType.KeyDown, KeyCode.UpArrow)
				.AddAction(MoveUp);

			input.Create<InputManager.KeyboardTrigger>()
				.SetEvent(EventType.KeyDown, KeyCode.DownArrow)
				.AddAction(MoveDown);

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
				.AddAction(() => { _createPosition = ViewArea.center; CopyNodes(); PasteNodes(); });
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

		private bool HasInput(InstructionGraphNode.NodeData node)
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

		private void SetHoveredNode(InstructionGraphNode.NodeData node)
		{
			if (_mouseMoveState != MouseState.Move || _hoveredNode != node)
			{
				ResetHover();
				_mouseMoveState = MouseState.Move;
				_hoveredNode = node;
				Repaint();
			}
		}

		private void SetHoveredOutput(InstructionGraphNode.ConnectionData connection)
		{
			if (_mouseMoveState != MouseState.Connect || _hoveredOutput != connection)
			{
				ResetHover();
				_mouseMoveState = MouseState.Connect;
				_hoveredOutput = connection;
				Repaint();
			}
		}

		private void SetHoveredInput(InstructionGraphNode.NodeData node)
		{
			if (_mouseMoveState != MouseState.Connect || _hoveredInput != node)
			{
				ResetHover();
				_mouseMoveState = MouseState.Connect;
				_hoveredInput = node;
				Repaint();
			}
		}

		private void SetHoveredInteraction(InstructionGraphNode.NodeData interaction)
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

					if (_selectedNodes.Count > 0)
						_mouseDragOffset = Event.current.mousePosition - _selectedNodes[0].Position;

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

					foreach (var node in _nodes)
					{
						if (_mouseDragBounds.Overlaps(node.Bounds))
							_pendingNodes.Add(node);
					}

					break;
				}
				case MouseState.Move:
				{
					var origin = _selectedNodes[0].Position;
					var offset = Event.current.mousePosition - _mouseDragOffset;

					foreach (var node in _selectedNodes)
					{
						var position = (node.Position - origin) + offset;

						position.x = _snapToGrid.Value ? MathHelper.Snap(position.x, _gridSize * _snapAmount.Value) : position.x;
						position.y = _snapToGrid.Value ? MathHelper.Snap(position.y, _gridSize * _snapAmount.Value) : position.y;

						InstructionGraphEditor.SetNodePosition(_graph, node, position, node.Node == _start);
					}

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

		private void SetSelection(InstructionGraphNode.NodeData node)
		{
			if (_selectedNodes.Count != 1 || _selectedNodes[0] != node)
			{
				_selectedNodes.Clear();
				_selectedNodes.Add(node);
				ApplySelection();
			}
		}

		private void TransferSelection(ref List<InstructionGraphNode.NodeData> nodes)
		{
			var selected = _selectedNodes;
			_selectedNodes = nodes;
			nodes = selected;

			selected.Clear();
			ApplySelection();
		}

		private void SetSelection(InstructionGraphNode.ConnectionData connection)
		{
			if (_selectedConnections.Count != 1 || _selectedConnections[0] != connection)
			{
				_selectedConnections.Add(connection);
				SetConnection(connection, null);
				Repaint();
			}
		}

		private void TransferSelection(ref List<InstructionGraphNode.ConnectionData> connections)
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

		#region Keyboard Movement

		void MoveLeft()
		{
			if (_selectedNodes.Count > 0)
				MoveSelectedNodes(new Vector2(-_snapAmount.Value, 0.0f));
			else
				Pan(new Vector2(-_gridSize * 5.0f, 0.0f));
		}

		void MoveRight()
		{
			if (_selectedNodes.Count > 0)
				MoveSelectedNodes(new Vector2(_snapAmount.Value, 0.0f));
			else
				Pan(new Vector2(_gridSize * 5.0f, 0.0f));
		}

		void MoveUp()
		{
			if (_selectedNodes.Count > 0)
				MoveSelectedNodes(new Vector2(0.0f, -_snapAmount.Value));
			else
				Pan(new Vector2(0.0f, -_gridSize * 5.0f));
		}

		void MoveDown()
		{
			if (_selectedNodes.Count > 0)
				MoveSelectedNodes(new Vector2(0.0f, _snapAmount.Value));
			else
				Pan(new Vector2(0.0f, _gridSize * 5.0f));
		}

		void MoveSelectedNodes(Vector2 amount)
		{
			foreach (var node in _selectedNodes)
			{
				var position = node.Position + new Vector2(_gridSize * amount.x, _gridSize * amount.y);
				InstructionGraphEditor.SetNodePosition(_graph, node, position, node.Node == _start);
			}

			Repaint();
		}

		#endregion
	}
}
