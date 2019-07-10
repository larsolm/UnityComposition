﻿using PiRhoSoft.Composition.Engine;
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
		private const string _ussBreakClass = "break-button";
		private const string _ussBreakContainerClass = "break-container";
		private const string _ussEnableClass = "enabled";

		private static readonly Color _edgeColor = new Color(0.49f, 0.73f, 1.0f, 1.0f);
		private static readonly Color _breakColor = new Color(1.0f, 0.2f, 0.2f, 1.0f);
		private static readonly Color _activeColor = new Color(0.0f, 0.9f, 0.0f, 1.0f);
		private static readonly Color _callstackColor = new Color(0.3f, 0.8f, 0.3f, 1.0f);

		private static readonly Color _portColor = new Color(0.572549045f, 0.572549045f, 0.572549045f);

		public GraphViewInputPort Input { get; private set; }
		public List<GraphViewOutputPort> Outputs { get; private set; }

		private readonly Graph _graph;
		private readonly VisualElement _breakpoint;
		private readonly TextField _rename;

		public DefaultGraphViewNode(Graph graph, GraphNode node, GraphViewConnector nodeConnector, bool isStart) : base(node, isStart)
		{
			_graph = graph;

			Outputs = new List<GraphViewOutputPort>(Data.Connections.Count);

			if (!IsStartNode)
			{
				CreateInput(nodeConnector);
				CreateDeleteButton();

				_rename = CreateRename();
				_breakpoint = CreateBreakpoint();
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

		private VisualElement CreateBreakpoint()
		{
			var breakpointContainer = new VisualElement { tooltip = "Toggle this node as a breakpoint" };
			breakpointContainer.AddToClassList(_ussBreakContainerClass);
			breakpointContainer.AddManipulator(new Clickable(ToggleBreakpoint));

			var breakpoint = new VisualElement();
			breakpoint.AddToClassList(_ussBreakClass);

			breakpointContainer.Add(_breakpoint);

			ElementHelper.ToggleClass(breakpoint, _ussEnableClass, Data.Node.IsBreakpoint);

			Input.Add(breakpointContainer);

			return breakpoint;
		}

		public void UpdateColors(bool active, int iteration)
		{
			var inCallstack = _graph.IsInCallStack(Data.Node);
			var paused = _graph.DebugState == Graph.PlaybackState.Paused;
			//var nodeColor = active ? (paused ? _breakColor : _activeColor) : inCallstack ? _callstackColor : _edgeColor;
			var outputs = Input.connections.Select(edge => edge.output).OfType<GraphViewOutputPort>();
			var label = !IsStartNode && iteration > 0 ? string.Format("{0} ({1})", Data.Node.name, iteration) : Data.Node.name;

			title = label;

			Input.portColor = inCallstack ? _callstackColor : _edgeColor;

			foreach (var output in outputs)
				output.portColor = _graph.IsInCallStack(Data.Node, output.Node.Data.Node.name) ? _callstackColor : _edgeColor;
		}

		private void ToggleBreakpoint()
		{
			Data.Node.IsBreakpoint = !Data.Node.IsBreakpoint;
			ElementHelper.ToggleClass(_breakpoint, _ussEnableClass, Data.Node.IsBreakpoint);
		}

		public override void OnUnselected()
		{
			base.OnUnselected();

			HideEditableText(_rename);
		}

		private void OnNameChanged(string name)
		{
			title = name;
			Data.Node.name = name;
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
	}
}
