using System;
using System.Collections;
using PiRhoSoft.PargonUtilities.Engine;
using PiRhoSoft.UtilityEngine;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[Serializable]
	public class MockupConnection
	{
		public string Name;
		public InstructionGraphNode Node;
	}

	[Serializable]
	public class MockupConnectionList : SerializedList<MockupConnection>
	{
	}
	
	[HelpURL(Composition.DocumentationUrl + "mockup-node")]
	[CreateInstructionGraphNodeMenu("Debug/Mockup", 402)]
	public class MockupNode : InstructionGraphNode
	{
		[Tooltip("The connections from this node")]
		[ListDisplay]
		[ClassDisplay(ClassDisplayType.Inline)]
		public MockupConnectionList Connections = new MockupConnectionList();

		[Tooltip("The display color of the node")]
		public Color DisplayColor = new Color(0.35f, 0.35f, 0.35f);

		public override Color NodeColor => DisplayColor;

		public override IEnumerator Run(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			yield break;
		}

		public override void GetConnections(NodeData data)
		{
			foreach (var node in Connections)
				data.AddConnection(node.Name, node.Node);
		}

		public override void SetConnection(ConnectionData connection, InstructionGraphNode target)
		{
			foreach (var node in Connections)
			{
				if (node.Name == connection.Name)
					node.Node = target;
			}
		}
	}
}
