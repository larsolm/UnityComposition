using PiRhoSoft.CompositionEngine;
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
	public class InstructionGraphWindow : ViewportWindow
	{
		// inspiration and parts of the implementation for this class have been adapted from the MIT licensed Unity
		// Node Editor Base Extension: https://unitylist.com/p/tb/Unity-Node-Editor-Base

		public static InstructionGraphWindow Instance => GetWindow<InstructionGraphWindow>("Instruction Graph", true);
		public static InstructionGraphWindow OpenInstance { get; private set; }

		private static readonly Base64Texture _gridTexture = new Base64Texture("iVBORw0KGgoAAAANSUhEUgAAAEAAAABACAIAAAAlC+aJAAAABGdBTUEAALGPC/xhBQAAAAlwSFlzAAAOwgAADsIBFShKgAAAABl0RVh0U29mdHdhcmUAcGFpbnQubmV0IDQuMC4xNzNun2MAAAKnSURBVGhD7ZnbbiJBDETzNwTCfbm/Av//S3uGKkXTETYbCS2zG5+HpNsu3F3tYWaUvH2kTCaT1Wq13W4ZgKMtSv26obETLYpTarlcRhogNZ1Od7vdbDZLZOPxeL1eX6/XMlAGvkCqDPSgRBkwipcBUwZ6KPUyA4gS0LHeZrNhAI62KEU50NiJFsUptVgsIg2QYn/4xEYie39/52Q7A/zKYUnOw5MYlgRPYihFozyJQcZxeHIPjhXN5XJ508IJ6MCTmD+XeZTyUMaxHg6HzoBbEkATh34JORBAiaEb6L7SMUjRUU4fc7RFqZfdhRwIoEQZMIqXAVMGeihVBgyp7xlAlICO9epB1qH48w2o9QnUoqGexCADT2Io9VCGSWT89PweHNZ+v+/ehRwIQMd6lLud79rRFqVu+99q7ESL4tpZpIFP2ef4Lhy/DbglATSxvgNG8ecbQJ2AFB3l9DFHW5RSZzV2okVxSj35NupAACXKgFG8DJgy0EOpMmBIfc8AogR0rFfPgQ7Fn9uB1/xh66FMGvD8HnTyeDx2BjiPBNrNufIBBuBoi1JU1OUBTrQoTikKRhpQit3T+UQ2n88p9V+8zOmSikA69LuQAwGUKANG8TJgykAPpcqAIfXDDCBKQMd6g36QoUugljpws/3haItS6oDGTrQorg5EGiDF2dMBXhYSGR5YrnuVQPovwkH4ZQ67OVwYqD2JUQc8iaEULfUkgLNni1xpnt+DJrgDuqQiuAqH/h1AnYB06HchBwIoUQaM4mXAlIEeSpUBQ+qHGWCUwPOC5w47YwCOtiiFT9DYiRbFKcWDNtIAKfaNT3aZyEajEQfRGcBHAnviMHjr0P4cbVFqf0NjJ1oUpxSbizSg1Ol0wmoiY/f+Jx8mchCBJzF/X4bgfD7/Bp0ChIMH9TUUAAAAAElFTkSuQmCC");
		private static readonly Base64Texture _windowIcon = new Base64Texture("iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAABGdBTUEAALGPC/xhBQAAACBjSFJNAAB6JgAAgIQAAPoAAACA6AAAdTAAAOpgAAA6mAAAF3CculE8AAAACXBIWXMAABJ0AAASdAHeZh94AAAAB3RJTUUH4wEOBR4Wp+XVagAAABh0RVh0U29mdHdhcmUAcGFpbnQubmV0IDQuMS41ZEdYUgAAATdJREFUOE+lkUtuglAYhWnnfQwITUdEQF6CPFRimBqcdKCzdl3GCZg0cS8O3IeJ2NA90HNSBsqlsYkkPx+H++VcbpDqur5pfm+SdJem6UOSJI/Xhh79i4LRaPQ0Ho8PmNNkMqnAsqGQMQf6FwVBEDzj5ck0zfcwDD/7/f4badv2x3nmOj36QkEcx5XneVs878FNQyHToy8URFFUYsdsOByuDcOYksjzVs7odRZg+AU5xJ3v+ysSuWjlnF5nAYQSZ1wOBoMCnDUUMr3OAuxSOY6zhbgHNw2FTE8oUFWVBWWv18sgrZGnJM4+P89cp0dfKMD5+AW567o7cNWwaOWcXmcBFktd1xeWZRWaps1I5GUrL+j9VXDEb/oCv8FTw658FApw3SuK8iLL8uu1oUf/ouCW6Xz5/6mlH0LCqCZdcm2YAAAAAElFTkSuQmCC");

		private static GenericMenu _viewMenu;
		private static GenericMenu _nodeMenu;
		private static GenericMenu _contextMenu;
		private static SettingsMenu _settingsMenu;

		private static readonly StaticStyle _headerStyle = new StaticStyle(CreateHeaderStyle);
		private static readonly StaticStyle _connectionStyle = new StaticStyle(CreateConnectionStyle);
		private static readonly IconButton _outputButton = new IconButton("sv_icon_dot0_sml", "Click and drag to make a connection from this output");
		private static readonly IconButton _inputButton = new IconButton("sv_icon_dot0_sml", "Drag an output to here to make a connection");
		private static readonly IconButton _removeButton = new IconButton("d_Toolbar Minus", "Remove this node");

		private const float _knobRadius = 6.0f;
		private const float _toolbarPadding = 17.0f;
		private const float _toolbarHeight = 17.0f;
		private const float _toolbarButtonWidth = 60.0f;
		private const float _dragTolerance = 4.0f;

		private const float _gridSize = 64.0f / 5.0f;

		private static BoolPreference _snapToGrid = new BoolPreference("PiRhoSoft.Composition.SnapToGrid", true);
		private static IntPreference _snapAmount = new IntPreference("PiRhoSoft.Composition.InstructionGraphWindow", 1);

		private static Color _hoveredColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
		private static Color _selectedColor = new Color(0.96f, 0.95f, 0.2f, 0.89f);
		private static Color _nodeColor = new Color(0.23f, 0.24f, 0.29f, 1.0f);
		private static Color _knobColor = new Color(0.49f, 0.73f, 1.0f, 1.0f);

		private InstructionGraph _graph = null;
		private StartNode _start = null;
		private List<InstructionGraphNode.NodeData> _nodes = new List<InstructionGraphNode.NodeData>();

		private Vector2 _createPosition;
		private InstructionGraphNode.NodeData _toRemove = null;
		private bool _showContextMenu = false;

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

		#region Setup

		[MenuItem("Window/Composition/Instruction Graph")]
		static void Create()
		{
			Instance.SetGraph(null);
		}

		[OnOpenAsset]
		static bool OpenAsset(int instanceID, int line)
		{
			var graph = EditorUtility.InstanceIDToObject(instanceID) as InstructionGraph;
			if (graph != null)
			{
				Instance.SetGraph(graph);
				return true;
			}

			return false;
		}

		protected override void Setup(InputManager input)
		{
			base.Setup(input);

			titleContent.image = _windowIcon.Texture;

			CreateViewMenu(ref _viewMenu, string.Empty);
			CreateNodeMenu(ref _nodeMenu, string.Empty);

			_contextMenu = CreateContextMenu();
			_settingsMenu = new SettingsMenu();

			SetupInput(input);
			SetupNodes();

			autoRepaintOnSceneChange = true;
			Undo.undoRedoPerformed += UndoPerformed;
			Selection.selectionChanged += UpdateSelection;
			OpenInstance = this;
		}

		protected override void Teardown()
		{
			OpenInstance = null;
			Selection.selectionChanged -= UpdateSelection;
			Undo.undoRedoPerformed -= UndoPerformed;

			TeardownNodes();

			_viewMenu = null;
			_nodeMenu = null;
			_contextMenu = null;
			_settingsMenu = null;

			base.Teardown();
		}

		#endregion

		#region Graph Management

		private class StartNode : InstructionGraphNode
		{
			public InstructionGraph Graph;

			public override bool IsExecutionImmediate => true;
			public override InstructionGraphExecutionMode ExecutionMode => InstructionGraphExecutionMode.Normal;
			public override Color GetNodeColor() => new Color(0.0f, 0.35f, 0.0f);
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
				GoToStart();
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

		private InstructionGraphNode.NodeData AddNode(InstructionGraphNode node)
		{
			var data = new InstructionGraphNode.NodeData(node);
			node.GetConnections(data);
			_nodes.Add(data);
			return data;
		}

		private void RemoveNode(InstructionGraphNode.NodeData node)
		{
			if (node.Node != _start)
			{
				RemoveInputConnections(node);
				InstructionGraphEditor.DestroyNode(_graph, node.Node);
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

		private void RemoveInputConnections(InstructionGraphNode.NodeData node)
		{
			foreach (var input in _nodes)
			{
				foreach (var output in input.Connections)
				{
					if (output.Target == node)
						output.ChangeTarget(null);
				}
			}
		}

		private void SetConnection(InstructionGraphNode.ConnectionData connection, InstructionGraphNode.NodeData node)
		{
			InstructionGraphEditor.ChangeConnectionTarget(connection, node);
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
				else if (selection is InstructionGraph graph)
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
			else if (Selection.activeObject is InstructionGraphNode graph)
				RefreshNode(_start);
		}

		private void DeleteSelectedNodes()
		{
			// TODO
		}

		#endregion

		#region Undo Handling

		private void UndoPerformed()
		{
			TeardownNodes();
			SetupNodes();
			Repaint();
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
		}

		private GenericMenu CreateContextMenu()
		{
			var menu = new GenericMenu();

			CreateNodeMenu(ref menu, "Create/");
			CreateViewMenu(ref menu, "View/");

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

		private int GetInteraction(InstructionGraphNode.NodeData node, Vector2 position)
		{
			if (GetInteractionBounds(node, 0).Contains(position))
				return 0;

			return -1;
		}

		#endregion

		#region Drawing

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
			DrawToolbar(rect);

			if (_toRemove != null)
			{
				RemoveNode(_toRemove);
				_toRemove = null;
			}

			if (_showContextMenu)
			{
				_contextMenu.ShowAsContext();
				_showContextMenu = false;
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
				}

				padding += _toolbarButtonWidth;

				if (GUILayout.Button("View", EditorStyles.toolbarDropDown, GUILayout.Width(_toolbarButtonWidth)))
					_viewMenu.DropDown(new Rect(padding, _toolbarHeight, 0f, 0f));

				GUILayout.FlexibleSpace();

				if (GUILayout.Button("Settings", EditorStyles.toolbarDropDown, GUILayout.Width(_toolbarButtonWidth)))
					PopupWindow.Show(new Rect(position.width - (3 * _toolbarButtonWidth), _toolbarHeight, 0, 0), _settingsMenu);

				if (GUILayout.Button(_graph == null ? "No Graph Selected" : _graph.name, EditorStyles.toolbarDropDown, GUILayout.Width(_toolbarButtonWidth * 2)))
				{
					var menu = CreateSelectMenu();
					menu.DropDown(new Rect(position.width - (2 * _toolbarButtonWidth), _toolbarHeight, 0, 0));
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

		private void DrawNode(InstructionGraphNode.NodeData node)
		{
			var rect = node.Bounds;

			var outlineRect = rect;
			var headerRect = TakeHeaderHeight(ref rect);
			var inputRect = GetInputBounds(node);
			var deleteRect = GetInteractionBounds(node, 0);
			var headerColor = node.Node.GetNodeColor();

			var labelRect = RectHelper.AdjustHeight(headerRect, EditorGUIUtility.singleLineHeight, RectVerticalAlignment.Middle);
			var inputIconRect = RectHelper.Adjust(inputRect, _inputButton.Content.image.width, _inputButton.Content.image.height, RectHorizontalAlignment.Center, RectVerticalAlignment.Middle);
			var deleteIconRect = RectHelper.Adjust(deleteRect, _removeButton.Content.image.width, _removeButton.Content.image.height, RectHorizontalAlignment.Center, RectVerticalAlignment.Middle);

			RectHelper.TakeWidth(ref labelRect, headerRect.height);
			RectHelper.TakeTrailingWidth(ref labelRect, headerRect.height);

			EditorGUI.DrawRect(headerRect, headerColor);

			if (node.Node != _start)
				EditorGUI.LabelField(inputIconRect, _inputButton.Content, GUIStyle.none);

			EditorGUI.LabelField(labelRect, node.Node.Name, _headerStyle.Style);

			if (node.Node != _start)
			{
				EditorGUI.LabelField(deleteIconRect, _removeButton.Content, GUIStyle.none);

				if (_hoveredInteraction == node)
					Handles.DrawSolidRectangleWithOutline(deleteRect, Color.clear, _hoveredColor);

				if (_selectedInteraction == node)
					Handles.DrawSolidRectangleWithOutline(deleteRect, Color.clear, _selectedColor);
			}

			EditorGUI.DrawRect(rect, _nodeColor);

			foreach (var connection in node.Connections)
			{
				var outputRect = GetOutputBounds(connection);
				var outputIconRect = RectHelper.Adjust(outputRect, _outputButton.Content.image.width, _outputButton.Content.image.height, RectHorizontalAlignment.Center, RectVerticalAlignment.Middle);
				var outputLabelRect = RectHelper.TakeHeight(ref rect, InstructionGraphNode.NodeData.LineHeight);

				// tooltip positioning is pretty messed up with zoom but nothing can really be done about that short
				// of implementing a custom tooltip system
				var label = new GUIContent(ObjectNames.NicifyVariableName(connection.Name), Label.GetTooltip(connection.From.GetType(), connection.Field));

				EditorGUI.LabelField(outputLabelRect, label, _connectionStyle.Style);
				EditorGUI.LabelField(outputIconRect, _outputButton.Content, GUIStyle.none);

				if (connection == _hoveredOutput)
					Handles.DrawSolidRectangleWithOutline(outputRect, Color.clear, _hoveredColor);

				if (_selectedConnections.Contains(connection))
					Handles.DrawSolidRectangleWithOutline(outputRect, Color.clear, _selectedColor);
			}

			if (_hoveredInput == node)
				Handles.DrawSolidRectangleWithOutline(inputRect, Color.clear, _hoveredColor);

			if (_hoveredNode == node || _pendingInput == node || _pendingNodes.Contains(node))
				Handles.DrawSolidRectangleWithOutline(outlineRect, Color.clear, _hoveredColor);

			if (_selectedNodes.Contains(node))
				Handles.DrawSolidRectangleWithOutline(outlineRect, Color.clear, _selectedColor);
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

			if (outputBounds.xMax > inputBounds.xMin)
			{
				var difference = (end.y - start.y) / 3;
				var magnitude = 200.0f;

				Handles.DrawBezier(start, end, start + new Vector2(magnitude, difference), end - new Vector2(magnitude, difference), _knobColor, null, 3);
			}
			else
			{
				HandleHelper.DrawBezier(start, end, _knobColor);
			}

			HandleHelper.DrawCircle(start, _knobRadius, _knobColor);
			HandleHelper.DrawCircle(end, _knobRadius, _knobColor);
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

		private void SetupSelection(InputManager input)
		{
			input.Create<InputManager.EventTrigger>()
				.SetEvent(EventType.MouseMove)
				.AddAction(UpdateHover);

			input.Create<InputManager.MouseTrigger>()
				.SetEvent(EventType.MouseDown, InputManager.MouseButton.Left)
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
				.AddAction(DeleteSelectedNodes);

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
		}

		private void SetupContextMenu(InputManager input)
		{
			var wasMouseDragged = false;

			input.Create<InputManager.EventTrigger>()
				.SetEvent(EventType.MouseDown)
				.AddAction(() => wasMouseDragged = false);

			input.Create<InputManager.EventTrigger>()
				.SetEvent(EventType.MouseDrag)
				.AddAction(() => wasMouseDragged = true);

			input.Create<InputManager.MouseTrigger>()
				.SetEvent(EventType.MouseDown, InputManager.MouseButton.Right)
				.AddAction(() => _createPosition = Event.current.mousePosition);

			input.Create<InputManager.MouseTrigger>()
				.SetEvent(EventType.MouseUp, InputManager.MouseButton.Right)
				.AddCondition(() => _graph != null && _hoveredNode == null && !wasMouseDragged)
				.AddAction(() => _showContextMenu = true);
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
					else if (GetInputBounds(node).Contains(Event.current.mousePosition) && HasInput(node))
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
						_mouseDragOffset = Event.current.mousePosition - _selectedNodes[0].Node.GraphPosition;

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
					var origin = _selectedNodes[0].Node.GraphPosition;
					var offset = Event.current.mousePosition - _mouseDragOffset;

					foreach (var node in _selectedNodes)
					{
						var position = (node.Node.GraphPosition - origin) + offset;

						position.x = _snapToGrid.Value ? MathHelper.Snap(position.x, _gridSize * _snapAmount.Value) : position.x;
						position.y = _snapToGrid.Value ? MathHelper.Snap(position.y, _gridSize * _snapAmount.Value) : position.y;

						InstructionGraphEditor.SetNodePosition(_graph, node, position, node.Node == _start);
					}

					break;
				}
				case MouseState.Connect:
				{
					var node = GetNode(Event.current.mousePosition);
					var canConnect = node != null && node.Node != _start && !_selectedConnections.Select(connection => connection.From).Contains(node.Node);

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
						{
							SetConnection(connection, _pendingInput);
						}

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
				var position = node.Node.GraphPosition + new Vector2(_gridSize * amount.x, _gridSize * amount.y);
				InstructionGraphEditor.SetNodePosition(_graph, node, position, node.Node == _start);
			}

			Repaint();
		}

		#endregion
	}
}
