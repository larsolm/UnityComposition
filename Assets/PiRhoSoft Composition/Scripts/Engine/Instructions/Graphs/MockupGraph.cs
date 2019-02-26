using System.Collections;
using PiRhoSoft.UtilityEngine;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[CreateAssetMenu(fileName = nameof(SimpleGraph), menuName = "Composition/Instruction Graph - Mockup", order = 145)]
	public class MockupGraph : InstructionGraph
	{
		[Tooltip("The root nodes")]
		[ListDisplay(ItemDisplay = ListItemDisplayType.Inline)]
		public MockupConnectionList EntryPoints = new MockupConnectionList();

		public override void GetConnections(InstructionGraphNode.NodeData data)
		{
			foreach (var node in EntryPoints)
				data.AddConnection(node.Name, node.Node);
		}

		public override void SetConnection(InstructionGraphNode.ConnectionData connection, InstructionGraphNode target)
		{
			foreach (var node in EntryPoints)
			{
				if (node.Name == connection.Name)
					node.Node = target;
			}
		}

		protected override IEnumerator Run(InstructionStore variables)
		{
			foreach (var node in EntryPoints)
				yield return Run(variables, node.Node, node.Name);
		}
	}
}
