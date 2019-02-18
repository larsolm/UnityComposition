using System;
using System.Collections;
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

	[CreateInstructionGraphNodeMenu("General/Mockup")]
	[HelpURL(Composition.DocumentationUrl + "mockup")]
	public class MockupNode : InstructionGraphNode
	{
		[Tooltip("The type of node to emulate")]
		[SerializeField]
		[EnumButtons]
		private InstructionGraphExecutionMode _executionMode = InstructionGraphExecutionMode.Normal;

		[Tooltip("The connections from this node")]
		[ListDisplay(ItemDisplay = ListItemDisplayType.Inline)]
		public MockupConnectionList Connections = new MockupConnectionList();

		public override bool IsExecutionImmediate => false;
		public override InstructionGraphExecutionMode ExecutionMode => _executionMode;

		protected override IEnumerator Run_(InstructionGraph graph, InstructionStore variables, int iteration)
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
