﻿using PiRhoSoft.CompositionEngine;
using PiRhoSoft.PargonUtilities.Editor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.CompositionEditor
{
	public class InstructionGraphViewNode : Node
	{
		private const string _ussDeleteClass = "delete-button";

		private static readonly Icon _deleteIcon = Icon.BuiltIn("d_LookDevClose");

		public InstructionGraphNode.NodeData Data { get; private set; }
		public InstructionGraphViewInputPort Input { get; private set; }

		public bool IsStartNode { get; private set; }
		public override bool IsAscendable() => true;
		public override bool IsDroppable() => false;
		public override bool IsMovable() => !IsStartNode;
		public override bool IsResizable() => false;
		public override bool IsRenamable() => !IsStartNode;
		public override bool IsSelectable() => true;
		protected override void ToggleCollapse() { }

		private readonly InstructionGraph _graph;

		public InstructionGraphViewNode(InstructionGraph graph, InstructionGraphNode node, InstructionGraphViewConnector nodeConnector, bool isStart)
		{
			_graph = graph;

			IsStartNode = isStart;
			Data = new InstructionGraphNode.NodeData(node);
			Data.Node.GetConnections(Data);

			title = node.Name;

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

		private void CreateInput(InstructionGraphViewConnector nodeConnector)
		{
			Input = new InstructionGraphViewInputPort(this, nodeConnector);

			var deleteButton = new Image { image = _deleteIcon.Content };
			deleteButton.AddToClassList(_ussDeleteClass);
			deleteButton.AddManipulator(new Clickable(DeleteNode));

			titleContainer.Insert(0, Input);
			titleButtonContainer.Add(deleteButton);
		}

		private void CreateOutputs(InstructionGraphViewConnector nodeConnector)
		{
			foreach (var connection in Data.Connections)
				outputContainer.Add(new InstructionGraphViewOutputPort(this, connection, nodeConnector) { portName = connection.Name });
		}

		private void DeleteNode()
		{
		}
	}
}
