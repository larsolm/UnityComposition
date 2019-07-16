using PiRhoSoft.Composition;
using PiRhoSoft.Utilities.Editor;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Composition.Editor
{
	public interface IInputOutputNode
	{
		GraphNode.NodeData Data { get; }
		GraphViewInputPort Input { get; }
		List<GraphViewOutputPort> Outputs { get; }

		void UpdateColors(bool active, int iteration);
	}

	public class DefaultGraphViewNode : GraphViewNode, IInputOutputNode
	{
		public const string UssDefaultName = UssClassName + "--default";
		public const string UssCallstackBorderClassName = GraphViewEditor.UssClassName + "__node-callstack-border";
		public const string UssBreakpointClassName = GraphViewEditor.UssClassName + "__node-breakpoint-button";
		public const string UssBreakpointContainerClassName = GraphViewEditor.UssClassName + "__node-breakpoint-container";
		public const string UssBreakpointActiveClassName = UssBreakpointClassName + "--active";

		private static readonly CustomStyleProperty<Color> _breakColorProperty = new CustomStyleProperty<Color>("--break-color");
		private static readonly CustomStyleProperty<Color> _callstackActiveColorProperty = new CustomStyleProperty<Color>("--callstack-active-color");
		private static readonly CustomStyleProperty<Color> _inCallstackColorProperty = new CustomStyleProperty<Color>("--in-callstack-color");

		private static readonly Color _defaultPortColor = new Color(240 / 255f, 240 / 255f, 240 / 255f); // Copied from internal Port class
		private static readonly Color _defaultBreakColor = new Color(1.0f, 0.2f, 0.2f, 1.0f);
		private static readonly Color _defaultCallstackActiveColor = new Color(0.0f, 0.9f, 0.0f, 1.0f);
		private static readonly Color _defaultInCallstackColor = new Color(0.3f, 0.8f, 0.3f, 1.0f);

		public GraphViewInputPort Input { get; private set; }
		public List<GraphViewOutputPort> Outputs { get; private set; }

		private readonly VisualElement _breakpoint;
		private readonly TextField _rename;
		private readonly VisualElement _callstackBorder;

		private Color _breakColor = _defaultBreakColor;
		private Color _callstackActiveColor = _defaultCallstackActiveColor;
		private Color _inCallstackColor = _defaultInCallstackColor;

		public DefaultGraphViewNode(GraphNode node, GraphViewConnector nodeConnector, bool isStart) : base(node, isStart)
		{
			AddToClassList(UssDefaultName);

			if (!IsStartNode)
			{
				CreateInput(nodeConnector);
				CreateDeleteButton();

				_rename = CreateRename();
				_breakpoint = CreateBreakpoint();
				_callstackBorder = CreateCallstackBorder();
			}

			if (Data.Connections.Count == 0)
				expanded = false;
			else
				CreateOutputs(nodeConnector);

			RefreshPorts();
		}

		private void CreateInput(GraphViewConnector nodeConnector)
		{
			Input = new GraphViewInputPort(this, nodeConnector) { tooltip = "Drag an output to here to make a connection" };
			titleContainer.Insert(0, Input);
		}

		private void CreateOutputs(GraphViewConnector nodeConnector)
		{
			Outputs = new List<GraphViewOutputPort>(Data.Connections.Count);

			foreach (var connection in Data.Connections)
			{
				var output = new GraphViewOutputPort(this, connection, nodeConnector) { portName = connection.Name, tooltip = "Click and drag to make a connection from this output" };
				outputContainer.Add(output);
				Outputs.Add(output);
			}
		}

		private TextField CreateRename()
		{
			var label = titleContainer.Q<Label>("title-label");
			label.tooltip = "Double click to rename this node";

			return CreateEditableLabel(label, () => Data.Node.name, OnNameChanged);
		}

		private void OnNameChanged(string name)
		{
			title = name;
			Data.Node.name = name;
		}

		private VisualElement CreateBreakpoint()
		{
			var breakpoint = new VisualElement();
			breakpoint.AddToClassList(UssBreakpointClassName);

			var breakpointContainer = new VisualElement { tooltip = "Toggle this node as a breakpoint" };
			breakpointContainer.AddToClassList(UssBreakpointContainerClassName);
			breakpointContainer.AddManipulator(new Clickable(ToggleBreakpoint));
			breakpointContainer.Add(breakpoint);

			breakpoint.EnableInClassList(UssBreakpointActiveClassName, Data.Node.IsBreakpoint);

			Input.Add(breakpointContainer);

			return breakpoint;
		}

		private VisualElement CreateCallstackBorder()
		{
			var border = new VisualElement();
			border.AddToClassList(UssCallstackBorderClassName);

			mainContainer.Add(border);

			return border;
		}

		public void UpdateColors(bool active, int iteration)
		{
			if (!IsStartNode)
			{
				var inCallstack = Data.Node.Graph.IsInCallStack(Data.Node);
				var paused = Data.Node.Graph.DebugState == Graph.PlaybackState.Paused;
				var outputs = Input.connections.Select(edge => edge.output).OfType<GraphViewOutputPort>();
				var label = iteration > 0 ? string.Format("{0} ({1})", Data.Node.name, iteration) : Data.Node.name;
				var borderColor = active ? (paused ? _breakColor : _callstackActiveColor) : _inCallstackColor;

				title = label;

				Input.portColor = inCallstack ? _inCallstackColor : Input.DefaultColor.GetValueOrDefault(_defaultPortColor);

				foreach (var output in outputs)
					output.portColor = Data.Node.Graph.IsInCallStack(Data.Node, output.Node.Data.Node.name) ? _inCallstackColor : output.DefaultColor.GetValueOrDefault(_defaultPortColor);

				_callstackBorder.style.visibility = inCallstack ? Visibility.Visible : Visibility.Hidden;
				_callstackBorder.style.borderColor = borderColor;
			}
		}

		private void ToggleBreakpoint()
		{
			Data.Node.IsBreakpoint = !Data.Node.IsBreakpoint;

			_breakpoint.EnableInClassList(UssBreakpointActiveClassName, Data.Node.IsBreakpoint);
		}

		public override void OnUnselected()
		{
			base.OnUnselected();

			HideEditableText(_rename);
		}

		public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
		{
			if (!IsStartNode)
			{
				evt.menu.AppendAction("Toggle Breakpoint", action => ToggleBreakpoint());
				evt.menu.AppendAction("Rename", action => ShowEditableText(_rename, Data.Node.name));
				evt.menu.AppendSeparator();
			}

			evt.menu.AppendAction("View Documentation", action => ViewDocumentation());
			evt.menu.AppendSeparator();

			if (IsStartNode)
				evt.StopPropagation();
		}

		protected override void OnCustomStyleResolved(ICustomStyle styles)
		{
			base.OnCustomStyleResolved(styles);

			_breakColor = styles.TryGetValue(_breakColorProperty, out var breakColor) ? breakColor : _defaultBreakColor;
			_callstackActiveColor = styles.TryGetValue(_callstackActiveColorProperty, out var callstackActiveColor) ? callstackActiveColor : _defaultCallstackActiveColor;
			_inCallstackColor = styles.TryGetValue(_inCallstackColorProperty, out var inCallstackColor) ? inCallstackColor: _defaultInCallstackColor;
		}
	}
}
