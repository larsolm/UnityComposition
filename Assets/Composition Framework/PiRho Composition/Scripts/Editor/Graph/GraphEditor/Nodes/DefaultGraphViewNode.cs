using PiRhoSoft.Utilities.Editor;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UIElements;

namespace PiRhoSoft.Composition.Editor
{
	public class DefaultGraphViewNode : GraphViewNode, IInputNode, IOutputNode
	{
		public const string DefaultUssClassName = UssClassName + "--default";
		public const string RenameButtonUssClassName = UssClassName + "__rename-button";
		public const string BreakpointUssClassName = UssClassName + "__breakpoint-button";
		public const string BreakpointUssContainerClassName = UssClassName + "__breakpoint-container";
		public const string BreakpointActiveUssClassName = BreakpointUssClassName + "--active";
		public const string CallstackBorderUssClassName = UssClassName + "__callstack-border";
		public const string BreakUssClassName = CallstackBorderUssClassName + "--break";
		public const string InCallstackUssClassName = CallstackBorderUssClassName + "--in-callstack";
		public const string ActiveInCallstackUssClassName = CallstackBorderUssClassName + "--active-in-callstack";

		protected static readonly Icon _renameIcon = Icon.BuiltIn("editicon.sml");

		public GraphViewInputPort Input { get; private set; }
		public List<GraphViewOutputPort> Outputs { get; private set; }

		protected readonly TextField _rename;
		protected readonly VisualElement _breakpoint;
		protected readonly VisualElement _callstackBorder;
		protected readonly GraphViewConnector _nodeConnector;

		public DefaultGraphViewNode(GraphNode node, GraphViewConnector nodeConnector) : base(node)
		{
			AddToClassList(DefaultUssClassName);

			CreateInput(nodeConnector);
			CreateDeleteButton();

			_rename = CreateRename();
			_breakpoint = CreateBreakpoint();
			_callstackBorder = CreateCallstackBorder();
			_nodeConnector = nodeConnector;

			Outputs = new List<GraphViewOutputPort>(Data.Connections.Count);
			CreateOutputs(Outputs, nodeConnector, null);
		}

		private void CreateInput(GraphViewConnector nodeConnector)
		{
			Input = new GraphViewInputPort(this, nodeConnector) { tooltip = "Drag an output to here to make a connection" };
			titleContainer.Insert(0, Input);
		}

		private TextField CreateRename()
		{
			var label = titleContainer.Q<Label>("title-label");
			label.tooltip = "Double click to rename this node";

			var renameIcon = new IconButton(_renameIcon.Texture, "Rename this node", () => ShowEditableText(_rename));
			renameIcon.AddToClassList(RenameButtonUssClassName);

			titleButtonContainer.Insert(0, renameIcon);

			return CreateEditableLabel(label, "m_Name");
		}

		private VisualElement CreateBreakpoint()
		{
			var breakpoint = new VisualElement();
			breakpoint.AddToClassList(BreakpointUssClassName);

			var breakpointContainer = new VisualElement { tooltip = "Toggle this node as a breakpoint" };
			breakpointContainer.AddToClassList(BreakpointUssContainerClassName);
			breakpointContainer.AddManipulator(new Clickable(ToggleBreakpoint));
			breakpointContainer.Add(breakpoint);

			breakpoint.EnableInClassList(BreakpointActiveUssClassName, Data.Node.IsBreakpoint);

			Input.Add(breakpointContainer);

			return breakpoint;
		}

		private VisualElement CreateCallstackBorder()
		{
			var border = new VisualElement();
			border.AddToClassList(CallstackBorderUssClassName);

			mainContainer.Add(border);

			return border;
		}

		public void UpdateColors(bool active)
		{
			var iteration = 0; // TODO: maybe get title from the node just like color?
			var label = iteration > 0 ? string.Format("{0} ({1})", Data.Node.name, iteration) : Data.Node.name;
			var inCallstack = Data.Node.Graph.IsInCallStack(Data.Node);
			var paused = Data.Node.Graph.DebugState == Graph.PlaybackState.Paused;

			title = label;

			_callstackBorder.EnableInClassList(InCallstackUssClassName, inCallstack);
			_callstackBorder.EnableInClassList(ActiveInCallstackUssClassName, inCallstack && active);
			_callstackBorder.EnableInClassList(BreakUssClassName, paused && active);

			Input.UpdateColor(inCallstack);

			foreach (var output in Outputs)
				output.UpdateColor();
		}

		private void ToggleBreakpoint()
		{
			Data.Node.IsBreakpoint = !Data.Node.IsBreakpoint;

			_breakpoint.EnableInClassList(BreakpointActiveUssClassName, Data.Node.IsBreakpoint);
		}

		private void OnNameChanged(string name)
		{
			title = name;
			Data.Node.name = name;
		}

		public override void OnSelected()
		{
			base.OnSelected();

			Selection.activeObject = Data.Node;
		}

		public override void OnUnselected()
		{
			base.OnUnselected();

			HideEditableText(_rename);
		}

		public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
		{
			evt.menu.AppendAction("Toggle Breakpoint", action => ToggleBreakpoint());
			evt.menu.AppendAction("Rename", action => ShowEditableText(_rename));
			evt.menu.AppendSeparator();
			evt.menu.AppendAction("View Documentation", action => ViewDocumentation(Data.Node.GetType()));
			evt.menu.AppendSeparator();
		}
	}
}
