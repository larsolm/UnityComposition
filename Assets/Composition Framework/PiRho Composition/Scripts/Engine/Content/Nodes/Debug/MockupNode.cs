using PiRhoSoft.PargonUtilities.Engine;
using System;
using System.Collections;
using UnityEngine;

namespace PiRhoSoft.Composition.Engine
{
	[Serializable]
	public class MockupConnection
	{
		public string Name;
		public GraphNode Node;
	}

	[Serializable]
	public class MockupConnectionList : SerializedList<MockupConnection>
	{
	}
	
	[HelpURL(Composition.DocumentationUrl + "mockup-node")]
	[CreateGraphNodeMenu("Debug/Mockup", 402)]
	public class MockupNode : GraphNode
	{
		[Tooltip("The connections from this node")]
		[List]
		[Inline]
		public MockupConnectionList Connections = new MockupConnectionList();

		[Tooltip("The display color of the node")]
		public Color DisplayColor = new Color(0.35f, 0.35f, 0.35f);

		public override Color NodeColor => DisplayColor;

		public override IEnumerator Run(Graph graph, GraphStore variables, int iteration)
		{
			yield break;
		}

		public override void GetConnections(NodeData data)
		{
			foreach (var node in Connections)
				data.AddConnection(node.Name, node.Node);
		}

		public override void SetConnection(ConnectionData connection, GraphNode target)
		{
			foreach (var node in Connections)
			{
				if (node.Name == connection.Name)
					node.Node = target;
			}
		}
	}
}
